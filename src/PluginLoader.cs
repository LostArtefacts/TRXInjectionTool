using System.Reflection;
using System.Runtime.Loader;

namespace TRXInjectionTool;

public static class PluginLoader
{
    // Builder plugins are DLLs dropped into Plugins/ beside the executable,
    // each with its private dependencies alongside it.
    public static List<Assembly> LoadPlugins()
    {
        var pluginDir = Path.Combine(AppContext.BaseDirectory, "Plugins");
        if (!Directory.Exists(pluginDir))
        {
            return [];
        }

        var assemblies = new List<Assembly>();
        foreach (var dll in Directory.GetFiles(pluginDir, "*.dll", SearchOption.AllDirectories))
        {
            var context = new PluginLoadContext(dll);
            try
            {
                var assembly = context.LoadFromAssemblyName(
                    new AssemblyName(Path.GetFileNameWithoutExtension(dll)));
                if (assembly.GetTypes().Any(t => t.IsSubclassOf(typeof(InjectionBuilder)) && !t.IsAbstract))
                {
                    assemblies.Add(assembly);
                }
            }
            catch (Exception e) when (e is BadImageFormatException or ReflectionTypeLoadException)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"WARNING: skipping plugin {Path.GetFileName(dll)}: {e.Message}");
                Console.ResetColor();
            }
        }

        return assemblies;
    }

    private class PluginLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver _resolver;

        public PluginLoadContext(string pluginPath)
            : base(Path.GetFileNameWithoutExtension(pluginPath))
        {
            _resolver = new(pluginPath);
        }

        protected override Assembly Load(AssemblyName name)
        {
            // The SDK, TombIO and their dependencies must resolve to the
            // host's copies so builder types share identity with the host;
            // only genuinely private plugin dependencies load here.
            if (Default.Assemblies.Any(a => a.GetName().Name == name.Name))
            {
                return null;
            }

            var path = _resolver.ResolveAssemblyToPath(name);
            return path == null ? null : LoadFromAssemblyPath(path);
        }
    }
}
