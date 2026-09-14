namespace TRXInjectionTool.Format;

// Container-level constants for TRXI version 1.
public static class Container
{
    public const string Magic = "TRXI";
    public const int FormatMajor = 1;

    // INJECTION_CHUNK_TYPE
    public const int TextureData = 0;
    public const int TextureInfo = 1;
    public const int MeshData = 2;
    public const int AnimationData = 3;
    public const int ObjectData = 4;
    public const int SfxData = 5;
    public const int DataEdits = 6;
    public const int CameraData = 7;
    public const int Symbols = 8;

    // Write order; SYMBOLS first so later records may reference the table.
    // Each chunk type carries its own layout version, starting at 1.
    public static readonly (int Type, string Name, int Version)[] Chunks =
    [
        (Symbols, "SYMBOLS", 1),
        (TextureData, "TEXTURE_DATA", 1),
        (TextureInfo, "TEXTURE_INFO", 1),
        (MeshData, "MESH_DATA", 1),
        (AnimationData, "ANIMATION_DATA", 1),
        (ObjectData, "OBJECT_DATA", 1),
        (SfxData, "SFX_DATA", 1),
        (CameraData, "CAMERA_DATA", 1),
        (DataEdits, "DATA_EDITS", 1),
    ];

    public static readonly (int Value, string Name, string Note)[] FileTypes =
    [
        (0, "GENERAL", "always on"),
        (1, "BRAID", "braid option"),
        (2, "TEXTURE_FIX", "texture fixes option"),
        (3, "PS1_SFX", "PS1 SFX option"),
        (4, "FLOOR_DATA", "floor-data fixes option"),
        (5, "LARA_ANIMS", "responsive Lara option"),
        (6, "ITEM_POSITION", "item-position fixes option"),
        (7, "PS1_ENEMY", "PS1 enemy option"),
        (8, "ALTER_ANIM_SPRITE", "sprite fixes option"),
        (9, "SKYBOX", "skybox option"),
        (10, "PS1_CRYSTAL", "PS1 crystal option"),
    ];

    public static readonly (int Value, string Name)[] FaceTypes =
    [
        (0, "TEXTURED_QUAD"),
        (1, "TEXTURED_TRIANGLE"),
        (2, "COLOURED_QUAD"),
        (3, "COLOURED_TRIANGLE"),
    ];
}
