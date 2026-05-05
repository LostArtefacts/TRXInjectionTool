using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;
using TRXInjectionTool.Util;

namespace TRXInjectionTool.Types.TR3.Textures;

public class TR3MinesTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        var data = CreateBaseData();
        data.AnimTextureEdits.AddRange(FixCartTracks());
        FixRopes(data);
        FixPushButton(data, TR3LevelNames.RXTECH);

        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.RXTECH}");
        data.RoomEdits.AddRange(CreateFillers(level));
        data.RoomEdits.AddRange(CreateRefacings(level));
        data.RoomEdits.AddRange(CreateRotations());
        return [data];
    }

    private static IEnumerable<TRAnimTextureEdit> FixCartTracks()
    {
        // The PC version has four unused animation ranges for the cart tracks.
        // Replace the first known texture in these with actual placed map textures.
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.RXTECH}");
        var remap = new Dictionary<ushort, ushort>
        {
            [2167] = 1576, // Snow, NW_Clockwise
            [2171] = 1635, // Snow, NE_AntiClockwise
            [2179] = 1945, // Metal, NW_Clockwise
            [2175] = 1900, // Metal, NE_AntiClockwise
        };

        for (int i = 0; i < level.AnimatedTextures.Count; i++)
        {
            var range = level.AnimatedTextures[i];
            for (int j = 0; j < range.Textures.Count; j++)
            {
                if (!remap.TryGetValue(range.Textures[j], out var newTexture))
                {
                    continue;
                }

                range.Textures[j] = newTexture;
                yield return new()
                {
                    Index = i,
                    Textures = range.Textures,
                };
                break;
            }
        }
    }

    private static void FixRopes(InjectionData data)
    {
        // Room 26 can't contain rope statics as it has no flipmap. Add hidden animatings instead
        // and trigger them when the puzzle items are used.
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.RXTECH}");
        var shade = (short)level.Rooms[22].StaticMeshes.First(s => s.ID == TR3Type.SceneryBase + 18).Colour;

        var floors = new List<ushort[]>
        {
            new ushort[] { 4, 7 },
            new ushort[] { 5, 8 },
        };
        var triggers = floors.Select(f => FDBuilder.GetTrigger(level, 32, f[0], f[1])).ToList();

        int y = -7168;
        for (int i = 0; i < 6; i++)
        {
            var room = (short)(i == 0 ? 24 : (y >= -765 ? 26 : 23));
            data.FloorEdits.Add(new()
            {
                RoomIndex = room,
                X = 3,
                Z = 3,
                Fixes = [new FDTrigItem
                {
                    Item = new()
                    {
                        TypeID = (TR1Type)TR3Type.Animating3,
                        Angle = 16384,
                        X = 22016,
                        Y = y,
                        Z = 31232,
                        Room = room,
                        Intensity = shade,
                        Invisible = true,
                    },
                }],
            });

            y += 1793;

            triggers.ForEach(t => t.Actions.Add(new()
            {
                Action = FDTrigAction.Object,
                Parameter = (short)(level.Entities.Count + i),
            }));
        }

        for (int i = 0; i < floors.Count; i++)
        {
            var floor = floors[i];
            var trigger = triggers[i];
            data.FloorEdits.Add(FDBuilder.MakeTrigger(level, 32, floor[0], floor[1], trigger));
        }

        // Hide the old static
        var ropeMesh = level.StaticMeshes[TR3Type.SceneryBase + 18];
        ropeMesh.Visible = false;
        data.StaticMeshEdits.Add(new()
        {
            TypeID = 18,
            Mesh = ropeMesh,
        });

        // Add shorter replacements at the very top and bottom to avoid z-fighting.
        {
            var roomMesh = level.Rooms[32].StaticMeshes.First(s => s.ID == TR3Type.SceneryBase + 18);
            roomMesh.ID = TR3Type.SceneryBase + 36;
            data.RoomEdits.Add(new TRRoomStatic3DCreate
            {
                ID = 36,
                RoomIndex = 32,
                StaticMesh = new()
                {
                    Angle = 16384,
                    X = roomMesh.X,
                    Y = roomMesh.Y,
                    Z = roomMesh.Z,
                    Intensity = (ushort)roomMesh.Colour,
                },
            });
            data.RoomEdits.Add(new TRRoomStatic3DCreate
            {
                ID = 36,
                RoomIndex = 30,
                StaticMesh = new()
                {
                    Angle = 16384,
                    X = 22016,
                    Y = 3590,
                    Z = 31232,
                    Intensity = (ushort)roomMesh.Colour,
                },
            });
        }
    }

    private static IEnumerable<TRRoomTextureEdit> CreateFillers(TR3Level level)
    {
        var room22 = level.Rooms[22];
        {
            var vtx = room22.Mesh.Vertices[room22.Mesh.Rectangles[25].Vertices[1]];
            var vertex = CreateVertex(22, room22, vtx, shift: 0);
            vertex.Vertex.Vertex.Z -= 1024;
            yield return vertex;

            vertex = CreateVertex(22, room22, vtx, shift: 0);
            vertex.Vertex.Vertex.Z -= 1024;
            vertex.Vertex.Vertex.X += 1024;
            yield return vertex;

            vtx = room22.Mesh.Vertices[room22.Mesh.Rectangles[39].Vertices[1]];
            vertex = CreateVertex(22, room22, vtx, shift: 0);
            vertex.Vertex.Vertex.Z -= 1024;
            yield return vertex;
        }

        yield return CreateFace(22, 24, 27, TRMeshFaceType.TexturedQuad,
        [
            room22.Mesh.Rectangles[25].Vertices[2],
            room22.Mesh.Rectangles[25].Vertices[1],
            (ushort)(room22.Mesh.Vertices.Count - 3),
            (ushort)(room22.Mesh.Vertices.Count - 2),
        ]);

        yield return CreateFace(22, 24, 32, TRMeshFaceType.TexturedTriangle,
        [
            (ushort)(room22.Mesh.Vertices.Count - 1),
            room22.Mesh.Triangles[25].Vertices[1],
            room22.Mesh.Triangles[15].Vertices[1],
        ]);

        var room32 = level.Rooms[32];
        yield return CreateFace(32, 32, 81, TRMeshFaceType.TexturedQuad,
        [
            room32.Mesh.Rectangles[60].Vertices[1],
            room32.Mesh.Rectangles[42].Vertices[3],
            room32.Mesh.Rectangles[80].Vertices[2],
            room32.Mesh.Rectangles[60].Vertices[2],
        ]);
        yield return CreateFace(32, 32, 81, TRMeshFaceType.TexturedQuad,
        [
            room32.Mesh.Rectangles[126].Vertices[2],
            room32.Mesh.Rectangles[125].Vertices[2],
            room32.Mesh.Rectangles[128].Vertices[1],
            room32.Mesh.Rectangles[126].Vertices[3],
        ]);

        foreach (var idx in new[] { 32, 44, 62, 82, 105 })
        {
            var face = room32.Mesh.Rectangles[idx];
            yield return CreateFace(32, 32, 89, TRMeshFaceType.TexturedQuad,
            [
                face.Vertices[1],
                face.Vertices[0],
                face.Vertices[3],
                face.Vertices[2],
            ]);
        }

        var room70 = level.Rooms[70];
        yield return CreateFace(70, 70, 0, TRMeshFaceType.TexturedTriangle,
        [
            room70.Mesh.Triangles[0].Vertices[1],
            room70.Mesh.Triangles[0].Vertices[0],
            room70.Mesh.Triangles[0].Vertices[2],
        ]);
        yield return CreateFace(70, 70, 1, TRMeshFaceType.TexturedTriangle,
        [
            room70.Mesh.Triangles[1].Vertices[2],
            room70.Mesh.Triangles[1].Vertices[1],
            room70.Mesh.Triangles[1].Vertices[0],
        ]);

        foreach (var idx in new[] {
            172, 174, 182, 183, 215, 236,
            254, 143, 164, 82, 83, 86,
            59, 62, 65, 68, 71, 74
        })
        {
            var face = room70.Mesh.Rectangles[idx];
            yield return CreateFace(70, 70, (ushort)idx, TRMeshFaceType.TexturedQuad,
            [
                face.Vertices[1],
                face.Vertices[0],
                face.Vertices[3],
                face.Vertices[2],
            ]);
        }

        yield return CreateFace(70, 70, 172, TRMeshFaceType.TexturedQuad,
        [
            room70.Mesh.Rectangles[182].Vertices[0],
            room70.Mesh.Rectangles[182].Vertices[1],
            room70.Mesh.Rectangles[183].Vertices[2],
            room70.Mesh.Rectangles[183].Vertices[3],
        ]);
        yield return CreateFace(70, 70, 173, TRMeshFaceType.TexturedQuad,
        [
            room70.Mesh.Rectangles[173].Vertices[3],
            room70.Mesh.Rectangles[173].Vertices[2],
            room70.Mesh.Rectangles[170].Vertices[1],
            room70.Mesh.Rectangles[170].Vertices[0],
        ]);
        yield return CreateFace(70, 70, 173, TRMeshFaceType.TexturedQuad,
        [
            room70.Mesh.Rectangles[183].Vertices[0],
            room70.Mesh.Rectangles[182].Vertices[3],
            room70.Mesh.Rectangles[182].Vertices[0],
            room70.Mesh.Rectangles[183].Vertices[3],
        ]);
        yield return CreateFace(70, 74, 6, TRMeshFaceType.TexturedQuad,
        [
            room70.Mesh.Rectangles[170].Vertices[1],
            room70.Mesh.Rectangles[173].Vertices[2],
            room70.Mesh.Rectangles[152].Vertices[0],
            room70.Mesh.Rectangles[152].Vertices[1],
        ]);
        yield return CreateFace(70, 70, 221, TRMeshFaceType.TexturedQuad,
        [
            room70.Mesh.Rectangles[221].Vertices[0],
            room70.Mesh.Rectangles[221].Vertices[1],
            room70.Mesh.Rectangles[193].Vertices[1],
            room70.Mesh.Rectangles[193].Vertices[0],
        ]);
        yield return CreateFace(70, 70, 218, TRMeshFaceType.TexturedQuad,
        [
            room70.Mesh.Rectangles[218].Vertices[0],
            room70.Mesh.Rectangles[218].Vertices[1],
            room70.Mesh.Rectangles[191].Vertices[0],
            room70.Mesh.Rectangles[191].Vertices[1],
        ]);
        yield return CreateFace(70, 74, 6, TRMeshFaceType.TexturedQuad,
        [
            room70.Mesh.Rectangles[221].Vertices[3],
            room70.Mesh.Rectangles[221].Vertices[0],
            room70.Mesh.Rectangles[219].Vertices[2],
            room70.Mesh.Rectangles[219].Vertices[3],
        ]);
        yield return CreateFace(70, 74, 6, TRMeshFaceType.TexturedQuad,
        [
            room70.Mesh.Rectangles[218].Vertices[1],
            room70.Mesh.Rectangles[237].Vertices[1],
            room70.Mesh.Rectangles[237].Vertices[2],
            room70.Mesh.Rectangles[218].Vertices[0],
        ]);

        var room69 = level.Rooms[69];
        var vtxPos = new List<ushort>();
        foreach (var idx in new[] { 273, 303, 335 })
        {
            var vtx = room69.Mesh.Vertices[room69.Mesh.Rectangles[idx].Vertices[1]];
            vtxPos.Add((ushort)room69.Mesh.Vertices.Count);
            yield return CreateVertex(69, room69, vtx, shift: -256);
        }

        {
            var vtx = room69.Mesh.Vertices[room69.Mesh.Rectangles[194].Vertices[3]];
            vtxPos.Add((ushort)room69.Mesh.Vertices.Count);
            yield return CreateVertex(69, room69, vtx, shift: -256);

            vtx = room69.Mesh.Vertices[room69.Mesh.Rectangles[175].Vertices[1]];
            vtxPos.Add((ushort)room69.Mesh.Vertices.Count);
            yield return CreateVertex(69, room69, vtx, shift: -256);

            vtx = room69.Mesh.Vertices[room69.Mesh.Rectangles[112].Vertices[1]];
            vtxPos.Add((ushort)room69.Mesh.Vertices.Count);
            yield return CreateVertex(69, room69, vtx, shift: -256);

            vtx = room69.Mesh.Vertices[room69.Mesh.Rectangles[112].Vertices[2]];
            vtxPos.Add((ushort)room69.Mesh.Vertices.Count);
            yield return CreateVertex(69, room69, vtx, shift: -256);
        }

        yield return CreateFace(69, 69, 336, TRMeshFaceType.TexturedQuad,
        [
            room69.Mesh.Rectangles[243].Vertices[1],
            room69.Mesh.Rectangles[250].Vertices[1],
            room69.Mesh.Rectangles[250].Vertices[0],
            vtxPos[0],
        ]);

        yield return CreateFace(69, 69, 276, TRMeshFaceType.TexturedQuad,
        [
            room69.Mesh.Rectangles[276].Vertices[0],
            room69.Mesh.Rectangles[276].Vertices[1],
            room69.Mesh.Rectangles[273].Vertices[1],
            vtxPos[0],
        ]);
        yield return CreateFace(69, 69, 276, TRMeshFaceType.TexturedQuad,
        [
            vtxPos[0],
            room69.Mesh.Rectangles[273].Vertices[1],
            room69.Mesh.Rectangles[273].Vertices[2],
            vtxPos[1],
        ]);
        yield return CreateFace(69, 69, 276, TRMeshFaceType.TexturedQuad,
        [
            vtxPos[1],
            room69.Mesh.Rectangles[303].Vertices[1],
            room69.Mesh.Rectangles[303].Vertices[2],
            vtxPos[2],
        ]);
        yield return CreateFace(69, 69, 107, TRMeshFaceType.TexturedTriangle,
        [
            room69.Mesh.Rectangles[335].Vertices[1],
            room69.Mesh.Rectangles[335].Vertices[2],
            vtxPos[2],
        ]);

        yield return CreateFace(69, 69, 276, TRMeshFaceType.TexturedQuad,
        [
            room69.Mesh.Rectangles[222].Vertices[3],
            room69.Mesh.Rectangles[222].Vertices[2],
            room69.Mesh.Rectangles[194].Vertices[3],
            vtxPos[3],
        ]);
        yield return CreateFace(69, 69, 276, TRMeshFaceType.TexturedQuad,
        [
            vtxPos[3],
            room69.Mesh.Rectangles[194].Vertices[3],
            room69.Mesh.Rectangles[175].Vertices[1],
            vtxPos[4],
        ]);
        yield return CreateFace(69, 69, 276, TRMeshFaceType.TexturedQuad,
        [
            vtxPos[4],
            room69.Mesh.Rectangles[175].Vertices[1],
            room69.Mesh.Rectangles[175].Vertices[2],
            room69.Mesh.Rectangles[178].Vertices[3],
        ]);

        foreach (var idx in new[] {
            216, 195, 178, 148, 144, 140, 102, 106, 79, 81, 83, 86, 89, 129, 126, 123, 120, 116,
        })
        {
            var face = room69.Mesh.Rectangles[idx];
            yield return CreateFace(69, 69, (ushort)idx, TRMeshFaceType.TexturedQuad,
            [
                face.Vertices[1],
                face.Vertices[0],
                face.Vertices[3],
                face.Vertices[2],
            ]);
        }

        yield return CreateFace(69, 69, 276, TRMeshFaceType.TexturedQuad,
        [
            vtxPos[6],
            room69.Mesh.Rectangles[112].Vertices[2],
            room69.Mesh.Rectangles[112].Vertices[1],
            vtxPos[5],
        ]);
        yield return CreateFace(69, 69, 276, TRMeshFaceType.TexturedQuad,
        [
            vtxPos[5],
            room69.Mesh.Rectangles[112].Vertices[1],
            room69.Mesh.Rectangles[104].Vertices[1],
            room69.Mesh.Rectangles[71].Vertices[0],
        ]);
        yield return CreateFace(69, 69, 276, TRMeshFaceType.TexturedQuad,
        [
            room69.Mesh.Rectangles[148].Vertices[3],
            room69.Mesh.Rectangles[148].Vertices[2],
            room69.Mesh.Rectangles[112].Vertices[2],
            vtxPos[6],
        ]);
        yield return CreateFace(69, 69, 276, TRMeshFaceType.TexturedQuad,
        [
            room69.Mesh.Rectangles[65].Vertices[3],
            room69.Mesh.Rectangles[65].Vertices[2],
            room69.Mesh.Rectangles[141].Vertices[1],
            room69.Mesh.Rectangles[141].Vertices[0],
        ]);
        yield return CreateFace(69, 69, 276, TRMeshFaceType.TexturedQuad,
        [
            room69.Mesh.Rectangles[47].Vertices[3],
            room69.Mesh.Rectangles[47].Vertices[2],
            room69.Mesh.Rectangles[117].Vertices[1],
            room69.Mesh.Rectangles[117].Vertices[0],
        ]);

        yield return CreateFace(69, 69, 37, TRMeshFaceType.TexturedTriangle,
        [
            room69.Mesh.Triangles[37].Vertices[1],
            room69.Mesh.Rectangles[89].Vertices[3],
            room69.Mesh.Rectangles[91].Vertices[3],
        ]);
        yield return CreateFace(69, 69, 37, TRMeshFaceType.TexturedTriangle,
        [
            room69.Mesh.Rectangles[129].Vertices[0],
            room69.Mesh.Rectangles[129].Vertices[1],
            room69.Mesh.Rectangles[91].Vertices[2],
        ]);
    }

    private static List<TRRoomTextureReface> CreateRefacings(TR3Level level)
    {
        return
        [
            Reface(level, 2, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedTriangle, 2056, 167),
            Reface(level, 7, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedTriangle, 2056, 54),
            Reface(level, 70, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1695, 152),
            Reface(level, 70, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1856, 216),

            Reface(level, 69, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1578, 17),
            Reface(level, 69, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1578, 44),
            Reface(level, 69, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1578, 74),
            Reface(level, 69, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1578, 96),
            Reface(level, 69, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1578, 112),
            Reface(level, 69, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1578, 151),
            Reface(level, 69, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1578, 180),

            Reface(level, 69, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1578, 218),
            Reface(level, 69, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1578, 240),
            Reface(level, 69, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1578, 271),
            Reface(level, 69, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1578, 298),
            Reface(level, 69, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1578, 332),
        ];
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return
        [
            Rotate(7, TRMeshFaceType.TexturedQuad, 22, 2),
            Rotate(69, TRMeshFaceType.TexturedTriangle, 107, 2),
            Rotate(70, TRMeshFaceType.TexturedTriangle, 0, 2),
            Rotate(70, TRMeshFaceType.TexturedQuad, 216, 1),
        ];
    }

    private static InjectionData CreateBaseData()
    {
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.RXTECH}");
        var rope = level.StaticMeshes[TR3Type.SceneryBase + 18];
        var mesh = rope.Mesh;
        for (int i = 0; i < 3; i++)
        {
            mesh.Vertices[i].X = mesh.Vertices[i + 3].X;
            mesh.Vertices[i].Z = mesh.Vertices[i + 3].Z;
        }

        var model = new TRModel
        {
            Meshes = [mesh],
            MeshTrees = [],
            Animations = [new()
            {
                Accel = new(),
                Speed = new(),
                FrameRate = 1,
                Frames = [new()
                {
                    Rotations = [new()],
                }],
            }],
        };
        level.Models[TR3Type.Animating3] = model;

        foreach (var frame in model.Animations[0].Frames)
        {
            frame.Bounds = TRAnimBoundsCalculator.ComputeFrameBounds(model, frame);
        }

        CreateModelLevel(level, TR3Type.Animating3);

        rope = rope.Clone();
        rope.Mesh = mesh.Clone();
        level.StaticMeshes[TR3Type.SceneryBase + 36] = rope;
        for (int i = 0; i < 3; i++)
        {
            rope.Mesh.Vertices[i].Y = 768;
        }

        var data = InjectionData.Create(level, InjectionType.TextureFix, "mines_textures");
        CreateDefaultTests(data, $"TR3/{TR3LevelNames.RXTECH}");
        return data;
    }
}
