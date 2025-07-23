using System.Drawing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Textures;

public class TR2VegasTextureBuilder : TextureBuilder
{
    public override string ID => "vegas_textures";

    public override List<InjectionData> Build()
    {
        TR2Level level = _control2.Read($"Resources/{TR2LevelNames.VEGAS}");
        InjectionData data = CreateBaseData();

        data.RoomEdits.AddRange(CreateRotations());
        FixTV(level, data);

        FixPassport(level, data);

        return new() { data };
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return new()
        {
            Rotate(4, TRMeshFaceType.TexturedQuad, 64, 1),
        };
    }

    private static void FixTV(TR2Level level, InjectionData data)
    {
        var mesh = level.StaticMeshes[TR2Type.SceneryBase + 15].Mesh;
        FixTransparentPixels(level, data, mesh.TexturedRectangles[1], Color.FromArgb(148, 148, 148));
        FixTransparentPixels(level, data, mesh.TexturedRectangles[2], Color.FromArgb(148, 148, 148));
    }

    private InjectionData CreateBaseData()
    {
        TR2Level level = CreateWinstonLevel(TR2LevelNames.VEGAS);
        // Current injection limitation, do not replace SFX
        level.SoundEffects.Clear();

        FixToilets(level, TR2LevelNames.VEGAS);

        InjectionData data = InjectionData.Create(level, InjectionType.TextureFix, ID);
        CreateDefaultTests(data, TR2LevelNames.VEGAS);

        return data;
    }
}
