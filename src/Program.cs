using System.Reflection;
using TRXInjectionTool.Control;

namespace TRXInjectionTool;

internal class Program
{
    static readonly string _topNS = "TRXInjectionTool.Types";

    static void Main()
    {
        Type[] types = Assembly.GetExecutingAssembly().GetTypes();
        List<string> namespaces = types
            .Where(t => t.Namespace != null && t.Namespace.StartsWith(_topNS) && t.Namespace.Length > _topNS.Length)
            .Select(t => t.Namespace[(_topNS.Length + 1)..])
            .Distinct()
            .OrderBy(t => t)
            .ToList();

        while (true)
        {
            Console.WriteLine("Select a namespace to run. All builders that match will be run.");
            Console.WriteLine();

            Console.WriteLine("[0] - Run all");
            for (int i = 0; i < namespaces.Count; i++)
            {
                Console.WriteLine($"[{i + 1}] - {namespaces[i]}");
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
            while (result < 0 || result > namespaces.Count);

            Console.WriteLine();
            List<string> selectedNS = result == 0 ? namespaces : namespaces.FindAll(n => n == namespaces[result - 1]);
            HashSet<string> usedNames = new();

            foreach (string ns in selectedNS)
            {
                Console.WriteLine(ns);
                IEnumerable<Type> builders = types
                    .Where(t => t.IsSubclassOf(typeof(InjectionBuilder)) && t.Namespace == $"{_topNS}.{ns}");
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
                }
                Console.WriteLine();
            }

            Console.WriteLine("Done!");
            Console.WriteLine();
        }
    }
}
