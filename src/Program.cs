using System.Reflection;
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
            .Where(x => x.Builder.ID != null && x.Builder.ID != "")
            .ToDictionary(x => x.Builder.ID, x => x.Type);

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
                RunBuilders(builders, usedNames);
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

        // Handle global options
        var first = args[0].ToLowerInvariant();
        if (first == "-h" || first == "--help")
        {
            PrintHelp();
            return 0;
        } else if (first == "-l" || first == "--list")
        {
            PrintList();
            return 0;
        }

        var id = args[0].ToLowerInvariant().Trim();
        if (!_builders.TryGetValue(id, out var builderType))
        {
            Console.WriteLine($"No matching injection builders for given ID '{id}'.");
            return 1;
        }

        RunBuilders(new[] { builderType });
        return 0;
    }

    private static void RunBuilders(IEnumerable<Type> builders, HashSet<string> usedNames = null)
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

            AssetPublisher.OnBuilderRun(builder);
        }

        AssetPublisher.Publish();
        Console.WriteLine();
    }

    private static void PrintHelp()
    {
        Console.WriteLine("Usage: TRXInjectionTool [options] <builderId>");
        Console.WriteLine("Options:");
        Console.WriteLine("  -h, --help     Show help");
        Console.WriteLine("  -l, --list     List available builder IDs");
    }

    private static void PrintList()
    {
        Console.WriteLine("Available builder IDs:");
        foreach (var id in _builders.Keys.OrderBy(id => id))
        {
            Console.WriteLine($"  {id}");
        }
    }
}
