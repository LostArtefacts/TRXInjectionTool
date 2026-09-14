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
    // The number written is a local slot the file's symbol table maps to a
    // name, so what it means does not depend on the numbers another mod took.
    Symbol,
}
