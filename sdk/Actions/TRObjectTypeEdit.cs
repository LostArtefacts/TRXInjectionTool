using TRLevelControl;
using TRLevelControl.Model;
using TRXInjectionTool.Control;
using TRXInjectionTool.Util;

namespace TRXInjectionTool.Actions
{
    public class TRObjectTypeEdit
    {
        public int BaseType { get; set; }
        public int TargetType { get; set; }

        public void Serialize(TRLevelWriter writer, TRGameVersion version)
        {
            writer.Write(BaseType, TRObjectType.Game, version);
            writer.Write(TargetType, TRObjectType.Game, version);
        }
    }
}
