using TRLevelControl;
using TRLevelControl.Model;
using TRXInjectionTool.Control;
using TRXInjectionTool.Util;

namespace TRXInjectionTool.Actions
{
    public class TRObjectLinkEdit
    {
        public int BaseType { get; set; }
        public int SourceType { get; set; }

        public void Serialize(TRLevelWriter writer, TRGameVersion version)
        {
            writer.Write(BaseType, TRObjectType.Game, version);
            writer.Write(SourceType, TRObjectType.Game, version);
        }
    }
}
