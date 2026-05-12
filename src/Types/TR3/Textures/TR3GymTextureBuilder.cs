using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Textures;

public class TR3GymTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        var data = CreateBaseData();
        FixPushButton(data, TR3LevelNames.ASSAULT);

        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.ASSAULT}");
        data.RoomEdits.AddRange(CreateFillers(level));
        data.RoomEdits.AddRange(CreateRefacings(level));
        data.RoomEdits.AddRange(CreateVertexShifts(level));
        data.RoomEdits.AddRange(CreateShifts(level));
        data.RoomEdits.AddRange(CreateRotations());

        FixRoom20(level, data);
        FixKitchenUnits(data);

        return [data];
    }

    private static IEnumerable<TRRoomTextureEdit> CreateFillers(TR3Level level)
    {
        var room96 = level.Rooms[96];
        yield return CreateFace(96, 96, 40, TRMeshFaceType.TexturedQuad,
        [
            room96.Mesh.Rectangles[33].Vertices[1],
            room96.Mesh.Rectangles[33].Vertices[0],
            room96.Mesh.Rectangles[32].Vertices[1],
            room96.Mesh.Rectangles[32].Vertices[0],
        ]);
        yield return CreateFace(96, 96, 40, TRMeshFaceType.TexturedQuad,
        [
            room96.Mesh.Rectangles[37].Vertices[1],
            room96.Mesh.Rectangles[37].Vertices[0],
            room96.Mesh.Rectangles[36].Vertices[0],
            room96.Mesh.Rectangles[36].Vertices[3],
        ]);
        yield return CreateFace(96, 96, 40, TRMeshFaceType.TexturedQuad,
        [
            room96.Mesh.Rectangles[25].Vertices[1],
            room96.Mesh.Rectangles[25].Vertices[0],
            room96.Mesh.Rectangles[24].Vertices[0],
            room96.Mesh.Rectangles[24].Vertices[3],
        ]);
        yield return CreateFace(96, 96, 40, TRMeshFaceType.TexturedQuad,
        [
            room96.Mesh.Rectangles[23].Vertices[1],
            room96.Mesh.Rectangles[23].Vertices[0],
            room96.Mesh.Rectangles[22].Vertices[1],
            room96.Mesh.Rectangles[22].Vertices[0],
        ]);

        var room62 = level.Rooms[62];
        yield return CreateFace(62, 62, 63, TRMeshFaceType.TexturedQuad,
        [
            room62.Mesh.Rectangles[63].Vertices[3],
            room62.Mesh.Rectangles[63].Vertices[2],
            room62.Mesh.Rectangles[109].Vertices[1],
            room62.Mesh.Rectangles[109].Vertices[0],
        ]);
        yield return CreateFace(62, 62, 63, TRMeshFaceType.TexturedQuad,
        [
            room62.Mesh.Rectangles[84].Vertices[3],
            room62.Mesh.Rectangles[84].Vertices[2],
            room62.Mesh.Rectangles[130].Vertices[1],
            room62.Mesh.Rectangles[130].Vertices[0],
        ]);

        var room92 = level.Rooms[92];
        yield return CreateFace(92, 92, 271, TRMeshFaceType.TexturedQuad,
        [
            room92.Mesh.Rectangles[268].Vertices[1],
            room92.Mesh.Triangles[264].Vertices[1],
            room92.Mesh.Rectangles[271].Vertices[1],
            room92.Mesh.Rectangles[271].Vertices[0],
        ]);

        var room9 = level.Rooms[9];
        yield return CreateFace(9, 9, 0, TRMeshFaceType.TexturedTriangle,
        [
            room9.Mesh.Rectangles[254].Vertices[0],
            room9.Mesh.Rectangles[255].Vertices[1],
            room9.Mesh.Rectangles[260].Vertices[1],
        ]);

        var room39 = level.Rooms[39];
        yield return CreateFace(39, 39, 95, TRMeshFaceType.TexturedQuad,
        [
            room39.Mesh.Rectangles[117].Vertices[2],
            room39.Mesh.Rectangles[94].Vertices[0],
            room39.Mesh.Rectangles[94].Vertices[3],
            room39.Mesh.Rectangles[93].Vertices[0],
        ]);
    }

    private static List<TRRoomVertexMove> CreateVertexShifts(TR3Level level)
    {
        return
        [
            new()
            {
                RoomIndex = 92,
                VertexIndex = level.Rooms[92].Mesh.Triangles[25].Vertices[1],
                VertexChange = new() { Y = 512 }
            },
            new()
            {
                RoomIndex = 92,
                VertexIndex = level.Rooms[92].Mesh.Rectangles[271].Vertices[0],
                VertexChange = new() { Y = 256 }
            },
            new()
            {
                RoomIndex = 92,
                VertexIndex = level.Rooms[92].Mesh.Rectangles[271].Vertices[1],
                VertexChange = new() { Y = 768 }
            },
            new()
            {
                RoomIndex = 5,
                VertexIndex = level.Rooms[5].Mesh.Rectangles[146].Vertices[3],
                VertexChange = new() { Y = 256 }
            },
            new()
            {
                RoomIndex = 5,
                VertexIndex = level.Rooms[5].Mesh.Rectangles[149].Vertices[3],
                VertexChange = new() { Y = 256 }
            },
        ];
    }

    private static IEnumerable<TRRoomTextureEdit> CreateShifts(TR3Level level)
    {
        yield return CreateQuadShift(9, 260,
        [
            new()
            {
                Index = 2,
                NewVertexIndex = level.Rooms[9].Mesh.Rectangles[255].Vertices[1],
            },
        ]);
        
        var room45 = level.Rooms[45];
        var vtx = room45.Mesh.Vertices[room45.Mesh.Rectangles[152].Vertices[2]];
        yield return CreateVertex(45, room45, vtx, shift: 256);
        yield return CreateQuadShift(45, 152,
        [
            new()
            {
                Index = 2,
                NewVertexIndex = (ushort)(room45.Mesh.Vertices.Count - 1),
            },
        ]);
        yield return CreateQuadShift(45, 153,
        [
            new()
            {
                Index = 1,
                NewVertexIndex = (ushort)(room45.Mesh.Vertices.Count - 1),
            },
        ]);

        yield return CreateQuadShift(39, 95,
        [
            new()
            {
                Index = 2,
                NewVertexIndex = level.Rooms[39].Mesh.Rectangles[94].Vertices[0],
            },
            new()
            {
                Index = 3,
                NewVertexIndex = level.Rooms[39].Mesh.Rectangles[117].Vertices[2],
            },
        ]);

        var room19 = level.Rooms[19];
        vtx = room19.Mesh.Vertices[room19.Mesh.Rectangles[13].Vertices[3]];
        yield return CreateVertex(19, room19, vtx, shift: -256);
        yield return CreateQuadShift(19, 13,
        [
            new()
            {
                Index = 2,
                NewVertexIndex = room19.Mesh.Rectangles[25].Vertices[3],
            },
            new()
            {
                Index = 3,
                NewVertexIndex = (ushort)(room19.Mesh.Vertices.Count - 1),
            },
        ]);
        yield return CreateFace(19, 50, 138, TRMeshFaceType.TexturedQuad,
        [
            (ushort)(room19.Mesh.Vertices.Count - 1),
            room19.Mesh.Rectangles[25].Vertices[3],
            room19.Mesh.Rectangles[13].Vertices[2],
            room19.Mesh.Rectangles[13].Vertices[3],
        ]);
    }

    private static IEnumerable<TRRoomTextureReface> CreateRefacings(TR3Level level)
    {
        foreach (var face in new[] { 6, 15 })
        {
            yield return Reface(level, 96, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1636, (short)face);
        }

        foreach (var face in new[] { 4, 11, 13, 20, 28, 30, 34 })
        {
            yield return Reface(level, 96, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1637, (short)face);
        }

        foreach (var face in new[] { 0, 4, 8, 13, 17, 21, 26, 29, 32 })
        {
            yield return Reface(level, 58, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1637, (short)face);
        }

        yield return Reface(level, 59, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1637, 26);

        yield return Reface(level, 65, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1504, 14);
        yield return Reface(level, 65, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1468, 15);
        yield return Reface(level, 51, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1405, 8);

        yield return Reface(level, 20, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedQuad, 1540, 3);
        yield return Reface(level, 54, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedQuad, 1427, 0);
        yield return Reface(level, 54, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedTriangle, 1411, 1);

        foreach (var face in new[] { 146, 149 })
        {
            yield return Reface(level, 5, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1417, (short)face);
        }

        yield return Reface(level, 19, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1417, 143);
        yield return Reface(level, 32, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1752, 125);

        yield return Reface(level, 3, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1418, 210);
        yield return Reface(level, 3, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1418, 212);
    }

    private static IEnumerable<TRRoomTextureRotate> CreateRotations()
    {
        foreach (var face in new[] { 4, 20, 28, 30, 34 })
        {
            yield return Rotate(96, TRMeshFaceType.TexturedQuad, (short)face, 1);
        }

        foreach (var face in new[] { 11, 13, 24, 36 })
        {
            yield return Rotate(96, TRMeshFaceType.TexturedQuad, (short)face, 3);
        }

        foreach (var face in new[] { 0, 4, 8, 13, 17, 21, 26, 29, 32 })
        {
            yield return Rotate(58, TRMeshFaceType.TexturedQuad, (short)face, 3);
        }
        yield return Rotate(59, TRMeshFaceType.TexturedQuad, 26, 3);

        yield return Rotate(96, TRMeshFaceType.TexturedQuad, 40, 2);
        yield return Rotate(57, TRMeshFaceType.TexturedQuad, 15, 3);

        yield return Rotate(36, TRMeshFaceType.TexturedQuad, 38, 3);
        foreach (var face in new[] { 31, 46, 62, 78, 77 })
        {
            yield return Rotate(36, TRMeshFaceType.TexturedQuad, (short)face, 2);
        }

        yield return Rotate(61, TRMeshFaceType.TexturedQuad, 3, 3);

        yield return Rotate(54, TRMeshFaceType.TexturedTriangle, 1, 2);

        foreach (var face in new[] { 44, 91 })
        {
            yield return Rotate(16, TRMeshFaceType.TexturedQuad, (short)face, 3);
        }

        foreach (var face in new[] { 146, 149 })
        {
            yield return Rotate(5, TRMeshFaceType.TexturedQuad, (short)face, 2);
        }

        foreach (var face in new[] { 15, 31, 42 })
        {
            yield return Rotate(8, TRMeshFaceType.TexturedQuad, (short)face, 2);
        }
    }

    private static void FixKitchenUnits(InjectionData data)
    {
        {
            var edit = new TRMeshEdit
            {
                EnforcedType = TRObjectType.Static3D,
                ModelID = (uint)(TR3Type.SceneryBase + 40),
            };
            data.MeshEdits.Add(edit);

            edit.VertexEdits.AddRange(new[] { 0, 1, 10, 12, 13, 22, 24 }.Select(i => new TRVertexEdit
            {
                Index = (short)i,
                Change = new() { Z = 13 },
            }));
            edit.VertexEdits.AddRange(new[] { 4, 6, 11, 14, 15, 17 }.Select(i => new TRVertexEdit
            {
                Index = (short)i,
                Change = new() { X = 13 },
            }));

            edit.VertexEdits.AddRange(new[] { 0, 1, 5, 16, 22, 23, 24 }.Select(i => new TRVertexEdit
            {
                Index = (short)i,
                Change = new() { X = -8 },
            }));

            edit.VertexEdits.Add(new()
            {
                Index = 5,
                Change = new() { X = -3 },
            });
            edit.VertexEdits.Add(new()
            {
                Index = 23,
                Change = new() { X = -1 },
            });
            edit.VertexEdits.Add(new()
            {
                Index = 16,
                Change = new() { Z = -1 },
            });

            edit.VertexEdits.AddRange(new[] { 5, 16, 23, 6, 17, 4, 15 }.Select(i => new TRVertexEdit
            {
                Index = (short)i,
                Change = new() { Z = -14 },
            }));
        }

        {
            var edit = new TRMeshEdit
            {
                EnforcedType = TRObjectType.Static3D,
                ModelID = (uint)(TR3Type.SceneryBase + 42),
            };
            data.MeshEdits.Add(edit);

            edit.VertexEdits.AddRange(new[] { 0, 1, 4, 5, 12, 14 }.Select(i => new TRVertexEdit
            {
                Index = (short)i,
                Change = new() { X = -3, Z = -3 },
            }));
            edit.VertexEdits.AddRange(new[] { 2, 3, 13 }.Select(i => new TRVertexEdit
            {
                Index = (short)i,
                Change = new() { X = -3 },
            }));
            edit.VertexEdits.AddRange(new[] { 12, 13, 14 }.Select(i => new TRVertexEdit
            {
                Index = (short)i,
                Change = new() { X = 1 },
            }));
        }

        {
            var edit = new TRMeshEdit
            {
                EnforcedType = TRObjectType.Static3D,
                ModelID = (uint)(TR3Type.SceneryBase + 44),
            };
            data.MeshEdits.Add(edit);

            edit.VertexEdits.AddRange(new[] { 0, 2, 4, 6, 8, 10, 12 }.Select(i => new TRVertexEdit
            {
                Index = (short)i,
                Change = new() { Z = -14 },
            }));
            edit.VertexEdits.AddRange(new[] { 0, 1, 4, 5, 14, 16, 18 }.Select(i => new TRVertexEdit
            {
                Index = (short)i,
                Change = new() { X = -19 },
            }));
            edit.VertexEdits.AddRange(new[] { 25, 26, 27 }.Select(i => new TRVertexEdit
            {
                Index = (short)i,
                Change = new() { X = -26 },
            }));
        }

        foreach (var id in new[] { 41, 47 })
        {
            var edit = new TRMeshEdit
            {
                EnforcedType = TRObjectType.Static3D,
                ModelID = (uint)(TR3Type.SceneryBase) + (uint)id,
            };
            data.MeshEdits.Add(edit);

            edit.VertexEdits.AddRange(new[] { 2, 3, 4, 5, 11, 12 }.Select(i => new TRVertexEdit
            {
                Index = (short)i,
                Change = new() { Y = 1 },
            }));
        }

        {
            var edit = new TRMeshEdit
            {
                EnforcedType = TRObjectType.Static3D,
                ModelID = (uint)(TR3Type.SceneryBase + 18),
            };
            data.MeshEdits.Add(edit);

            edit.VertexEdits.AddRange(new[] { 0, 1, 4, 5, 6, 8 }.Select(i => new TRVertexEdit
            {
                Index = (short)i,
                Change = new() { Z = -10 },
            }));
            edit.VertexEdits.AddRange(new[] { 0, 2, 4 }.Select(i => new TRVertexEdit
            {
                Index = (short)i,
                Change = new() { X = 12 },
            }));
        }

        {
            var edit = new TRMeshEdit
            {
                EnforcedType = TRObjectType.Static3D,
                ModelID = (uint)(TR3Type.SceneryBase + 43),
            };
            data.MeshEdits.Add(edit);

            edit.VertexEdits.AddRange(new[] { 0, 1, 4, 5, 14, 16 }.Select(i => new TRVertexEdit
            {
                Index = (short)i,
                Change = new() { Z = (short)((i == 1 || i == 5) ? -12 : -10) },
            }));
            edit.VertexEdits.AddRange(new[] { 1, 3, 5 }.Select(i => new TRVertexEdit
            {
                Index = (short)i,
                Change = new() { X = -11 },
            }));
            edit.VertexEdits.AddRange(new[] { 14, 15, 16 }.Select(i => new TRVertexEdit
            {
                Index = (short)i,
                Change = new() { X = 25 },
            }));
        }
    }

    private static void FixGates(TRModel model)
    {
        var mesh = model.Meshes[0];
        var verts = new List<List<ushort>>
        {
            new() { 0, 3, 4, 7, 8, 11 },
            new() { 1, 2, 5, 6, 9, 10 },
        };

        short shift = -29;
        foreach (var group in verts)
        {
            foreach (var v in group)
            {
                var vtx = mesh.Vertices[v].Clone();
                vtx.X += shift;
                mesh.Vertices.Add(vtx);
                mesh.Normals.Add(mesh.Normals[v].Clone());
            }
            shift *= -1;
        }

        verts =
        [
            [17, 15, 13, 16],
            [15, 14, 12, 13],
            [21, 23, 22, 19],
            [20, 21, 19, 18],
        ];
        mesh.TexturedRectangles.AddRange(verts.Select(v => new TRMeshFace
        {
            Type = TRFaceType.Rectangle,
            Texture = mesh.TexturedRectangles[7].Texture,
            Vertices = v,
        }));

        verts =
        [
            [8, 9, 2, 3],
            [3, 2, 1, 0],
            [10, 11, 6, 7],
            [6, 7, 5, 4],
        ];
        foreach (var group in verts)
        {
            var face = mesh.TexturedRectangles.First(f => f.Vertices.All(group.Contains));
            face.DoubleSided = true;
        }
    }

    private static void FixRoom20(TR3Level level, InjectionData data)
    {
        var sector = level.Rooms[20].GetSector(1, 7, TRUnit.Sector);
        sector.Floor = level.Rooms[20].GetSector(1, 12, TRUnit.Sector).Floor;
        sector.FDIndex = 0;
        data.FloorEdits.Add(new()
        {
            RoomIndex = 20,
            X = 1,
            Z = 7,
            Fixes = [new FDSectorOverwrite { Sector = TRRoomSectorExt.CloneFrom(sector) }],
        });
        data.FloorEdits.Add(FDBuilder.MakeSlant(level, 20, 1, 7, -1, 1));

        var room20 = level.Rooms[20];
        var vtx = room20.Mesh.Vertices[room20.Mesh.Triangles[0].Vertices[1]];
        data.RoomEdits.Add(CreateVertex(20, room20, vtx, shift: -256));
        vtx = room20.Mesh.Vertices[room20.Mesh.Triangles[2].Vertices[1]];
        data.RoomEdits.Add(CreateVertex(20, room20, vtx, shift: -256));
        vtx = room20.Mesh.Vertices[room20.Mesh.Triangles[1].Vertices[1]];
        data.RoomEdits.Add(CreateVertex(20, room20, vtx, shift: 768));

        data.RoomEdits.Add(CreateTriShift(20, 0,
        [
            new()
            {
                Index = 1,
                NewVertexIndex = (ushort)(room20.Mesh.Vertices.Count - 3)
            },
        ]));
        data.RoomEdits.Add(CreateTriShift(20, 2,
        [
            new()
            {
                Index = 2,
                NewVertexIndex = (ushort)(room20.Mesh.Vertices.Count - 3)
            },
        ]));

        data.RoomEdits.Add(CreateTriShift(20, 0,
        [
            new()
            {
                Index = 2,
                NewVertexIndex = (ushort)(room20.Mesh.Vertices.Count - 2)
            },
        ]));
        data.RoomEdits.Add(CreateTriShift(20, 2,
        [
            new()
            {
                Index = 1,
                NewVertexIndex = (ushort)(room20.Mesh.Vertices.Count - 2)
            },
        ]));

        data.RoomEdits.Add(CreateTriShift(20, 2,
        [
            new()
            {
                Index = 0,
                NewVertexIndex = (ushort)(room20.Mesh.Vertices.Count - 1)
            },
        ]));

        data.RoomEdits.Add(CreateTriShift(20, 1,
        [
            new()
            {
                Index = 0,
                NewVertexIndex = (ushort)(room20.Mesh.Vertices.Count - 3)
            },
            new()
            {
                Index = 1,
                NewVertexIndex = room20.Mesh.Triangles[0].Vertices[0],
            },
        ]));
        data.RoomEdits.Add(CreateTriShift(20, 3,
        [
            new()
            {
                Index = 0,
                NewVertexIndex = (ushort)(room20.Mesh.Vertices.Count - 2)
            },
            new()
            {
                Index = 2,
                NewVertexIndex = room20.Mesh.Triangles[0].Vertices[0],
            },
        ]));

        data.RoomEdits.Add(Rotate(20, TRMeshFaceType.TexturedTriangle, 3, 1));
        data.RoomEdits.Add(Rotate(20, TRMeshFaceType.TexturedTriangle, 4, 2));
        data.RoomEdits.Add(Rotate(20, TRMeshFaceType.TexturedQuad, 61, 3));
        data.RoomEdits.Add(Rotate(20, TRMeshFaceType.TexturedQuad, 62, 3));

        data.RoomEdits.Add(CreateFace(20, 20, 12, TRMeshFaceType.TexturedQuad,
        [
            room20.Mesh.Rectangles[12].Vertices[1],
            room20.Mesh.Rectangles[16].Vertices[2],
            (ushort)(room20.Mesh.Vertices.Count - 1),
            (ushort)(room20.Mesh.Vertices.Count - 3),
        ]));
        data.RoomEdits.Add(CreateFace(20, 20, 12, TRMeshFaceType.TexturedQuad,
        [
            room20.Mesh.Rectangles[16].Vertices[2],
            room20.Mesh.Rectangles[18].Vertices[2],
            (ushort)(room20.Mesh.Vertices.Count - 2),
            (ushort)(room20.Mesh.Vertices.Count - 1),
        ]));

        data.RoomEdits.Add(Reface(level, 20, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedTriangle, 1534, 1));
        data.RoomEdits.Add(Reface(level, 20, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedTriangle, 1534, 3));
    }

    private static InjectionData CreateBaseData()
    {
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.ASSAULT}");
        CreateModelLevel(level, TR3Type.Winston, TR3Type.Door8);
        level.SoundEffects.Clear();

        FixWinstonNose(level.Models[TR3Type.Winston]);
        FixCatStatue(level, TR3LevelNames.ASSAULT);
        FixGates(level.Models[TR3Type.Door8]);

        var data = InjectionData.Create(level, InjectionType.TextureFix, "gym_textures");
        CreateDefaultTests(data, $"TR3/{TR3LevelNames.ASSAULT}");

        data.SetMeshOnlyModel((uint)TR3Type.Winston);
        data.SetMeshOnlyModel((uint)TR3Type.Door8);

        return data;
    }
}
