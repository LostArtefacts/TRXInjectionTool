using System.Diagnostics;
using System.IO.Compression;
using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Lara;

public class TR3LaraAnimBuilder : LaraBuilder
{
    public override string ID => "tr3_lara_anims";
    public override TRGameVersion GameVersion => TRGameVersion.TR3;
    protected override short JumpSFX => (short)TR3SFX.LaraJump;
    protected override short DryFeetSFX => (short)TR3SFX.LaraFeet;
    protected override short WetFeetSFX => (short)TR3SFX.LaraWetFeet;
    protected override short LandSFX => (short)TR3SFX.LaraLand;
    protected override short ResponsiveState => (short)InjState.Responsive;

    enum InjAnim : int
    {
        Kick = 313,
        JumpNeutralRoll = 314,
        ControlledDrop = 315,
        ControlledDropContinue = 316,
        HangToJumpUp = 317,
        HangToJumpUpContinue = 318,
        HangToJumpBack = 319,
        HangToJumpBackContinue = 320,
        PoseRightStart = 321,
        PoseRightContinue = 322,
        PoseRightEnd = 323,
        PoseLeftStart = 324,
        PoseLeftContinue = 325,
        PoseLeftEnd = 326,
    };

    enum InjState : int
    {
        Controlled = 89,
        Responsive = 90,
        NeutralRoll = 91,
        PoseStart = 92,
        PoseEnd = 93,
        PoseLeft = 94,
        PoseRight = 95,
    };

    public override List<InjectionData> Build()
    {
        var result = new List<InjectionData>();

        {
            var level = CreateLevel();
            result.Add(InjectionData.Create(level, InjectionType.LaraAnims, "lara_animations"));
        }

        return result;
    }

    public override byte[] Publish()
    {
        var level = CreateLevel();
        return ExportLaraWAD(level);
    }

    private TR3Level CreateLevel()
    {
        var jungle = _control3.Read($"Resources/{TR3LevelNames.JUNGLE}");
        ResetLevel(jungle);
        var tr3Lara = jungle.Models[TR3Type.Lara];
        
        SyncToTR2(tr3Lara);
        ImportTR1Jumping(tr3Lara);
        ImportTR1Gliding(tr3Lara);
        ImproveTwists(tr3Lara);
        ImportNeutralTwist(tr3Lara, (short)InjAnim.JumpNeutralRoll, (short)InjState.NeutralRoll);
        ImportControlledDrop(tr3Lara, (short)InjAnim.ControlledDropContinue);
        ImportHangToJump(tr3Lara, (short)InjAnim.HangToJumpUp);
        ImportIdlePose(tr3Lara, InjState.PoseStart, InjState.PoseEnd, InjState.PoseLeft, InjState.PoseRight);
        FixJumpToFreefall(tr3Lara);

        return jungle;
    }

    private void SyncToTR2(TRModel lara)
    {
        // Unused kick from TR2 was removed in TR3. Re-add so that counts align in all three games.
        var tr2Lara = _control2.Read($"Resources/{TR2LevelNames.GW}").Models[TR2Type.Lara];
        Debug.Assert(lara.Animations.Count == (int)InjAnim.Kick);
        lara.Animations.Add(tr2Lara.Animations[214]);
        AddChange(lara, 103, TR3LaraState.Kick, 0, 69, InjAnim.Kick, 0);

        // Alter slide to run
        var change = lara.Animations[70].Changes.Find(c => c.StateID == 1);
        change.StateID = (ushort)ResponsiveState;
        change.Dispatches[0].NextFrame = 2;
    }

    private static void ImportTR1Jumping(TRModel lara)
    {
        var runAnim = lara.Animations[(int)LaraAnim.Run];
        var jumpChange = runAnim.Changes.FirstOrDefault(c => c.StateID == (ushort)LaraState.JumpForward);
        var responsiveChange = jumpChange.Clone();
        runAnim.Changes.Add(responsiveChange);
        responsiveChange.StateID = (ushort)InjState.Responsive;

        foreach (var dispatch in jumpChange.Dispatches)
        {
            if (dispatch.NextAnimation == (short)LaraAnim.RunJumpRightStart)
            {
                dispatch.Low = 14;
                dispatch.High = 15;
            }
            else
            {
                dispatch.Low = 3;
                dispatch.High = 4;
            }
        }
    }

    private static void ImportTR1Gliding(TRModel lara)
    {
        var swimAnim = lara.Animations[(int)LaraAnim.UnderwaterSwimForward];
        var glideChange = swimAnim.Changes.FirstOrDefault(c => c.StateID == (ushort)LaraState.Glide);
        var dispatches = glideChange.Dispatches.Select(d => d.Clone()).ToList();
        glideChange.Dispatches.RemoveAll(d => d.NextAnimation != (short)LaraAnim.UnderwaterSwimGlide);
        glideChange.Dispatches.FirstOrDefault(d => d.Low == 0).High = 2;

        swimAnim.Changes.Add(new()
        {
            StateID = (ushort)InjState.Responsive,
            Dispatches = dispatches,
        });

        dispatches.Sort((d1, d2) => d1.Low.CompareTo(d2.Low));
    }

    private static void ImproveTwists(TRModel lara)
    {
        var laraExt = GetLaraExtModel();
        lara.Animations[203] = laraExt.Animations[(int)ExtLaraAnim.UWRollStart];
        lara.Animations[205] = laraExt.Animations[(int)ExtLaraAnim.UWRollEnd];
        lara.Animations[203].NextAnimation = 205;
        lara.Animations[203].NextFrame = 1;
        lara.Animations[205].NextAnimation = 108;

        lara.Animations[207] = laraExt.Animations[(int)ExtLaraAnim.RunJumpRollStart];
        lara.Animations[209] = laraExt.Animations[(int)ExtLaraAnim.RunJumpRollEnd];
        lara.Animations[210] = laraExt.Animations[(int)ExtLaraAnim.JumpFwdRollStart];
        lara.Animations[211] = laraExt.Animations[(int)ExtLaraAnim.JumpFwdRollEnd];
        lara.Animations[212] = laraExt.Animations[(int)ExtLaraAnim.JumpBackRollStart];
        lara.Animations[213] = laraExt.Animations[(int)ExtLaraAnim.JumpBackRollEnd];

        lara.Animations[207].NextAnimation = 209;
        lara.Animations[209].NextAnimation = (ushort)LaraAnim.JumpBack;
        lara.Animations[209].NextFrame = 39;
        lara.Animations[210].NextAnimation = 211;
        lara.Animations[211].NextAnimation = (ushort)LaraAnim.JumpBack;
        lara.Animations[211].NextFrame = 39;
        lara.Animations[212].NextAnimation = 213;
        lara.Animations[213].NextAnimation = (ushort)LaraAnim.JumpForward;
        lara.Animations[213].NextFrame = 39;
    }

    private static byte[] ExportLaraWAD(TR3Level level)
    {
        // Generate the injection's effect on a regular level to allow TRLE builders to utilise
        // the new animations while also being able to edit the defaults. This is a stripped back
        // level file that can be opened in WADTool.
        var originalInfos = level.ObjectTextures.ToList();
        var texMap = new Dictionary<ushort, ushort>();
        level.Models[TR3Type.Lara].Meshes
            .SelectMany(m => m.TexturedFaces)
            .Select(f => f.Texture)
            .Distinct()
            .ToList().ForEach(t => texMap[t] = (ushort)texMap.Count);

        var packer = new TR3TexturePacker(level);
        var laraRegions = packer.GetMeshRegions(level.Models[TR3Type.Lara].Meshes)
            .Values.SelectMany(v => v);

        level.Images16 = new() { new() { Pixels = new ushort[TRConsts.TPageSize] } };
        level.ObjectTextures.Clear();

        packer = new(level);
        packer.AddRectangles(laraRegions);
        packer.Pack(true);

        level.ObjectTextures.AddRange(texMap.Keys.Select(t => originalInfos[t]));
        level.Models[TR3Type.Lara].Meshes
            .SelectMany(m => m.TexturedFaces)
            .ToList().ForEach(f => f.Texture = texMap[f.Texture]);

        GenerateImages8(level, level.Palette.Select(c => c.ToTR1Color()).ToList());

        return ExportZip(level);
    }

    private static byte[] ExportZip(TR3Level level)
    {
        var stream = new MemoryStream();
        using var zip = new ZipArchive(stream, ZipArchiveMode.Create);

        {
            using var ms = new MemoryStream();
            _control3.Write(level, ms);
            byte[] phdRaw = ms.ToArray();
            var entry = zip.CreateEntry("lara.tr2", CompressionLevel.Optimal);
            using var zipStream = entry.Open();
            zipStream.Write(phdRaw, 0, phdRaw.Length);
        }

        zip.Dispose();
        stream.Flush();
        return stream.ToArray();
    }

    private static void ResetLevel(TR3Level level)
    {
        level.Sprites.Clear();
        level.Rooms.Clear();
        level.StaticMeshes.Clear();
        level.Boxes.Clear();
        level.SoundEffects.Clear();
        level.Entities.Clear();
        level.Cameras.Clear();

        level.Models = new()
        {
            [TR3Type.Lara] = level.Models[TR3Type.Lara],
        };
    }
}
