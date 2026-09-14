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
        InjectionData data = CreateBaseData();

        data.RoomEdits.AddRange(CreateRefacings(opera));
        data.RoomEdits.AddRange(CreateRotations());

        FixPassport(opera, data);
        FixPushButton(data);
        FixDressingTables(data);

        return [data];
    }

    private static List<TRRoomTextureReface> CreateRefacings(TR2Level level)
    {
        var result = new List<TRRoomTextureReface>
        {
            Reface(level, 176, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1729, 9),
        };

        var planeAreaFixes = new[] { 33, 35, 45, 54, 66, 69, 85, 87 }
            .Select(i => new TRRoomTextureReface
            {
                RoomIndex = 134,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 134,
                SourceIndex = 0,
                TargetIndex = (short)i,
            });
        result.AddRange(planeAreaFixes);
        return result;
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return
        [
            Rotate(122, TRMeshFaceType.TexturedTriangle, 1, 1),
        ];
    }

    private static void FixDressingTables(InjectionData data)
    {
        var topIds = new[] { 0, 1, 4, 5 };
        foreach (var id in new[] { 16, 17 })
        {
            data.MeshEdits.Add(new()
            {
                EnforcedType = TRObjectType.Static3D,
                ModelID = (uint)(TR2Type.SceneryBase) + (uint)id,
                VertexEdits = [.. Enumerable.Range(0, 8).Select(i => new TRVertexEdit
                {
                    Index = (short)i,
                    Change = new()
                    {
                        Y = (short)((id == 17 && topIds.Contains(i)) ? -1 : 0),
                        Z = 2,
                    },
                })],
            });
        }
    }

    private InjectionData CreateBaseData()
    {
        var level = _control2.Read($"Resources/{TR2LevelNames.RIG}");
        CreateModelLevel(level, TR2Type.AirplanePropeller);
        level.SoundEffects.Clear();

        FixTR2Propeller(level);

        var data = InjectionData.Create(level, InjectionType.TextureFix, ID);
        CreateDefaultTests(data, TR2LevelNames.OPERA);
        data.SetMeshOnlyModel((uint)TR2Type.AirplanePropeller);
        return data;
    }
}
