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
