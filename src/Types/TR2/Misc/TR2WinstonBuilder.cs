using TRLevelControl.Model;
using TRXInjectionTool.Control;
using TRDataControl;
using TRImageControl;
using TRImageControl.Packing;

namespace TRXInjectionTool.Types.TR2.Misc;

public class TR2WinstonBuilder : InjectionBuilder
{
    public override string ID => "tr2-winston";

    protected static TR2Level CreateWinstonLevel()
    {
        var level = _control2.Read($"Resources/TR2/assault.tr2");
        var models = new TRDictionary<TR2Type, TRModel>();

        List<TR2SFX> soundIDs = new() {
            TR2SFX.WinstonGrunt1,
            TR2SFX.WinstonGrunt2,
            TR2SFX.WinstonGrunt3,
            TR2SFX.WinstonCups,
        };
        TR2DataProvider tr2Data = new();
        var model = level.Models[TR2Type.Winston];
        models[TR2Type.Winston] = model;
        soundIDs.AddRange(model.Animations
            .SelectMany(a => a.Commands.Where(c => c is TRSFXCommand))
            .Select(s => (TR2SFX)((TRSFXCommand)s).SoundID));

        var idsToPack = soundIDs
            .Where(s => level.SoundEffects.ContainsKey(s))
            .Distinct()
            .ToList();
        TRDictionary<TR2SFX, TR2SoundEffect> effects = new();
        idsToPack.ForEach(s => effects[s] = level.SoundEffects[s]);

        var packer = new TR2TexturePacker(level);
        var regions = packer.GetMeshRegions(models.Values.SelectMany(m => m.Meshes))
            .Values.SelectMany(v => v);
        var originalInfos = level.ObjectTextures.ToList();

        var basePalette = level.Palette.Select(c => c.ToTR1Color()).ToList();
        ResetLevel(level, 1);

        packer = new(level);
        packer.AddRectangles(regions);
        packer.Pack(true);

        level.Models = models;
        level.SoundEffects = effects;
        level.ObjectTextures.AddRange(regions.SelectMany(r => r.Segments.Select(s => s.Texture as TRObjectTexture)));
        models.Values
            .SelectMany(m => m.Meshes)
            .SelectMany(m => m.TexturedFaces)
            .ToList()
            .ForEach(f =>
            {
                f.Texture = (ushort)level.ObjectTextures.IndexOf(originalInfos[f.Texture]);
            });

        // Fix Winston's nose
        model.Meshes[25].TexturedTriangles.Add(new()
        {
            Type = TRFaceType.Triangle,
            Vertices = new() { 24, 22, 25 },
            Texture = model.Meshes[25].TexturedRectangles[9].Texture,
        });

        GenerateImages8(level, basePalette);
        return level;
    }

    public override List<InjectionData> Build()
    {
        var winstonLevel = CreateWinstonLevel();
        _control2.Write(winstonLevel, MakeOutputPath(TRGameVersion.TR2, $"Debug/{ID}.tr2"));

        var data = InjectionData.Create(winstonLevel, InjectionType.General, "winston_model");
        return new() { data };
    }
}
