using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR4.Textures;

public class TR4SethTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR4, InjectionType.TextureFix, "seth_textures");
        CreateDefaultTests(data, $"TR4/{TR4LevelNames.SETH}");
        var level = _control4.Read($"Resources/TR4/{TR4LevelNames.SETH}");
        data.RoomEdits.AddRange(CreateRefacings(level));

        return [data];
    }

    private static IEnumerable<TRRoomTextureReface> CreateRefacings(TR4Level level)
    {
        yield return Reface(level, 31, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 2039, 5);
        yield return Reface(level, 42, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 2235, 103);
        yield return Reface(level, 126, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1997, 12);
    }
}
