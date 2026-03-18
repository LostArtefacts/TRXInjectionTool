using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Textures;

public class TR3CityTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        var data = CreateBaseData();
        CreateDefaultTests(data, $"TR3/{TR3LevelNames.CITY}");
        return [data];
    }

    private static InjectionData CreateBaseData()
    {
        // The fish in the City cutscene have better texturing than City itself, so replace
        // all of the meshes.
        var cut = _control3.Read($"Resources/TR3/{TR3LevelNames.LUDS_CUT}");
        CreateModelLevel(cut, TR3Type.CutsceneActor4);
        cut.SoundEffects.Clear();

        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.CITY}");
        var animatings = new[] { TR3Type.Animating2, TR3Type.Animating3, TR3Type.Animating4, TR3Type.Animating5 };
        var models = new TRDictionary<TR3Type, TRModel>();
        foreach (var type in animatings)
        {
            models[type] = level.Models[type];
        }
        ResetLevel(level, 1);

        level.Models = models;
        level.Images16 = cut.Images16;
        level.Images8 = cut.Images8;
        level.ObjectTextures = cut.ObjectTextures;

        foreach (var model in models.Values)
        {
            model.Meshes = [.. cut.Models[TR3Type.CutsceneActor4].Meshes.Select(m => m.Clone())];
            model.Animations.Clear();
        }

        var data = InjectionData.Create(level, InjectionType.TextureFix, "city_textures");
        foreach (var type in animatings)
        {
            data.SetMeshOnlyModel((uint)type);
        }

        return data;
    }
}
