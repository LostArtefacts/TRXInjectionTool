using TRLevelControl;
using TRLevelControl.Model;

namespace TRXInjectionTool.Applicability;

public abstract class ApplicabilityTest
{
    public abstract ApplicabilityType Type { get; }
    
    public void Serialize(TRLevelWriter writer, TRGameVersion version)
    {
        writer.Write((int)Type);
        SerializeImpl(writer, version);
    }

    protected abstract void SerializeImpl(TRLevelWriter writer, TRGameVersion version);
}
