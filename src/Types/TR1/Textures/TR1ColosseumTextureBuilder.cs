using TRImageControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Textures;

public class TR1ColosseumTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level colosseum = _control1.Read($"Resources/{TR1LevelNames.COLOSSEUM}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.TextureFix, "colosseum_textures");
        CreateDefaultTests(data, TR1LevelNames.COLOSSEUM);

        data.RoomEdits.AddRange(CreateVertices(colosseum));
        data.RoomEdits.AddRange(CreateFillers(colosseum));
        data.RoomEdits.AddRange(CreateRefacings());
        data.RoomEdits.AddRange(CreateVertexShifts(colosseum));
        data.RoomEdits.AddRange(CreateRotations());

        FixRoofTextures(data);
        FixPassport(colosseum, data);

        return new() { data };
    }

    private static List<TRRoomVertexCreate> CreateVertices(TR1Level colosseum)
    {
        TRRoomVertex copy = colosseum.Rooms[2].Mesh.Vertices[colosseum.Rooms[2].Mesh.Rectangles[13].Vertices[1]];
        return new()
        {
            new()
            {
                RoomIndex = 2,
                Vertex = new()
                {
                    Lighting = colosseum.Rooms[7].Mesh.Vertices[colosseum.Rooms[7].Mesh.Rectangles[1].Vertices[0]].Lighting,
                    Vertex = new()
                    {
                        X = copy.Vertex.X,
                        Y = (short)(copy.Vertex.Y + 512),
                        Z = copy.Vertex.Z
                    }
                }
            },
        };
    }

    private static List<TRRoomTextureCreate> CreateFillers(TR1Level colosseum)
    {
        return new()
        {
            new()
            {
                RoomIndex = 2,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 2,
                SourceIndex = 1,
                Vertices = new()
                {
                    (ushort)colosseum.Rooms[2].Mesh.Vertices.Count,
                    colosseum.Rooms[2].Mesh.Rectangles[13].Vertices[1],
                    colosseum.Rooms[2].Mesh.Rectangles[2].Vertices[0],
                }
            },
        };
    }

    private static List<TRRoomTextureReface> CreateRefacings()
    {
        return new()
        {
            new()
            {
                RoomIndex = 82,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 82,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 3,
                TargetIndex = 5
            },
            new()
            {
                RoomIndex = 37,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 37,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 36,
                TargetIndex = 46
            },
            new()
            {
                RoomIndex = 75,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 75,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 34,
                TargetIndex = 12
            },
            new()
            {
                RoomIndex = 67,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 67,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 7,
                TargetIndex = 0
            }
        };
    }

    private static List<TRRoomVertexMove> CreateVertexShifts(TR1Level colosseum)
    {
        return new()
        {
            new()
            {
                RoomIndex = 2,
                VertexIndex = colosseum.Rooms[2].Mesh.Rectangles[2].Vertices[0],
                VertexChange = new() { Y = -256 }
            },
        };
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return new()
        {
            Rotate(0, TRMeshFaceType.TexturedQuad, 4, 2),
            Rotate(83, TRMeshFaceType.TexturedQuad, 59, 2),
        };
    }

    private static void FixRoofTextures(InjectionData data)
    {
        // Replace the Midas textures used on the roof of Colosseum with
        // those from the beta.
        TRImage betaRoof = new("Resources/TR1/Colosseum/roof.png");
        data.TextureOverwrites.Add(new()
        {
            Page = 3,
            X = 128,
            Y = 0,
            Width = 128,
            Height = 64,
            Data = betaRoof.ToRGBA(),
        });
    }
}
