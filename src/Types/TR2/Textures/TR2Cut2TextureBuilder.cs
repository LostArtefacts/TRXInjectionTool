using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Textures;

public class TR2Cut2TextureBuilder : TextureBuilder
{
    public override string ID => "cut2_textures";

    public override List<InjectionData> Build()
    {
        TR2Level level = _control2.Read($"Resources/{TR2LevelNames.OPERA_CUT}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.TextureFix, ID);
        CreateDefaultTests(data, TR2LevelNames.OPERA_CUT);

        data.RoomEdits.AddRange(CreateRefacings(level));

        return new() { data };
    }

    private static IEnumerable<TRRoomTextureReface> CreateRefacings(TR2Level level)
    {
        for (int i = 0; i < level.Rooms[0].Mesh.Rectangles.Count; i++)
        {
            if (level.Rooms[0].Mesh.Rectangles[i].Texture == 813)
            {
                yield return Reface(level, 0, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 812, (short)i);
            }
        }
    }
}
