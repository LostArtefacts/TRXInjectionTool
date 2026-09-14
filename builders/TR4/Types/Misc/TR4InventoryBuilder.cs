using TRImageControl;
using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;
using TRXInjectionTool.Types.TR3.Misc;
using TRXInjectionTool.Util;

namespace TRXInjectionTool.Types.TR4.Misc;

public class TR4InventoryBuilder : InjectionBuilder, IPublisher
{
    private static readonly Dictionary<TR3Type, TR4Type> _typeMap = new()
    {
        [TR3Type.PassportOpening_H] = (TR4Type)536,
        [TR3Type.PassportClosed_H] = (TR4Type)537,
        [TR3Type.Sunglasses_M_H] = (TR4Type)538,
        [TR3Type.CDPlayer_M_H] = (TR4Type)539,
        [TR3Type.DirectionKeys_M_H] = (TR4Type)540,
        [TR3Type.Map_H] = (TR4Type)541,
    };

    public override List<InjectionData> Build()
    {
        var level = CreateLevel();
        var data = InjectionData.Create(level, InjectionType.General, "inventory_models");

        var jungle = _control3.Read($"Resources/TR3/{TR3LevelNames.JUNGLE}");
        data.SFX.Add(TRSFXData.Create(TR4SFX.Unused05, jungle.SoundEffects[TR3SFX.MenuPassport]));
        return [data];
    }

    private static TR4Level CreateLevel()
    {
        var baseLevel = CreateBaseLevel();
        var level = _control4.Read($"Resources/TR4/{TR4LevelNames.SETH}");
        ResetLevel(level);

        level.ObjectTextures.AddRange(baseLevel.ObjectTextures);
        foreach (var (tr3Type, tr4Type) in _typeMap)
        {
            level.Models[tr4Type] = baseLevel.Models[tr3Type];
        }

        var images = baseLevel.Images16.Select(i => new TRImage(i.Pixels));
        level.Images.Objects.Images16.AddRange(images.Select(i => new TRTexImage16 { Pixels = i.ToRGB555() }));
        level.Images.Objects.Images32.AddRange(images.Select(i => new TRTexImage32 { Pixels = i.ToRGB32() }));

        return level;
    }

    private static TR3Level CreateBaseLevel()
    {
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.JUNGLE}");
        ApplyFixes(level);
        ImportPDA(level);
        TRFaceConverter.ConvertFlatFaces(level, [.. level.Palette16.Select(c => c.ToColor())]);
        CreateModelLevel(level, [.. _typeMap.Keys]);
        return level;
    }

    private static void ApplyFixes(TR3Level level)
    {
        TR3PickupBuilder.FixCDPlayer(level);

        // Essentially InjectionBuilder.FixPassport, but applied directly
        var map = new Dictionary<TR3Type, short>
        {
            [TR3Type.PassportClosed_H] = -2,
            [TR3Type.PassportOpening_H] = -1,
        };
        foreach (var (type, shift) in map)
        {
            var mesh = level.Models[type].Meshes[0];
            foreach (var vtx in new[] { 1, 2, 5, 6 })
            {
                mesh.Vertices[vtx].X += shift;
            }
        }
    }

    private static void ImportPDA(TR3Level level)
    {
        var pdaLevel = CreatePDALevel();
        level.Models[TR3Type.Map_H] = pdaLevel.Models[TR1Type.Map_M_U];
        var texInfos = pdaLevel.ObjectTextures.ToList();

        level.Models[TR3Type.Map_H].Meshes.SelectMany(m => m.TexturedFaces)
            .ToList().ForEach(f => f.Texture += (ushort)level.ObjectTextures.Count);

        level.ObjectTextures.AddRange(texInfos);
        texInfos.ForEach(o => o.Atlas += (ushort)level.Images16.Count);

        var tile = new TRImage(TRConsts.TPageWidth, TRConsts.TPageHeight);
        tile.Import(new("Resources/TR4/pda.png"), new(0, 0));
        level.Images16.Add(new() { Pixels = tile.ToRGB555() });
        level.Images8.Add(new());
    }

    public TRLevelBase Publish()
        => CreateLevel();

    public string GetPublishedName()
        => "inventory.tr4";
}
