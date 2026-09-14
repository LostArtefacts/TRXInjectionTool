using System.Reflection;
using System.Runtime.Loader;

namespace TRXInjectionTool;

public static class PluginLoader
{
    // Builder plugins are DLLs dropped into Plugins/ beside the executable.
    // All plugins share one load context so they can reference each other;
    // assemblies the host provides (the SDK, TombIO libraries and their
    // dependencies) resolve to the host's copies.
    public static List<Assembly> LoadPlugins()
    {
        var pluginDir = Path.Combine(AppContext.BaseDirectory, "Plugins");
        if (!Directory.Exists(pluginDir))
        {
            return [];
        }

        var dlls = Directory.GetFiles(pluginDir, "*.dll", SearchOption.AllDirectories);
        var context = new PluginLoadContext(dlls);

        var assemblies = new List<Assembly>();
        foreach (var dll in dlls)
        {
            try
            {
                var assembly = context.LoadFromAssemblyName(
                    new AssemblyName(Path.GetFileNameWithoutExtension(dll)));
                if (!assembly.GetTypes().Any(t => t.IsSubclassOf(typeof(InjectionBuilder)) && !t.IsAbstract))
                {
                    continue;
                }

                var stamp = assembly.GetCustomAttribute<TRXPluginAttribute>();
                if (stamp == null)
                {
                    Warn($"skipping plugin {Path.GetFileName(dll)}: missing [assembly: TRXPlugin] stamp");
                    continue;
                }
                if (stamp.BinIteration != SdkInfo.BinIteration)
                {
                    Warn($"skipping plugin {Path.GetFileName(dll)}: built for bin iteration " +
                        $"{stamp.BinIteration}, host writes {SdkInfo.BinIteration}");
                    continue;
                }

                assemblies.Add(assembly);
            }
            catch (Exception e) when (e is BadImageFormatException or ReflectionTypeLoadException)
            {
                Warn($"skipping plugin {Path.GetFileName(dll)}: {e.Message}");
            }
        }

        return assemblies;
    }

    private static void Warn(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"WARNING: {message}");
        Console.ResetColor();
    }

    private class PluginLoadContext : AssemblyLoadContext
    {
        private readonly List<AssemblyDependencyResolver> _resolvers;

        public PluginLoadContext(IEnumerable<string> pluginPaths)
            : base("Plugins")
        {
            _resolvers = [.. pluginPaths.Select(p => new AssemblyDependencyResolver(p))];
        }

        protected override Assembly Load(AssemblyName name)
        {
            if (Default.Assemblies.Any(a => a.GetName().Name == name.Name))
            {
                return null;
            }

            foreach (var resolver in _resolvers)
            {
                var path = resolver.ResolveAssemblyToPath(name);
                if (path != null)
                {
                    return LoadFromAssemblyPath(path);
                }
            }

            return null;
        }
    }
}
