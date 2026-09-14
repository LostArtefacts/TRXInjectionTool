namespace TRXInjectionTool.Model;

// One name the file brings, the catalog it belongs to, and the local slot it
// stands for. A record that means the name writes the same slot with a symbol
// marker, so the file says what it means rather than what number it happened
// to be given.
public class TRSymbol
{
    public SymbolContext Context { get; set; }
    public int Slot { get; set; }
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
