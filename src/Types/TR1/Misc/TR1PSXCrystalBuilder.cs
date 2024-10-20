using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Misc;

public class TR1PSXCrystalBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level caves = _control1.Read($@"Resources\{TR1LevelNames.CAVES}");
        TRModel crystal = caves.Models[TR1Type.SavegameCrystal_P];
        ResetLevel(caves);

        caves.Models[TR1Type.SavegameCrystal_P] = crystal;
        caves.Palette[1] = new()
        {
            Red = 64,
            Green = 64,
            Blue = 252,
        };

        crystal.Meshes[0].ColouredFaces.ToList()
            .ForEach(f => f.Texture = 1);

        InjectionData data = InjectionData.Create(caves, InjectionType.PSCrystal, "purple_crystal");
        return new() { data };
    }
}
