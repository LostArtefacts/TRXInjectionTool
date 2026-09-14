using TRLevelControl.Helpers;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Misc;

public class TR2ShowerBuilder : InjectionBuilder
{
    public override string ID => "house_shower_frames";

    public override List<InjectionData> Build()
    {
        var hsh = _control2.Read($"Resources/{TR2LevelNames.HOME}");
        var cineFrames = hsh.CinematicFrames.ToList();
        for (int i = 1; i <= 3; i++)
        {
            // Get the camera out of the wall
            cineFrames[^i].Target.X += 208;
            cineFrames[^i].Position.X -= 144;
        }
        
        ResetLevel(hsh);
        hsh.CinematicFrames = cineFrames;

        var data = InjectionData.Create(hsh, InjectionType.General, ID, true);
        return [ data ];
    }
}
