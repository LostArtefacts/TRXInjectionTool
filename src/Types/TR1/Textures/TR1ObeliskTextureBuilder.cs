using System.Drawing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Textures;

public class TR1ObeliskTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level obelisk = _control1.Read($"Resources/{TR1LevelNames.OBELISK}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.TextureFix, "obelisk_textures");
        CreateDefaultTests(data, TR1LevelNames.OBELISK);

        data.RoomEdits.AddRange(CreateRefacings());
        data.RoomEdits.AddRange(CreateRotations());

        FixCityGaps(obelisk, data);
        FixTransparentTextures(obelisk, data);

        return new() { data };
    }

    private static List<TRRoomTextureReface> CreateRefacings()
    {
        return new()
        {
            new()
            {
                RoomIndex = 42,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 42,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 18,
                TargetIndex = 9
            },
            new()
            {
                RoomIndex = 65,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 31,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 100,
                TargetIndex = 1
            },
            new()
            {
                RoomIndex = 22,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 22,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 17,
                TargetIndex = 20
            },
            new()
            {
                RoomIndex = 23,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 23,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 25,
                TargetIndex = 1
            }
        };
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return new()
        {
            Rotate(42, TRMeshFaceType.TexturedQuad, 9, 1),
            Rotate(23, TRMeshFaceType.TexturedQuad, 1, 3),
        };
    }

    private static void FixCityGaps(TR1Level obelisk, InjectionData data)
    {
        // This adds textures to the gaps "into" City from rooms 8, 20 and 21.
        // This effectively adds fake room fog as we set the lightin to the darkest,
        // but ultimately it prevents the skybox being visible through these holes.

        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 8,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 8,
            SourceIndex = 33,
            Vertices = new()
            {
                obelisk.Rooms[8].Mesh.Rectangles[23].Vertices[0],
                obelisk.Rooms[8].Mesh.Rectangles[23].Vertices[3],
                obelisk.Rooms[8].Mesh.Rectangles[27].Vertices[0],
                obelisk.Rooms[8].Mesh.Rectangles[27].Vertices[3],
            }
        });
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 8,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 8,
            SourceIndex = 33,
            Vertices = new()
            {
                obelisk.Rooms[8].Mesh.Rectangles[38].Vertices[0],
                obelisk.Rooms[8].Mesh.Rectangles[38].Vertices[3],
                obelisk.Rooms[8].Mesh.Rectangles[42].Vertices[2],
                obelisk.Rooms[8].Mesh.Rectangles[42].Vertices[1],
            }
        });
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 8,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 8,
            SourceIndex = 33,
            Vertices = new()
            {
                obelisk.Rooms[8].Mesh.Rectangles[59].Vertices[0],
                obelisk.Rooms[8].Mesh.Rectangles[59].Vertices[3],
                obelisk.Rooms[8].Mesh.Rectangles[60].Vertices[2],
                obelisk.Rooms[8].Mesh.Rectangles[60].Vertices[1],
            }
        });


        List<ushort> vertices = new()
        {
            obelisk.Rooms[8].Mesh.Rectangles[23].Vertices[0],
            obelisk.Rooms[8].Mesh.Rectangles[23].Vertices[3],
            obelisk.Rooms[8].Mesh.Rectangles[27].Vertices[0],
            obelisk.Rooms[8].Mesh.Rectangles[27].Vertices[3],
            obelisk.Rooms[8].Mesh.Rectangles[38].Vertices[0],
            obelisk.Rooms[8].Mesh.Rectangles[38].Vertices[3],
            obelisk.Rooms[8].Mesh.Rectangles[42].Vertices[2],
            obelisk.Rooms[8].Mesh.Rectangles[42].Vertices[1],
            obelisk.Rooms[8].Mesh.Rectangles[59].Vertices[0],
            obelisk.Rooms[8].Mesh.Rectangles[59].Vertices[3],
            obelisk.Rooms[8].Mesh.Rectangles[60].Vertices[2],
            obelisk.Rooms[8].Mesh.Rectangles[60].Vertices[1],
        };

        foreach (ushort vertex in vertices)
        {
            data.RoomEdits.Add(new TRRoomVertexMove
            {
                RoomIndex = 8,
                VertexChange = new(),
                VertexIndex = vertex,
                ShadeChange = (short)(8192 - obelisk.Rooms[8].Mesh.Vertices[vertex].Lighting)
            });
        }

        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 21,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 21,
            SourceIndex = 5,
            Vertices = new()
            {
                obelisk.Rooms[21].Mesh.Triangles[0].Vertices[2],
                obelisk.Rooms[21].Mesh.Triangles[0].Vertices[1],
                obelisk.Rooms[21].Mesh.Rectangles[2].Vertices[2],
                obelisk.Rooms[21].Mesh.Rectangles[6].Vertices[1],
            }
        });
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 21,
            FaceType = TRMeshFaceType.TexturedTriangle,
            SourceRoom = 21,
            SourceIndex = 0,
            Vertices = new()
            {
                obelisk.Rooms[21].Mesh.Triangles[0].Vertices[1],
                obelisk.Rooms[21].Mesh.Rectangles[1].Vertices[0],
                obelisk.Rooms[21].Mesh.Rectangles[2].Vertices[2],
            }
        });
        data.RoomEdits.Add(new TRRoomTextureCreate
        {
            RoomIndex = 20,
            FaceType = TRMeshFaceType.TexturedQuad,
            SourceRoom = 20,
            SourceIndex = 6,
            Vertices = new()
            {
                obelisk.Rooms[20].Mesh.Rectangles[5].Vertices[1],
                obelisk.Rooms[20].Mesh.Rectangles[1].Vertices[0],
                obelisk.Rooms[20].Mesh.Rectangles[2].Vertices[0],
                obelisk.Rooms[20].Mesh.Rectangles[6].Vertices[3],
            }
        });

        {
            vertices = new()
            {
                obelisk.Rooms[21].Mesh.Triangles[0].Vertices[2],
                obelisk.Rooms[21].Mesh.Rectangles[5].Vertices[1],
                obelisk.Rooms[21].Mesh.Rectangles[3].Vertices[0],
                obelisk.Rooms[21].Mesh.Rectangles[0].Vertices[0],
                obelisk.Rooms[21].Mesh.Rectangles[1].Vertices[0],
                obelisk.Rooms[21].Mesh.Rectangles[1].Vertices[3],
                obelisk.Rooms[21].Mesh.Rectangles[6].Vertices[0],
                obelisk.Rooms[21].Mesh.Rectangles[6].Vertices[1],
                obelisk.Rooms[21].Mesh.Rectangles[2].Vertices[2],
            };

            foreach (ushort vertex in vertices)
            {
                data.RoomEdits.Add(new TRRoomVertexMove
                {
                    RoomIndex = 21,
                    VertexChange = new TRVertex(),
                    VertexIndex = vertex,
                    ShadeChange = (short)(8192 - obelisk.Rooms[21].Mesh.Vertices[vertex].Lighting)
                });
            }
        }

        {
            vertices = new()
            {
                obelisk.Rooms[20].Mesh.Rectangles[5].Vertices[1],
                obelisk.Rooms[20].Mesh.Rectangles[5].Vertices[2],
                obelisk.Rooms[20].Mesh.Rectangles[6].Vertices[3],
                obelisk.Rooms[20].Mesh.Rectangles[1].Vertices[0],
                obelisk.Rooms[20].Mesh.Rectangles[1].Vertices[3],
                obelisk.Rooms[20].Mesh.Rectangles[2].Vertices[0],
                obelisk.Rooms[20].Mesh.Rectangles[4].Vertices[0],
                obelisk.Rooms[20].Mesh.Rectangles[3].Vertices[0],
            };

            foreach (ushort vertex in vertices)
            {
                data.RoomEdits.Add(new TRRoomVertexMove
                {
                    RoomIndex = 20,
                    VertexChange = new TRVertex(),
                    VertexIndex = vertex,
                    ShadeChange = (short)(8192 - obelisk.Rooms[20].Mesh.Vertices[vertex].Lighting)
                });
            }
        }
    }

    private static void FixTransparentTextures(TR1Level obelisk, InjectionData data)
    {
        FixTransparentPixels(obelisk, data,
            obelisk.Rooms[31].Mesh.Rectangles[30], Color.FromArgb(188, 140, 64));
        FixTransparentPixels(obelisk, data,
            obelisk.Rooms[37].Mesh.Rectangles[79], Color.FromArgb(188, 140, 64));

        List<ushort> verts = new() { 2, 6, 8, 11 };
        TRMeshFace face = obelisk.StaticMeshes[TR1Type.Furniture1].Mesh
            .TexturedRectangles.Find(t => t.Vertices.All(verts.Contains));

        FixTransparentPixels(obelisk, data, face, Color.FromArgb(200, 144, 88));
    }
}
