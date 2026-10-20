using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR4.Textures;

public class TR4ValleyTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR4, InjectionType.TextureFix, "valley_textures");
        CreateDefaultTests(data, $"TR4/{TR4LevelNames.VALLEY}");
        var level = _control4.Read($"Resources/TR4/{TR4LevelNames.VALLEY}");
        data.RoomEdits.AddRange(CreateRefacings(level));
        data.RoomEdits.AddRange(CreateRotations());

        return [data];
    }

    private static IEnumerable<TRRoomTextureReface> CreateRefacings(TR4Level level)
    {
        yield return Reface(level, 25, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedTriangle, 1721, 1);
    }

    private static IEnumerable<TRRoomTextureRotate> CreateRotations()
    {
        yield return Rotate(25, TRMeshFaceType.TexturedTriangle, 1, 2);
    }
}
