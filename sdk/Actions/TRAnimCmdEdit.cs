using TRLevelControl;
using TRLevelControl.Model;
using TRXInjectionTool.Control;
using TRXInjectionTool.Util;

namespace TRXInjectionTool.Actions;

public class TRAnimCmdEdit
{
    // This relies on setup defining the raw list of commands. This action
    // will then update the correspoding animation to point to the new list.
    public int TypeID { get; set; }
    public int AnimIndex { get; set; }
    public int RawCount { get; set; }
    public int TotalCount { get; set; }

    public void Serialize(TRLevelWriter writer, TRGameVersion version)
    {
        writer.Write(TypeID, TRObjectType.Game, version);
        writer.Write(AnimIndex);
        writer.Write(RawCount);
        writer.Write(TotalCount);
    }
}
