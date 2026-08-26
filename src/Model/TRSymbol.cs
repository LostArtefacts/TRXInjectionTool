namespace TRXInjectionTool.Model;

// One name the file brings, and the catalog it belongs to. A chunk states this
// symbol's index in place of a slot, so the file says what it means rather than
// what number it happened to be given.
public class TRSymbol
{
    public SymbolContext Context { get; set; }
    public string Name { get; set; }
}

public enum SymbolContext
{
    Objects     = 0,
    Music       = 1,
    Samples     = 2,
    LaraStates  = 3,
    LaraAnims   = 4,
    ItemActions = 5,
}
