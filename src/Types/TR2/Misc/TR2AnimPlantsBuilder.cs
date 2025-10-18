using System.Drawing;
using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;
using TRXInjectionTool.Util;

namespace TRXInjectionTool.Types.TR2.Misc;

public class TR2AnimPlantsBuilder : InjectionBuilder
{
    private static readonly string _resourceDir = "Resources/TR2/Plants";

    public override string ID => "tr2_anim_plants";

    public override List<InjectionData> Build()
    {
        var plantData = DeserializeFile<PlantData>($"Resources/TR2/plants/data.json");
        var baseLevel = CreateBaseLevel(plantData);
        var random = new Random(21072025);
        return plantData.Defs.Select(data => CreateLevelData(data, baseLevel, random)).ToList();
    }

    private static TR2Level CreateBaseLevel(PlantData data)
    {
        var spriteSequence = new TRSpriteSequence();
        var regions = new List<TRTextileRegion>();

        foreach (var def in data.Sprites)
        {
            var texture = new TRSpriteTexture
            {
                Bounds = new(def.x, def.y, def.w, def.h),
                Alignment = new()
                {
                    Left = def.l,
                    Top = def.t,
                    Right = def.r,
                    Bottom = def.b,
                },
            };

            spriteSequence.Textures.Add(texture);
            regions.Add(new()
            {
                Bounds = texture.Bounds,
                Image = GetImage(def),
                Segments = new()
                {
                    new()
                    {
                        Index = def.mesh_num,
                        Texture = texture,
                    },
                },
            });
        }

        var level = _control2.Read($"Resources/{TR2LevelNames.FATHOMS}");
        var basePalette = level.Palette.Select(c => c.ToTR1Color()).ToList();
        ResetLevel(level, 1);
        var packer = new TR2TexturePacker(level);
        packer.AddRectangles(regions);
        packer.Pack(true);

        GenerateImages8(level, basePalette);

        level.Sprites[TR2Type.Plant2] = spriteSequence;
        return level;
    }

    private static TRImage GetImage(GlyphDef glyph)
    {
        var image = new TRImage(Path.Combine(_resourceDir, glyph.filename));
        var bounds = new Rectangle(glyph.x, glyph.y, glyph.w, glyph.h);
        return image.Export(bounds);
    }

    private static InjectionData CreateLevelData(LevelDef def, TR2Level baseLevel, Random random)
    {
        var level = _control2.Read($"Resources/{def.Level}");
        var data = InjectionData.Create(baseLevel, InjectionType.AlterAnimSprite, $"{_tr2NameMap[def.Level]}_plants");

        var staticID = (TR2Type)((int)TR2Type.SceneryBase + def.StaticType);
        var mesh = level.StaticMeshes[staticID];
        mesh.Visible = false;
        data.StaticMeshEdits.Add(new()
        {
            TypeID = def.StaticType,
            Mesh = mesh,
        });

        var sequence = baseLevel.Sprites[TR2Type.Plant2];
        foreach (var location in def.Locations)
        {
            var room = level.Rooms[location.Room];
            room.Mesh.Vertices.Add(new()
            {
                Lighting = location.Shade,
                Vertex = new()
                {
                    X = (short)(location.X - room.Info.X),
                    Y = (short)location.Y,
                    Z = (short)(location.Z - room.Info.Z),
                },
            });

            data.RoomEdits.Add(new TRRoomVertexCreate
            {
                RoomIndex = location.Room,
                Vertex = new()
                {
                    Lighting = room.Mesh.Vertices[^1].Lighting,
                    Vertex = room.Mesh.Vertices[^1].Vertex,
                },
            });
            data.RoomEdits.Add(new TRRoomSpriteCreate
            {
                RoomIndex = location.Room,
                ID = (int)(TR2Type.Plant2 - TR2Type.SceneryBase),
                Vertex = (ushort)(room.Mesh.Vertices.Count - 1),
                Frame = (ushort)random.Next(sequence.Textures.Count),
            });
        }

        return data;
    }

    private class PlantData
    {
        public List<GlyphDef> Sprites { get; set; }
        public List<LevelDef> Defs { get; set; }
    }

    private class LevelDef
    {
        public string Level { get; set; }
        public int StaticType { get; set; }
        public List<Location> Locations { get; set; }
    }

    private class Location
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public short Room { get; set; }
        public short Shade { get; set; }
    }
}
