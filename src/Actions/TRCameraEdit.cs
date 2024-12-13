using TRLevelControl;
using TRLevelControl.Model;

namespace TRXInjectionTool.Actions;

public class TRCameraEdit
{
    public short Index { get; set; }
    public TRCamera Camera { get; set; }

    public void Serialize(TRLevelWriter writer)
    {
        writer.Write(Index);
        writer.Write(Camera);
    }
}
