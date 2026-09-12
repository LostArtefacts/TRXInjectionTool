using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Lara;

public class TR2CutsceneBuilder : InjectionBuilder
{
    public override string ID => "tr2_cutscenes";

    public override List<InjectionData> Build()
    {
        return
        [
            CreateCut2Data(),
            CreateCut3Data(),
            CreateCut3GunFlashData(),
            CreateCut4Data(),
        ];
    }

    private static InjectionData CreateCut2Data()
    {
        var cut = _control2.Read($"Resources/{TR2LevelNames.OPERA_CUT}");
        var models = new TRDictionary<TR2Type, TRModel>
        {
            [TR2Type.CutsceneActor5] = cut.Models[TR2Type.CutsceneActor5], // Cockpit
            [TR2Type.CutsceneActor7] = cut.Models[TR2Type.CutsceneActor7], // Rail hook
            [TR2Type.CutsceneActor8] = cut.Models[TR2Type.CutsceneActor8], // Bartoli
        };
        ResetLevel(cut);
        cut.Models = models;

        foreach (var type in new[] { TR2Type.CutsceneActor5, TR2Type.CutsceneActor7 })
        {
            models[type].Animations[0].Commands.Add(new TRFXCommand
            {
                EffectID = (short)TR2FX.ShadowOff,
                FrameNumber = 1,
            });
        }

        {
            // Fix frozen Bartoli remaining on-screen at the end of his animation set
            var bartoliAnim = models[TR2Type.CutsceneActor8].Animations[^1];
            const short shift = 20;
            for (int i = 99; i < bartoliAnim.Frames.Count; i++)
            {
                bartoliAnim.Frames[i].OffsetZ = bartoliAnim.Frames[i - 1].OffsetZ;
                bartoliAnim.Frames[i].Bounds = bartoliAnim.Frames[i - 1].Bounds.Clone();
                if (i < 104)
                {
                    bartoliAnim.Frames[i].OffsetZ += shift;
                    bartoliAnim.Frames[i].Bounds.MinZ += shift;
                    bartoliAnim.Frames[i].Bounds.MaxZ += shift;
                }
            }
        }

        return InjectionData.Create(cut, InjectionType.General, "cut2_setup", true);
    }

    private static InjectionData CreateCut3Data()
    {
        var cut = _control2.Read($"Resources/{TR2LevelNames.DA_CUT}");
        var lara = cut.Models[TR2Type.Lara];
        var monk = cut.Models[TR2Type.CutsceneActor4];
        ResetLevel(cut);
        cut.Models[TR2Type.Lara] = lara;
        cut.Models[TR2Type.CutsceneActor4] = monk;

        var endAnim = lara.Animations[^1];
        endAnim.NextFrame = (ushort)endAnim.FrameEnd;

        // Remove mesh swap commands, now handled in Lua
        lara.Animations[0].Commands.Clear();
        lara.Animations[7].Commands.Clear();

        FixMonkDeath(monk);

        return InjectionData.Create(cut, InjectionType.General, "cut3_setup", true);
    }

    // The scene carries neither the gun flash object nor the glow drawn over
    // it, so a flash on one of its actors has no mesh, no sprite and no
    // texture. All three come from the Diving Area itself, which is the level
    // the scene breaks away from.
    private static InjectionData CreateCut3GunFlashData()
    {
        var level = _control2.Read($"Resources/{TR2LevelNames.DA}");
        var flash = level.Models[TR2Type.Gunflare_H];
        var glow = level.Sprites[TR2Type.Glow_S_H];

        var packer = new TR2TexturePacker(level);
        var meshRegions = packer.GetMeshRegions(flash.Meshes)
            .Values.SelectMany(v => v).ToList();
        var spriteRegions = packer.GetSpriteRegions(glow)
            .Values.SelectMany(v => v).ToList();
        var originalInfos = level.ObjectTextures.ToList();
        var basePalette = level.Palette.Select(c => c.ToTR1Color()).ToList();

        ResetLevel(level, 1);

        packer = new(level);
        packer.AddRectangles(meshRegions);
        packer.AddRectangles(spriteRegions);
        packer.Pack(true);

        level.Models[TR2Type.Gunflare_H] = flash;
        level.Sprites[TR2Type.Glow_S_H] = glow;
        level.ObjectTextures.AddRange(meshRegions
            .SelectMany(r => r.Segments.Select(s => s.Texture as TRObjectTexture)));
        flash.Meshes
            .SelectMany(m => m.TexturedFaces)
            .Distinct()
            .ToList()
            .ForEach(f =>
            {
                f.Texture = (ushort)level.ObjectTextures.IndexOf(originalInfos[f.Texture]);
            });

        GenerateImages8(level, basePalette);

        return InjectionData.Create(level, InjectionType.General, "cut3_gunflash");
    }

    private static void FixMonkDeath(TRModel model)
    {
        var level = _control2.Read($"Resources/TR2/Objects/monk_cut3.tr2");
        var deathPoseFrame = level.Models[TR2Type.CutsceneActor4].Animations[0].Frames[0];

        var finalCoreFrame = model.Animations[9].Frames[^1];
        model.Animations[10].Frames[0] = finalCoreFrame;
        
        for (int i = 1; i < model.Animations[10].Frames.Count; i++)
        {
            model.Animations[10].Frames [i] = deathPoseFrame;
        }
        for (int anim = 11; anim < model.Animations.Count; anim++)
        {
            for (int i = 0; i < model.Animations[anim].Frames.Count; i++)
            {
                model.Animations[anim].Frames[i] = deathPoseFrame;
            }
        }
    }

    private static InjectionData CreateCut4Data()
    {
        var cut = _control2.Read($"Resources/{TR2LevelNames.XIAN_CUT}");
        var models = new TRDictionary<TR2Type, TRModel>
        {
            [TR2Type.CutsceneActor5] = cut.Models[TR2Type.CutsceneActor5], // Bartoli
            [TR2Type.CutsceneActor6] = cut.Models[TR2Type.CutsceneActor6], // Goon
            [TR2Type.CutsceneActor8] = cut.Models[TR2Type.CutsceneActor8], // Goon
            [TR2Type.CutsceneActor9] = cut.Models[TR2Type.CutsceneActor9], // Goon
            [TR2Type.CutsceneActor10] = cut.Models[TR2Type.CutsceneActor10], // Goon
        };
        ResetLevel(cut);
        cut.Models = models;

        // Hide shadows when the goons go into the jade
        foreach (var model in cut.Models.Values)
        {
            model.Animations[7].Commands.Add(new TRFXCommand
            {
                EffectID = (short)TR2FX.ShadowOff,
                FrameNumber = 94,
            });
        }

        return InjectionData.Create(cut, InjectionType.General, "cut4_setup", true);
    }
}
