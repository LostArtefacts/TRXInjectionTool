namespace TRXInjectionTool.Control;
public enum InjectionType
{
    General,
    Braid,
    TextureFix,
    PS1SFX,
    FDFix,
    LaraAnims,
    ItemRotation,
    Item,
    AlterAnimSprite,
    Skybox,
    PSCrystal,
}

public enum TRObjectType
{
    Game,
    Static2D,
    Static3D,
    // The number written is an index into the file's symbol table rather than a
    // slot, so what it names does not depend on the numbers another mod took.
    Symbol,
}
