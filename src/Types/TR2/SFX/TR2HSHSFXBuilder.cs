using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;
using TRXInjectionTool.Util;

namespace TRXInjectionTool.Types.TR2.SFX;

public class TR2HSHSFXBuilder : InjectionBuilder
{
    public override string ID => "house_sfx";

    public override List<InjectionData> Build()
    {
        TR2Level wall = _control2.Read($"Resources/{TR2LevelNames.GW}");
        TR2SFX[] soundIDs = new[]
        {
            TR2SFX.DeathSlideGrab,
            TR2SFX.DeathSlideGo,
            TR2SFX.DeathSlideStop,
        };

        TRDictionary<TR2SFX, TR2SoundEffect> copiedSounds = new();
        soundIDs.ToList().ForEach(s => copiedSounds[s] = wall.SoundEffects[s]);

        var barkhang = _control2.Read($"Resources/{TR2LevelNames.MONASTERY}");
        copiedSounds[TR2SFX.DoorCreak] = barkhang.SoundEffects[TR2SFX.DoorCreak];
        copiedSounds[TR2SFX.Breaking3] = barkhang.SoundEffects[TR2SFX.Breaking3];

        ResetLevel(wall, 1);
        wall.SoundEffects = copiedSounds;
        ImportDoorMesh(wall);

        InjectionData data = InjectionData.Create(wall, InjectionType.General, ID);

        // To avoid hearing the door in Lara's bedroom/kitchen opening when the cutscene
        // starts, replace them with static meshes.
        data.ItemPosEdits.Add(new()
        {
            Index = 11,
            Item = new()
            {
                X = 33280,
                Y = -3840,
                Z = 74240,
                Room = 31,
                Intensity = -1,
            },
        });
        data.ItemPosEdits.Add(new()
        {
            Index = 55,
            Item = new()
            {
                X = 34304,
                Y = -3840,
                Z = 74240,
                Room = 31,
                Intensity = -1,
            },
        });
        data.RoomEdits.Add(new TRRoomStatic3DCreate
        {
            ID = 25,
            RoomIndex = 52,
            StaticMesh = new()
            {
                X = 38400,
                Z = 65024,
                Angle = -16384,
                Intensity = 6000,
            },
        });
        data.RoomEdits.Add(new TRRoomStatic3DCreate
        {
            ID = 25,
            RoomIndex = 64,
            StaticMesh = new()
            {
                X = 29184,
                Y = 2560,
                Z = 65024,
                Angle = 16384,
                Intensity = 7096,
            },
        });

        return new() { data };
    }

    private static void ImportDoorMesh(TR2Level level)
    {
        var hsh = _control2.Read($"Resources/{TR2LevelNames.HOME}");
        var doorMesh = hsh.Models[TR2Type.Door3].Meshes[0];
        var bounds = doorMesh.GetBounds();
        doorMesh.Vertices.ForEach(v =>
        {
            v.X -= (short)(Math.Abs(bounds.MaxX - bounds.MinX) / 2);
            v.Y -= (short)(Math.Abs(bounds.MaxY - bounds.MinY) / 2);
            v.Z -= 514;
            v.X -= 41;
        });
        doorMesh.Normals = null;
        doorMesh.Lights = Enumerable.Repeat((short)5376, doorMesh.Vertices.Count).ToList();
        foreach (var i in new[] { 1, 2, 5, 6 })
        {
            doorMesh.Lights[i] += 320;
        }
        var doorStatic = new TRStaticMesh
        {
            Mesh = doorMesh,
            CollisionBox = doorMesh.GetBounds(),
            VisibilityBox = doorMesh.GetBounds(),
            Visible = true,
        };

        var packer = new TR2TexturePacker(hsh);
        var regions = packer.GetMeshRegions(new[] { doorMesh })
            .Values.SelectMany(v => v);
        var originalInfos = hsh.ObjectTextures.ToList();

        packer = new(level);
        packer.AddRectangles(regions);
        packer.Pack(true);

        level.StaticMeshes[TR2Type.SceneryBase + 25] = doorStatic;
        level.ObjectTextures.AddRange(regions.SelectMany(r => r.Segments.Select(s => s.Texture as TRObjectTexture)));
        doorMesh.TexturedFaces.ToList()
            .ForEach(f =>
            {
                f.Texture = (ushort)level.ObjectTextures.IndexOf(originalInfos[f.Texture]);
            });

        GenerateImages8(level, level.Palette.Select(c => c.ToTR1Color()).ToList());
    }
}
