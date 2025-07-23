using System.Drawing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Textures;

public class TR2WreckTextureBuilder : TextureBuilder
{
    public override string ID => "wreck_textures";

    public override List<InjectionData> Build()
    {
        TR2Level level = _control2.Read($"Resources/{TR2LevelNames.DORIA}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.TextureFix, ID);
        CreateDefaultTests(data, TR2LevelNames.DORIA);

        data.RoomEdits.AddRange(CreateShifts(level));
        data.RoomEdits.AddRange(CreateRotations());

        FixTransparentPixels(level, data, level.Rooms[31].Mesh.Rectangles[23], Color.FromArgb(189, 222, 230));
        FixPassport(level, data);

        return new() { data };
    }

    private static List<TRRoomTextureMove> CreateShifts(TR2Level level)
    {
        return new()
        {
            new()
            {
                RoomIndex = 104,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 34,
                VertexRemap = new()
                {
                    new()
                    {
                        Index = 2,
                        NewVertexIndex = level.Rooms[104].Mesh.Rectangles[33].Vertices[3],
                    },
                    new()
                    {
                        Index = 3,
                        NewVertexIndex = level.Rooms[104].Mesh.Rectangles[55].Vertices[2],
                    }
                }
            },
            new()
            {
                RoomIndex = 104,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 35,
                VertexRemap = new()
                {
                    new()
                    {
                        Index = 0,
                        NewVertexIndex = level.Rooms[104].Mesh.Rectangles[55].Vertices[2],
                    },
                    new()
                    {
                        Index = 1,
                        NewVertexIndex = level.Rooms[104].Mesh.Rectangles[33].Vertices[3],
                    }
                }
            },
        };
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return new()
        {
            Rotate(80, TRMeshFaceType.TexturedTriangle, 19, 1),
        };
    }
}
