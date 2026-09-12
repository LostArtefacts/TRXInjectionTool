namespace TRXInjectionTool.Model;

// A sprite sequence the file brings under a name rather than a slot. Games
// that keep no sprites of this kind have no slot to give it, and a slot past
// what their files use is read as a static sprite instead.
public class TRSymbolSpriteSequence
{
    public int SymbolIndex { get; set; }
    public short SpriteCount { get; set; }
    public short StartIndex { get; set; }
}
