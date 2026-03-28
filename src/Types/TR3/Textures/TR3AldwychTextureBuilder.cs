using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR3.Textures;

public class TR3AldwychTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        var data = InjectionData.Create(TRGameVersion.TR3, InjectionType.TextureFix, "aldwych_textures");
        FixStaircaseMesh(data, 37);
        return [data];
    }

    public static void FixStaircaseMesh(InjectionData data, uint meshIdx)
    {
        var right = new[] { 0, 1, 2, 9, 10, 15, 16 };
        var left = new[] { 3, 4, 5, 11, 12, 17, 18 };
        var top = new[] { 1, 2, 4, 5, 7, 8 };
        var back = new[] { 2, 5, 7 };

        TRVertex GetChange(int index)
        {
            var vtx = new TRVertex();
            
            if (right.Contains(index))
            {
                vtx.Z += 6;
            }
            else if (left.Contains(index))
            {
                vtx.Z -= 6;
            }
            
            if (top.Contains(index))
            {
                vtx.Y -= 6;
            }
            if (back.Contains(index))
            {
                vtx.X -= 6;
            }

            return vtx;
        }

        data.MeshEdits.Add(new()
        {
            ModelID = (uint)TR3Type.SceneryBase + meshIdx,
            EnforcedType = TRObjectType.Static3D,
            VertexEdits = [.. Enumerable.Range(0, 21).Select(i => new TRVertexEdit
            {
                Index = (short)i,
                Change = GetChange(i),
            })],
        });
    }
}
