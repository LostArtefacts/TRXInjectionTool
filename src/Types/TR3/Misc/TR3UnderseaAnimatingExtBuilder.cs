using TRLevelControl.Helpers;
using TRXInjectionTool.Actions;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Misc;

public class TR3UnderseaAnimatingExtBuilder : ItemBuilder
{
    private const int M_Propeller2ID = 119;
    private const int M_AnimatingExt1ID = 370;

    public override string ID => "undersea_animating_ext";

    public override List<InjectionData> Build()
    {
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.FISHES}");
        var itemIndices = level.Entities
            .Select((entity, index) => new { entity, index })
            .Where(x => (int)x.entity.TypeID == M_Propeller2ID)
            .Select(x => (short)x.index)
            .ToList();

        if (itemIndices.Count == 0)
        {
            throw new InvalidOperationException(
                $"Expected at least one {M_Propeller2ID} item in {TR3LevelNames.FISHES}.");
        }

        var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.General, ID);
        data.ObjectLinkEdits.Add(new()
        {
            BaseType = M_AnimatingExt1ID,
            SourceType = M_Propeller2ID,
        });
        foreach (var itemIndex in itemIndices)
        {
            data.ItemFlagEdits.Add(
                SetType(level, itemIndex, (TR3Type)M_AnimatingExt1ID));
        }
        return [data];
    }
}
