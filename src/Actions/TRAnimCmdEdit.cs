using TRLevelControl;

namespace TRXInjectionTool.Actions;

public class TRAnimCmdEdit
{
    // This relies on setup defining the raw list of commands. This action
    // will then update the correspoding animation to point to the new list.
    public int TypeID { get; set; }
    public int AnimIndex { get; set; }
    public int RawCount { get; set; }
    public int TotalCount { get; set; }

    public void Serialize(TRLevelWriter writer)
    {
        writer.Write(TypeID);
        writer.Write(AnimIndex);
        writer.Write(RawCount);
        writer.Write(TotalCount);
    }
}
