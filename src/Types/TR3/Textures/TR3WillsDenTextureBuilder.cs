using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Textures;

public class TR3WillsDenTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.TextureFix, "willsden_textures");

        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.LAIR}");
        data.RoomEdits.AddRange(CreateRotations());
        data.RoomEdits.AddRange(CreateRefacings(level));

        return [data];
    }

    private static IEnumerable<TRRoomTextureRotate> CreateRotations()
    {
        yield return Rotate(52, TRMeshFaceType.TexturedQuad, 6, 2);
    }

    private static IEnumerable<TRRoomTextureReface> CreateRefacings(TR3Level level)
    {
        yield return Reface(level, 11, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1765, 10);
        yield return Reface(level, 11, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedTriangle, 1809, 43);
    }
}
