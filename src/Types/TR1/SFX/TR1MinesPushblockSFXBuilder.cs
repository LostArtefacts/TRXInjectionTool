using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.SFX;

public class TR1MinesPushblockSFXBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level mines = _control1.Read($"Resources/{TR1LevelNames.MINES}");
        TRDictionary<TR1Type, TRModel> pushblocks = new();
        foreach (TR1Type blockType in TR1TypeUtilities.GetPushblockTypes())
        {
            pushblocks[blockType] = mines.Models[blockType];
        }

        ResetLevel(mines);

        TRModel basePushblock = pushblocks[TR1Type.PushBlock1];
        pushblocks.Remove(TR1Type.PushBlock1);
        mines.Models = pushblocks;

        foreach (TRModel pushblock in pushblocks.Values)
        {
            pushblock.Animations[2].Commands = basePushblock.Animations[2].Commands;
        }

        InjectionData data = InjectionData.Create(mines, InjectionType.General, "mines_pushblocks", true);
        CreateDefaultTests(data, TR1LevelNames.MINES);
        return new() { data };
    }
}
