using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Textures;

public class TR2OperaTextureBuilder : TextureBuilder
{
    public override string ID => "opera_textures";

    public override List<InjectionData> Build()
    {
        TR2Level opera = _control2.Read($"Resources/{TR2LevelNames.OPERA}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.TextureFix, "opera_textures");
        CreateDefaultTests(data, TR2LevelNames.OPERA);
        data.RoomEdits.AddRange(CreateRefacings());
        return new() { data };
    }

    private static List<TRRoomTextureReface> CreateRefacings()
    {
        return new[]{ 33, 35, 45, 54, 66, 69, 85, 87 }
            .Select(i => new TRRoomTextureReface
            {
                RoomIndex = 134,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 134,
                SourceIndex = 0,
                TargetIndex = (short)i,
            })
            .ToList();
    }
}
