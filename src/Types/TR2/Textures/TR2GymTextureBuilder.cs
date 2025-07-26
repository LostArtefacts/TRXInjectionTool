using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;
using TRXInjectionTool.Util;

namespace TRXInjectionTool.Types.TR2.Textures;

public class TR2GymTextureBuilder : TextureBuilder
{
    public override string ID => "gym_textures";

    public override List<InjectionData> Build()
    {
        InjectionData data = CreateBaseData();

        TR2Level gym = _control2.Read($"Resources/{TR2LevelNames.ASSAULT}");
        FixLaraTransparency(gym, data);

        data.RoomEdits.AddRange(CreateVertexShifts(gym));
        data.RoomEdits.AddRange(CreateShifts(gym));
        data.RoomEdits.AddRange(CreateRefacings(gym));
        data.RoomEdits.AddRange(CreateRotations());
        ReplaceGoldIdol(data, gym);
        FixPassport(gym, data);

        return new() { data };
    }

    private static List<TRRoomVertexMove> CreateVertexShifts(TR2Level level)
    {
        return new()
        {
            new()
            {
                RoomIndex = 65,
                VertexIndex = level.Rooms[65].Mesh.Rectangles[14].Vertices[0],
                VertexChange = new() { Y = -256 },
            },
            new()
            {
                RoomIndex = 65,
                VertexIndex = level.Rooms[65].Mesh.Rectangles[14].Vertices[1],
                VertexChange = new() { Y = -256 },
            },
        };
    }

    private static List<TRRoomTextureMove> CreateShifts(TR2Level level)
    {
        return new()
        {
            new TRRoomTextureMove
            {
                RoomIndex = 65,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 15,
                VertexRemap = new()
                {
                    new()
                    {
                        Index = 0,
                        NewVertexIndex = level.Rooms[65].Mesh.Rectangles[15].Vertices[1],
                    },
                    new()
                    {
                        Index = 1,
                        NewVertexIndex = level.Rooms[65].Mesh.Rectangles[15].Vertices[0],
                    },
                    new()
                    {
                        Index = 2,
                        NewVertexIndex = level.Rooms[65].Mesh.Rectangles[15].Vertices[3],
                    },
                    new()
                    {
                        Index = 3,
                        NewVertexIndex = level.Rooms[65].Mesh.Rectangles[15].Vertices[3],
                    },
                }
            },
        };
    }

    private static List<TRRoomTextureReface> CreateRefacings(TR2Level level)
    {
        return new()
        {
            Reface(level, 9, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedQuad, 867, 0),
            Reface(level, 65, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 838, 14),
        };
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return new()
        {
            Rotate(36, TRMeshFaceType.TexturedQuad, 38, 3),
            Rotate(57, TRMeshFaceType.TexturedQuad, 15, 3),
        };
    }

    private static void ReplaceGoldIdol(InjectionData data, TR2Level level)
    {
        var room = level.Rooms[53];
        var vertIndex = (ushort)room.Mesh.Sprites[0].Vertex;
        var vert = room.Mesh.Vertices[vertIndex];

        data.RoomEdits.Add(new TRRoomVertexMove
        {
            RoomIndex = 53,
            VertexIndex = vertIndex,
            VertexChange = new() { X = 1024 },
            ShadeChange = (short)(16384 - vert.Lighting),
        });

        data.RoomEdits.Add(new TRRoomStatic3DCreate
        {
            ID = 24,
            RoomIndex = 53,
            StaticMesh = new()
            {
                X = room.Info.X + vert.Vertex.X,
                Y = vert.Vertex.Y,
                Z = room.Info.Z + vert.Vertex.Z,
                Angle = 19456,
                Intensity = 8100,
            }
        });
    }

    private InjectionData CreateBaseData()
    {
        TR2Level level = CreateWinstonLevel(TR2LevelNames.ASSAULT);
        // Current injection limitation, do not replace SFX
        level.SoundEffects.Clear();

        FixHomeWindows(level, TR2LevelNames.ASSAULT);
        FixHomeStatues(level);
        ImportGoldIdol(level);

        InjectionData data = InjectionData.Create(level, InjectionType.TextureFix, ID);
        CreateDefaultTests(data, TR2LevelNames.ASSAULT);
        return data;
    }

    private static void ImportGoldIdol(TR2Level level)
    {
        var vilcabamba = _control1.Read($"Resources/{TR1LevelNames.VILCABAMBA}");
        var idolMesh = vilcabamba.Models[TR1Type.Puzzle1_M_H].Meshes[0];
        idolMesh.Scale(1.5f);
        var bounds = idolMesh.GetBounds();
        idolMesh.Vertices.ForEach(v => v.Y -= (short)(Math.Abs(bounds.MaxY - bounds.MinY) / 2));
        idolMesh.Normals = null;
        idolMesh.Lights = Enumerable.Repeat((short)4096, idolMesh.Vertices.Count).ToList();

        var idolStatic = new TRStaticMesh
        {
            Mesh = idolMesh,
            CollisionBox = idolMesh.GetBounds(),
            VisibilityBox = idolMesh.GetBounds(),
            Visible = true,
        };

        var packer1 = new TR1TexturePacker(vilcabamba);
        var regions = packer1.GetMeshRegions(new[] { idolMesh })
            .Values.SelectMany(v => v).ToList();
        var originalInfos = vilcabamba.ObjectTextures.ToList();

        var packer2 = new TR2TexturePacker(level);
        packer2.AddRectangles(regions);
        packer2.Pack(true);
        GenerateImages8(level, level.Palette.Select(c => c.ToTR1Color()).ToList());

        level.StaticMeshes[TR2Type.SceneryBase + 24] = idolStatic;
        level.ObjectTextures.AddRange(regions.SelectMany(r => r.Segments.Select(s => s.Texture as TRObjectTexture)));

        idolMesh.TexturedFaces.ToList()
            .ForEach(f =>
            {
                f.Texture = (ushort)level.ObjectTextures.IndexOf(originalInfos[f.Texture]);
            });
    }
}
