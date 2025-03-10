using TRLevelControl;

namespace TRXInjectionTool.Applicability;

public abstract class ApplicabilityTest
{
    public abstract ApplicabilityType Type { get; }
    
    public void Serialize(TRLevelWriter writer)
    {
        writer.Write((int)Type);
        SerializeImpl(writer);
    }

    protected abstract void SerializeImpl(TRLevelWriter writer);
}
