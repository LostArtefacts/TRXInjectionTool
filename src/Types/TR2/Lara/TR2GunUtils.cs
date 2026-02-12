using Newtonsoft.Json;
using System.Drawing;
using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;

namespace TRXInjectionTool.Types.TR2.Lara;

public static class TR2GunUtils
{
    public static void ReplaceGlovedHands(TR2Level level)
    {
        // Remove gloves from gun anim hands.
        var handsImg = new TRImage("Resources/TR2/lara_hands.png");
        var handsMap = JsonConvert.DeserializeObject<List<TexMap>>
            (File.ReadAllText("Resources/TR2/lara_hands.json"));
        foreach (var map in handsMap)
        {
            var tex = handsImg.Export(map.TexA);
            var id = tex.GenerateID();
            foreach (var objTex in level.ObjectTextures)
            {
                var tile = new TRImage(level.Images16[objTex.Atlas].Pixels);
                var img = tile.Export(objTex.Bounds);
                if (img.GenerateID() == id)
                {
                    var cleanHand = handsImg.Export(map.TexB);
                    tile.Import(cleanHand, objTex.Position);
                    level.Images16[objTex.Atlas].Pixels = tile.ToRGB555();
                }
            }
        }

        var verts = new List<ushort> { 2, 3, 6, 7 };
        var baseFace = level.Models[TR2Type.LaraPistolAnim_H]
            .Meshes[10].TexturedRectangles.Find(t => t.Vertices.All(verts.Contains));
        foreach (var type in new[] { TR2Type.LaraHarpoonAnim_H, TR2Type.LaraM16Anim_H, TR2Type.LaraGrenadeAnim_H })
        {
            var face = level.Models[type]
                .Meshes[10].TexturedRectangles.Find(t => t.Vertices.All(verts.Contains));
            face.Texture = baseFace.Texture;
        }
    }

    public static List<TRAnimCmdEdit> FixHolsterSFX(TR2Level level, bool resetLevel)
    {
        var animMap = new Dictionary<TR2Type, List<int>>
        {
            [TR2Type.LaraShotgunAnim_H] = [1],
            [TR2Type.LaraM16Anim_H] = [1],
            [TR2Type.LaraGrenadeAnim_H] = [0],
            [TR2Type.LaraHarpoonAnim_H] = [1, 9, 10],
        };

        var models = new TRDictionary<TR2Type, TRModel>();
        foreach (var type in animMap.Keys)
        {
            if (level.Models.TryGetValue(type, out var model))
            {
                models[type] = model;
            }
        }

        var edits = new List<TRAnimCmdEdit>();
        foreach (var (type, model) in models)
        {
            var animIds = animMap[type];
            for (int i = 0; i < animIds.Count; i++)
            {
                var anim = model.Animations[animIds[i]];
                if (i == 0)
                {
                    (anim.Commands[0] as TRSFXCommand).SoundID = (short)TR2SFX.LaraDraw;
                }
                else
                {
                    anim.Commands.Add(new TRSFXCommand
                    {
                        SoundID = (short)TR2SFX.LaraHolster,
                        FrameNumber = 20,
                    });
                }

                edits.Add(InjectionBuilder.CreateAnimCmdEdit(level, type, animIds[i]));
            }

            if (resetLevel)
            {
                for (int i = 0; i < model.Animations.Count; i++)
                {
                    if (!animIds.Contains(i))
                    {
                        model.Animations[i].Commands.Clear();
                    }
                }
            }
        }

        if (resetLevel)
        {
            InjectionBuilder.ResetLevel(level);
            level.Models = models;
        }

        return edits;
    }

    private class TexMap
    {
        public Rectangle TexA { get; set; }
        public Rectangle TexB { get; set; }
    }
}
