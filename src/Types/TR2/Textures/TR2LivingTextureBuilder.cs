using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Textures;

public class TR2LivingTextureBuilder : TextureBuilder
{
    public override string ID => "living_textures";

    public override List<InjectionData> Build()
    {
        TR2Level level = _control2.Read($"Resources/{TR2LevelNames.LQ}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.TextureFix, ID);
        CreateDefaultTests(data, TR2LevelNames.LQ);

        data.RoomEdits.AddRange(CreateRefacings(level));

        FixPassport(level, data);

        return new() { data };
    }

    private static IEnumerable<TRRoomTextureReface> CreateRefacings(TR2Level level)
    {
        foreach (int r in new[] { 43, 44 })
        {
            var mesh = level.Rooms[r].Mesh;
            for (short i = 0; i < mesh.Rectangles.Count; i++)
            {
                if (mesh.Rectangles[i].Texture == 1642)
                {
                    yield return Reface(level, (short)r, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1677, i);
                }
            }
        }
    }
}
