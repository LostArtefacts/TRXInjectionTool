using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Lara;

public class TR1LaraGunBuilder : InjectionBuilder, IPublisher
{
    public override string ID => "lara_guns";

    public override List<InjectionData> Build()
    {
        var level = CreateLevel();
        var data = InjectionData.Create(level, InjectionType.General, ID);
        return [data];
    }

    private static TR1Level CreateLevel()
    {
        var level = _control1.Read($"Resources/{TR1LevelNames.CAVES}");

        var model = level.Models[TR1Type.LaraShotgunAnim_H];
        var oldBack = model.Meshes[7];
        oldBack.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v < 30));
        oldBack.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v < 30));
        oldBack.ColouredRectangles.RemoveAll(f => f.Vertices.All(v => v < 30));
        oldBack.Vertices.RemoveRange(0, 30);
        oldBack.Normals.RemoveRange(0, 30);
        oldBack.TexturedFaces.Concat(oldBack.ColouredFaces).ToList().ForEach(f =>
        {
            for (int i = 0; i < f.Vertices.Count; i++)
            {
                f.Vertices[i] -= 30;
            }
        });

        {
            // Fix glove
            var gloveFace = model.Meshes[10].ColouredRectangles[1];
            gloveFace.Texture = level.Models[TR1Type.Lara].Meshes[10].TexturedRectangles[3].Texture;
            model.Meshes[10].ColouredRectangles.Remove(gloveFace);
            model.Meshes[10].TexturedRectangles.Add(gloveFace);
        }

        var hips = level.Models[TR1Type.Lara].Meshes[0];
        CreateModelLevel(level, TR1Type.LaraShotgunAnim_H);

        hips.TexturedFaces.Concat(hips.ColouredFaces)
            .ToList().ForEach(f => f.Texture = 0);
        hips.ColouredRectangles.AddRange(hips.TexturedRectangles);
        hips.ColouredTriangles.AddRange(hips.TexturedTriangles);
        hips.TexturedRectangles.Clear();
        hips.TexturedTriangles.Clear();

        model = level.Models[TR1Type.LaraShotgunAnim_H];
        oldBack = model.Meshes[7];
        for (int i = 0; i < 15; i++)
        {
            if (model.Meshes[i] == null || i == 7)
            {
                model.Meshes[i] = hips;
            }
        }

        oldBack.Vertices.ForEach(v =>
        {
            v.Y += 204;
            v.Z += 25;
        });
        oldBack.Centre = new() { X = 42, Y = 99, Z = 70 };
        oldBack.CollRadius = 105;
        model.Meshes[14] = oldBack;

        var wall = _control2.Read($"Resources/{TR2LevelNames.GW}");
        model.Animations = wall.Models[TR2Type.LaraShotgunAnim_H].Animations;
        // Fix using holster SFX on draw
        (model.Animations[1].Commands[0] as TRSFXCommand).SoundID = (short)TR1SFX.LaraDraw;

        return level;
    }

    public string GetPublishedName()
        => "lara_guns.phd";

    public TRLevelBase Publish()
        => CreateLevel();
}
