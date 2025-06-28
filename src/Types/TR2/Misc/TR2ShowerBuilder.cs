using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Misc;

public class TR2ShowerBuilder : InjectionBuilder
{
    public override string ID => "house_shower_frames";

    public override List<InjectionData> Build()
    {
        var hsh = _control2.Read($"Resources/{TR2LevelNames.HOME}");
        var laraExtra = hsh.Models[TR2Type.LaraMiscAnim_H];
        var cineFrames = hsh.CinematicFrames.ToList();
        for (int i = 1; i <= 3; i++)
        {
            // Get the camera out of the wall
            cineFrames[^i].Target.X += 208;
            cineFrames[^i].Position.X -= 144;
        }
        
        ResetLevel(hsh);
        hsh.Models[TR2Type.LaraMiscAnim_H] = laraExtra;
        hsh.CinematicFrames = cineFrames;

        TRAnimation endAnim = laraExtra.Animations[2];
        for (int i = 0; i < 120; i++)
        {
            endAnim.Frames.Add(endAnim.Frames[^1]);
            endAnim.FrameEnd++;
            hsh.CinematicFrames.Add(cineFrames[^1]);
        }

        InjectionData data = InjectionData.Create(hsh, InjectionType.General, ID, true);
        return new() { data };
    }
}
