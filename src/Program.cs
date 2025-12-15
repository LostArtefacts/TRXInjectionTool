using System.Reflection;
using System.Text.RegularExpressions;
using TRXInjectionTool.Control;
using TRXInjectionTool.Types;

namespace TRXInjectionTool;

internal class Program
{
    static readonly string _topNS = "TRXInjectionTool.Types";
    static Type[] _types;
    static List<string> _namespaces;
    // Mapping of builder ID to builder type (default ID is class name if not assigned)
    static Dictionary<string, Type> _builders = null;

    static int Main(string[] args)
    {
        _types = Assembly.GetExecutingAssembly().GetTypes();
        _namespaces = _types
            .Where(t => t.Namespace != null && t.Namespace.StartsWith(_topNS) && t.Namespace.Length > _topNS.Length)
            .Select(t => t.Namespace[(_topNS.Length + 1)..])
            .Distinct()
            .OrderBy(t => t)
            .ToList();

        // Construct builder ID mapping
        _builders = _types
            .Where(t =>
                t.IsSubclassOf(typeof(InjectionBuilder)) &&
                !t.IsAbstract &&
                t.Namespace != null &&
                t.Namespace.StartsWith(_topNS))
            .Select(t => new { Type = t, Builder = (InjectionBuilder)Activator.CreateInstance(t) })
            .ToDictionary(x => {
                var id = x.Builder.ID;
                if (string.IsNullOrWhiteSpace(id)) {
                    id = ToSnakeCase(x.Type.Name);
                }
                return id.ToLowerInvariant();
            }, x => x.Type);

        if (args.Length == 0)
        {
            RunInteractiveMode();
            return 0;
        }
        else
        {
            return RunCliMode(args);
        }
    }

    private static void RunInteractiveMode()
    {
        while (true)
        {
            Console.WriteLine("Select a namespace to run. All builders that match will be run.");
            Console.WriteLine();

            Console.WriteLine("[0] - Run all");
            for (int i = 0; i < _namespaces.Count; i++)
            {
                Console.WriteLine($"[{i + 1}] - {_namespaces[i]}");
            }
            Console.WriteLine();

            int result = -1;
            do
            {
                Console.Write("Selected namespace: ");
                string option = Console.ReadLine();
                if (!int.TryParse(option, out result))
                {
                    result = -1;
                }
            }
            while (result < 0 || result > _namespaces.Count);

            Console.WriteLine();
            List<string> selectedNS = result == 0 ? _namespaces : _namespaces.FindAll(n => n == _namespaces[result - 1]);
            HashSet<string> usedNames = new();

            foreach (string ns in selectedNS)
            {
                Console.WriteLine(ns);
                IEnumerable<Type> builders = _types
                    .Where(t => t.IsSubclassOf(typeof(InjectionBuilder)) && t.Namespace == $"{_topNS}.{ns}");
                RunBuilders(builders, usedNames, true);
            }

            Console.WriteLine("Done!");
            Console.WriteLine();
        }
    }

    private static int RunCliMode(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Missing injection identifier.");
            PrintHelp();
            return 1;
        }

        bool publishAssets = true;

        // Handle global options
        var globalArgs = new List<string>();
        foreach (var arg in args)
        {
            switch (arg.ToLowerInvariant())
            {
                case "-h":
                case "--help":
                    PrintHelp();
                    return 0;
                case "-l":
                case "--list":
                    PrintList();
                    return 0;
                case "-n":
                case "--no-publish":
                    publishAssets = false;
                    break;
                default:
                    globalArgs.Add(arg);
                    break;
            }
        }

        if (globalArgs.Count < 1)
        {
            Console.WriteLine("Missing injection identifier.");
            PrintHelp();
            return 1;
        }

        var builderTypes = new List<Type>();

        foreach (var idRaw in globalArgs)
        {
            var id = idRaw.ToLowerInvariant().Trim();
            if (!_builders.TryGetValue(id, out var builderType))
            {
                Console.WriteLine($"No matching injection builders for given ID '{id}'.");
                return 1;
            }
            builderTypes.Add(builderType);
        }

        RunBuilders(builderTypes.ToArray(), publishAssets: publishAssets);
        return 0;
    }

    private static void RunBuilders(IEnumerable<Type> builders, HashSet<string> usedNames = null, bool publishAssets = true)
    {
        usedNames ??= new();
        foreach (Type type in builders)
        {
            Console.WriteLine($"\t{type.Name}");
            InjectionBuilder builder = (InjectionBuilder)Activator.CreateInstance(type);
            List<InjectionData> dataGroup = builder.Build();

            foreach (InjectionData data in dataGroup)
            {
                string path = InjectionBuilder.MakeOutputPath(data);
                InjectionIO.Export(data, path);
                if (!usedNames.Add(path))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"\t\tWARNING: {path} is already defined");
                    Console.ResetColor();
                }
            }

            if (publishAssets) {
                AssetPublisher.OnBuilderRun(builder);
            }
        }

        if (publishAssets) {
            AssetPublisher.Publish();
            Console.WriteLine();
        }
    }

    private static void PrintHelp()
    {
        Console.WriteLine("Usage: TRXInjectionTool [options] <builderId>...");
        Console.WriteLine("Options:");
        Console.WriteLine("  -h, --help         Show help");
        Console.WriteLine("  -l, --list         List available builder IDs");
        Console.WriteLine("  -n, --no-publish   Turn off asset publishing");
    }

    private static void PrintList()
    {
        Console.WriteLine("Available builder IDs:");
        foreach (var id in _builders.Keys.OrderBy(id => id))
        {
            Console.WriteLine($"  {id}");
        }
    }

    private static string ToSnakeCase(string input)
    {
        return Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
    }
}
