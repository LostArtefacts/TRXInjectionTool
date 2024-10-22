using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Textures;

public class TR1CisternTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level cistern = _control1.Read($"Resources/{TR1LevelNames.CISTERN}");
        InjectionData data = InjectionData.Create(InjectionType.TextureFix, "cistern_textures");

        data.RoomEdits.AddRange(CreateFillers(cistern));
        data.RoomEdits.AddRange(CreateRotations());

        FixRoom9(cistern, data);

        return new() { data };
    }

    private static List<TRRoomTextureCreate> CreateFillers(TR1Level cistern)
    {
        return new()
        {
            new()
            {
                RoomIndex = 3,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 9,
                SourceIndex = 66,
                Vertices = new()
                {
                    cistern.Rooms[3].Mesh.Rectangles[105].Vertices[1],
                    cistern.Rooms[3].Mesh.Rectangles[85].Vertices[1],
                    cistern.Rooms[3].Mesh.Rectangles[83].Vertices[2],
                    cistern.Rooms[3].Mesh.Rectangles[83].Vertices[1],
                }
            }
        };
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return new()
        {
            Rotate(102, TRMeshFaceType.TexturedTriangle, 4, 2),
        };
    }

    private static void FixRoom9(TR1Level cistern, InjectionData data)
    {
        // This fills the one-click gaps around the perimiter of the base of room 9.

        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 9,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 9,
            SourceIndex = 36,
            Vertices = new()
            {
                cistern.Rooms[9].Mesh.Rectangles[35].Vertices[3],
                cistern.Rooms[9].Mesh.Rectangles[35].Vertices[2],
                cistern.Rooms[9].Mesh.Rectangles[36].Vertices[3],
                cistern.Rooms[9].Mesh.Rectangles[39].Vertices[2],
            }
        });
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 9,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 9,
            SourceIndex = 36,
            Vertices = new()
            {
                cistern.Rooms[9].Mesh.Rectangles[52].Vertices[3],
                cistern.Rooms[9].Mesh.Rectangles[52].Vertices[2],
                cistern.Rooms[9].Mesh.Rectangles[39].Vertices[3],
                cistern.Rooms[9].Mesh.Rectangles[36].Vertices[2],
            }
        });

        // New verts
        ushort nextVert = (ushort)cistern.Rooms[9].Mesh.Vertices.Count;
        List<TRFace> rects = cistern.Rooms[9].Mesh.Rectangles;
        data.RoomEdits.Add(MakeRoom9Vertex(cistern, 76, 3));
        data.RoomEdits.Add(MakeRoom9Vertex(cistern, 76, 2));
        data.RoomEdits.Add(MakeRoom9Vertex(cistern, 74, 2));

        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 9,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 9,
            SourceIndex = 36,
            Vertices = new()
            {
                rects[76].Vertices[3],
                rects[76].Vertices[2],
                (ushort)(nextVert + 1),
                nextVert,
            }
        });
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 9,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 9,
            SourceIndex = 36,
            Vertices = new()
            {
                rects[74].Vertices[3],
                rects[74].Vertices[2],
                (ushort)(nextVert + 2),
                (ushort)(nextVert + 1),
            }
        });

        data.RoomEdits.Add(MakeRoom9Vertex(cistern, 64, 2));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 9,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 9,
            SourceIndex = 39,
            Vertices = new()
            {
                rects[64].Vertices[3],
                rects[64].Vertices[2],
                (ushort)(nextVert + 3),
                (ushort)(nextVert + 2),
            }
        });
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 9,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 9,
            SourceIndex = 39,
            Vertices = new()
            {
                rects[50].Vertices[3],
                rects[50].Vertices[2],
                rects[33].Vertices[3],
                (ushort)(nextVert + 3),
            }
        });

        data.RoomEdits.Add(MakeRoom9Vertex(cistern, 86, 2));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 9,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 9,
            SourceIndex = 39,
            Vertices = new()
            {
                rects[75].Vertices[0],
                rects[75].Vertices[1],
                nextVert,
                (ushort)(nextVert + 4),
            }
        });

        data.RoomEdits.Add(MakeRoom9Vertex(cistern, 86, 3));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 9,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 9,
            SourceIndex = 39,
            Vertices = new()
            {
                rects[86].Vertices[3],
                rects[86].Vertices[2],
                (ushort)(nextVert + 4),
                (ushort)(nextVert + 5),
            }
        });

        data.RoomEdits.Add(MakeRoom9Vertex(cistern, 87, 3));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 9,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 9,
            SourceIndex = 39,
            Vertices = new()
            {
                rects[87].Vertices[3],
                rects[87].Vertices[2],
                (ushort)(nextVert + 5),
                (ushort)(nextVert + 6),
            }
        });

        data.RoomEdits.Add(MakeRoom9Vertex(cistern, 88, 3));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 9,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 9,
            SourceIndex = 39,
            Vertices = new()
            {
                rects[88].Vertices[3],
                rects[88].Vertices[2],
                (ushort)(nextVert + 6),
                (ushort)(nextVert + 7),
            }
        });

        data.RoomEdits.Add(MakeRoom9Vertex(cistern, 83, 3));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 9,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 9,
            SourceIndex = 36,
            Vertices = new()
            {
                rects[83].Vertices[3],
                rects[83].Vertices[2],
                (ushort)(nextVert + 7),
                (ushort)(nextVert + 8),
            }
        });

        data.RoomEdits.Add(MakeRoom9Vertex(cistern, 82, 3));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 9,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 9,
            SourceIndex = 36,
            Vertices = new()
            {
                rects[82].Vertices[3],
                rects[82].Vertices[2],
                (ushort)(nextVert + 8),
                (ushort)(nextVert + 9),
            }
        });

        data.RoomEdits.Add(MakeRoom9Vertex(cistern, 73, 2));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 9,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 9,
            SourceIndex = 36,
            Vertices = new()
            {
                rects[73].Vertices[2],
                rects[73].Vertices[1],
                (ushort)(nextVert + 9),
                (ushort)(nextVert + 10),
            }
        });

        data.RoomEdits.Add(MakeRoom9Vertex(cistern, 62, 2));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 9,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 9,
            SourceIndex = 36,
            Vertices = new()
            {
                rects[62].Vertices[2],
                rects[62].Vertices[1],
                (ushort)(nextVert + 10),
                (ushort)(nextVert + 11),
            }
        });

        data.RoomEdits.Add(MakeRoom9Vertex(cistern, 47, 2));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 9,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 9,
            SourceIndex = 36,
            Vertices = new()
            {
                rects[47].Vertices[2],
                rects[47].Vertices[1],
                (ushort)(nextVert + 11),
                (ushort)(nextVert + 12),
            }
        });

        data.RoomEdits.Add(MakeRoom9Vertex(cistern, 30, 2));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 9,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 9,
            SourceIndex = 36,
            Vertices = new()
            {
                rects[30].Vertices[2],
                rects[30].Vertices[1],
                (ushort)(nextVert + 12),
                (ushort)(nextVert + 13),
            }
        });

        data.RoomEdits.Add(MakeRoom9Vertex(cistern, 18, 2));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 9,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 9,
            SourceIndex = 36,
            Vertices = new()
            {
                rects[18].Vertices[2],
                rects[18].Vertices[1],
                (ushort)(nextVert + 13),
                (ushort)(nextVert + 14),
            }
        });

        data.RoomEdits.Add(MakeRoom9Vertex(cistern, 17, 3));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 9,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 9,
            SourceIndex = 39,
            Vertices = new()
            {
                rects[17].Vertices[3],
                rects[17].Vertices[2],
                (ushort)(nextVert + 14),
                (ushort)(nextVert + 15),
            }
        });

        data.RoomEdits.Add(MakeRoom9Vertex(cistern, 12, 3));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 9,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 9,
            SourceIndex = 39,
            Vertices = new()
            {
                rects[12].Vertices[3],
                rects[12].Vertices[2],
                (ushort)(nextVert + 15),
                (ushort)(nextVert + 16),
            }
        });

        data.RoomEdits.Add(MakeRoom9Vertex(cistern, 13, 3));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 9,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 9,
            SourceIndex = 36,
            Vertices = new()
            {
                rects[13].Vertices[3],
                rects[13].Vertices[2],
                (ushort)(nextVert + 16),
                (ushort)(nextVert + 17),
            }
        });

        data.RoomEdits.Add(MakeRoom9Vertex(cistern, 28, 2));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 9,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 9,
            SourceIndex = 36,
            Vertices = new()
            {
                rects[28].Vertices[2],
                rects[28].Vertices[1],
                (ushort)(nextVert + 17),
                (ushort)(nextVert + 18),
            }
        });

        data.RoomEdits.Add(MakeRoom9Vertex(cistern, 45, 2));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 9,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 9,
            SourceIndex = 36,
            Vertices = new()
            {
                rects[45].Vertices[2],
                rects[45].Vertices[1],
                (ushort)(nextVert + 18),
                (ushort)(nextVert + 19),
            }
        });

        data.RoomEdits.Add(MakeRoom9Vertex(cistern, 59, 3));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 9,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 9,
            SourceIndex = 36,
            Vertices = new()
            {
                rects[59].Vertices[3],
                rects[59].Vertices[2],
                (ushort)(nextVert + 19),
                (ushort)(nextVert + 20),
            }
        });

        data.RoomEdits.Add(MakeRoom9Vertex(cistern, 69, 3));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 9,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 9,
            SourceIndex = 36,
            Vertices = new()
            {
                rects[69].Vertices[3],
                rects[69].Vertices[2],
                (ushort)(nextVert + 20),
                (ushort)(nextVert + 21),
            }
        });

        data.RoomEdits.Add(MakeRoom9Vertex(cistern, 56, 2));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 9,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 9,
            SourceIndex = 36,
            Vertices = new()
            {
                rects[56].Vertices[2],
                rects[56].Vertices[1],
                (ushort)(nextVert + 21),
                (ushort)(nextVert + 22),
            }
        });

        data.RoomEdits.Add(MakeRoom9Vertex(cistern, 21, 0));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 9,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 9,
            SourceIndex = 39,
            Vertices = new()
            {
                rects[21].Vertices[0],
                rects[21].Vertices[1],
                rects[39].Vertices[2],
                (ushort)(nextVert + 23),
            }
        });

        data.RoomEdits.Add(MakeRoom9Vertex(cistern, 6, 2));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 9,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 9,
            SourceIndex = 39,
            Vertices = new()
            {
                rects[6].Vertices[2],
                rects[6].Vertices[3],
                (ushort)(nextVert + 23),
                (ushort)(nextVert + 24),
            }
        });

        data.RoomEdits.Add(MakeRoom9Vertex(cistern, 8, 3));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 9,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 9,
            SourceIndex = 36,
            Vertices = new()
            {
                rects[8].Vertices[3],
                rects[8].Vertices[2],
                (ushort)(nextVert + 24),
                (ushort)(nextVert + 25),
            }
        });

        data.RoomEdits.Add(MakeRoom9Vertex(cistern, 7, 3));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 9,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 9,
            SourceIndex = 39,
            Vertices = new()
            {
                rects[7].Vertices[3],
                rects[7].Vertices[2],
                (ushort)(nextVert + 25),
                (ushort)(nextVert + 26),
            }
        });

        data.RoomEdits.Add(MakeRoom9Vertex(cistern, 5, 3));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 9,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 9,
            SourceIndex = 39,
            Vertices = new()
            {
                rects[5].Vertices[3],
                rects[5].Vertices[2],
                (ushort)(nextVert + 26),
                (ushort)(nextVert + 27),
            }
        });

        data.RoomEdits.Add(MakeRoom9Vertex(cistern, 1, 3));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 9,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 9,
            SourceIndex = 39,
            Vertices = new()
            {
                rects[1].Vertices[3],
                rects[1].Vertices[2],
                (ushort)(nextVert + 27),
                (ushort)(nextVert + 28),
            }
        });

        data.RoomEdits.Add(MakeRoom9Vertex(cistern, 2, 3));
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 9,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 9,
            SourceIndex = 39,
            Vertices = new()
            {
                rects[2].Vertices[3],
                rects[2].Vertices[2],
                (ushort)(nextVert + 28),
                (ushort)(nextVert + 29),
            }
        });

        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 9,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 9,
            SourceIndex = 39,
            Vertices = new()
            {
                rects[20].Vertices[3],
                rects[20].Vertices[2],
                (ushort)(nextVert + 29),
                rects[33].Vertices[2],
            }
        });
    }

    private static TRRoomVertexCreate MakeRoom9Vertex(TR1Level cistern, short rect, short vert)
    {
        TR1RoomVertex copy = cistern.Rooms[9].Mesh.Vertices[cistern.Rooms[9].Mesh.Rectangles[rect].Vertices[vert]];
        return new TRRoomVertexCreate
        {
            RoomIndex = 9,
            Vertex = new()
            {
                Lighting = copy.Lighting,
                Vertex = new()
                {
                    X = copy.Vertex.X,
                    Y = (short)(copy.Vertex.Y + 256),
                    Z = copy.Vertex.Z
                }
            }
        };
    }
}
