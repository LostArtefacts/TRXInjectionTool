using System.Diagnostics;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.SFX;

public class TR2BreakableTileSFXBuilder : InjectionBuilder
{
    public override string ID => "breakable_tile_sfx";

    public override List<InjectionData> Build()
    {
        return new()
        {
            CreateLooseBoardsFix(),
            CreateBreakableTileFix(),
        };
    }

    private static InjectionData CreateLooseBoardsFix()
    {
        TR2Level level = _control2.Read($"Resources/{TR2LevelNames.OPERA}");
        const TR2Type type = TR2Type.LooseBoards;
        TRModel boards = level.Models[type];
        ResetLevel(level);
        level.Models[type] = boards;

        foreach (short frame in new short[] { 4, 12, 22, 27 })
        {
            boards.Animations[1].Commands.Add(new TRSFXCommand
            {
                FrameNumber = frame,
                SoundID = (short)TR2SFX.SandbagSnap,
            });
        }

        foreach (short frame in new short[] { 1, 18 })
        {
            boards.Animations[4].Commands.Add(new TRSFXCommand
            {
                FrameNumber = frame,
                SoundID = (short)TR2SFX.SandbagHit,
            });
        }

        InjectionData data = CreateBaseData("loose_boards_sfx", level);
        foreach (int animIdx in new int[] { 1, 4 })
        {
            data.AnimCmdEdits.Add(CreateEdit(level, type, animIdx));
        }
        Debug.Assert(data.AnimCmdEdits.Sum(c => c.RawCount) == data.AnimCommands.Count);

        return data;
    }

    private static InjectionData CreateBreakableTileFix()
    {
        TR2Level level = _control2.Read($"Resources/{TR2LevelNames.GW}");
        Dictionary<TR2SFX, TR2SoundEffect> sfxCache = new(level.SoundEffects);
        const TR2Type type = TR2Type.FallingBlock;
        TRModel breakableTile = level.Models[type];
        ResetLevel(level);

        level.Models[type] = breakableTile;

        breakableTile.Animations[1].Commands.Add(new TRSFXCommand
        {
            FrameNumber = 1,
            SoundID = (short)TR2SFX.RockFallCrumble,
        });
        breakableTile.Animations[2].Commands.Add(new TRSFXCommand
        {
            FrameNumber = 1,
            SoundID = (short)TR2SFX.RockFallLand,
        });
        breakableTile.Animations[3].Commands.Add(new TRSFXCommand
        {
            FrameNumber = 1,
            SoundID = (short)TR2SFX.RockFallSolid,
        });

        breakableTile.Animations
            .SelectMany(a => a.Commands.Where(c => c is TRSFXCommand).Select(c => (TR2SFX)((TRSFXCommand)c).SoundID))
            .Distinct().ToList()
            .ForEach(s => level.SoundEffects[s] = sfxCache[s]);

        InjectionData data = CreateBaseData("breakable_tile_sfx", level);
        for (int animIdx = 1; animIdx <= 3; animIdx++)
        {
            data.AnimCmdEdits.Add(CreateEdit(level, type, animIdx));
        }
        Debug.Assert(data.AnimCmdEdits.Sum(c => c.RawCount) == data.AnimCommands.Count);
        return data;
    }

    private static InjectionData CreateBaseData(string name, TR2Level level)
    {
        InjectionData data = InjectionData.Create(level, InjectionType.General, name, true);
        data.Animations.Clear();
        data.AnimFrames.Clear();
        data.AnimChanges.Clear();
        data.AnimDispatches.Clear();
        data.Models.Clear();
        return data;
    }

    private static TRAnimCmdEdit CreateEdit(TR2Level level, TR2Type type, int animIdx)
    {
        TRAnimation anim = level.Models[type].Animations[animIdx];
        int rawCount = 0;
        foreach (TRAnimCommand cmd in anim.Commands)
        {
            rawCount++;
            switch (cmd.Type)
            {
                case TRAnimCommandType.SetPosition:
                    rawCount += 3;
                    break;
                case TRAnimCommandType.JumpDistance:
                case TRAnimCommandType.FlipEffect:
                case TRAnimCommandType.PlaySound:
                    rawCount += 2;
                    break;
            }
        }

        return new()
        {
            AnimIndex = animIdx,
            TypeID = (short)type,
            RawCount = rawCount,
            TotalCount = anim.Commands.Count,
        };
    }
}
