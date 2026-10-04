using TRLevelControl.Model;
using TRXInjectionTool.Model;

namespace TRXInjectionTool.Control;

// Adds models to a level under names rather than slots. The engine creates
// each name in its object catalog when it loads the file, so a model added
// this way needs no slot in any game.
//
// The format still pairs every symbol with a key local to the file, which
// records use to point at the name. This class takes the keys past every slot
// the level holds, so that they cannot clash with a model added by slot.
public class NamedModels
{
    private readonly List<(string Name, TRModel Model)> _models = [];

    public void Add(string name, TRModel model)
    {
        if (_models.Any(m => m.Name == name))
        {
            throw new ArgumentException($"{name} is named twice");
        }
        _models.Add((name, model));
    }

    // Adds the models to the level, and returns the symbols that name them.
    // Add the symbols to the injection data created from the level.
    public List<TRSymbol> AddTo(TRLevelBase level) => level switch
    {
        TR1Level l => AddTo(l.Models),
        TR2Level l => AddTo(l.Models),
        TR3Level l => AddTo(l.Models),
        TR4Level l => AddTo(l.Models),
        _ => throw new NotSupportedException($"{level.GetType().Name} cannot hold named models"),
    };

    private List<TRSymbol> AddTo<T>(TRDictionary<T, TRModel> models)
        where T : struct, Enum
    {
        var next = models.Keys.Select(k => Convert.ToInt32(k)).DefaultIfEmpty(0).Max() + 1;
        var symbols = new List<TRSymbol>();
        foreach (var (name, model) in _models)
        {
            models[(T)Enum.ToObject(typeof(T), next)] = model;
            symbols.Add(new()
            {
                Context = SymbolContext.Objects,
                Slot = next++,
                Name = name,
            });
        }
        return symbols;
    }
}
