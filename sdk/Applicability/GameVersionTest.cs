using TRLevelControl;
using TRLevelControl.Model;

namespace TRXInjectionTool.Applicability;

// Restricts an injection to the game its data is written for. The engine
// numbers the games from 1, where TRGameVersion counts from 0.
public class GameVersionTest : ApplicabilityTest
{
    public override ApplicabilityType Type => ApplicabilityType.GameVersion;

    protected override void SerializeImpl(TRLevelWriter writer, TRGameVersion version)
    {
        writer.Write((int)version + 1);
    }
}
