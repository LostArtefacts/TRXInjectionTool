using System.Text;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TRX.Poses;

public class TRXPoseBuilder : InjectionBuilder
{
    public override string ID => "pose";

    public override List<InjectionData> Build()
    {
        var level = _control1.Read($"Resources/pose.phd");
        var poses = level.Models[TR1Type.Lara].Animations.Select(a => new Pose(a)).ToList();
        
        string outputPath = $"Output/TRX/poses.json5";
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
        File.WriteAllText(outputPath, Format(poses));

        return new();
    }

    private static string Format(List<Pose> poses)
    {
        var formattedOffsets = FormatOffsets(poses);
        var formattedRots = FormatRots(poses);

        var sb = new StringBuilder();
        sb.AppendLine("[");

        for (int i = 0; i < poses.Count; i++)
        {
            sb.Append("    {\"offset\": ");
            sb.Append(formattedOffsets[i].PadRight(formattedOffsets.Max(s => s.Length) + 1));

            sb.Append("\"rots\": [");
            for (int j = 0; j < formattedRots.Count; j++)
            {
                var rots = formattedRots[j];
                if (j == formattedRots.Count - 1)
                {
                    sb.Append(rots[i]);
                }
                else
                {
                    sb.Append(rots[i].PadRight(rots.Max(s => s.Length) + 1));
                }
            }
            sb.AppendLine("]},");
        }

        sb.AppendLine("]");
        return sb.ToString();
    }

    private static List<string> FormatOffsets(List<Pose> poses)
    {
        var allOffsets = poses.Select(p => p.Offset).ToList();
        return poses.Select(p => FormatArray(p.Offset, allOffsets, true)).ToList();
    }

    private static List<List<string>> FormatRots(List<Pose> poses)
    {
        var result = new List<List<string>>();
        for (int i = 0; i < 15; i++)
        {
            var all = poses.Select(p => p.Rots[i]).ToList();
            result.Add(poses.Select(p => FormatArray(p.Rots[i], all, i < 14)).ToList());
        }
        return result;
    }

    private static string FormatArray(short[] arr, List<short[]> all, bool endComma)
    {
        int[] widths = new int[arr.Length];
        foreach (var arr2 in all)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                widths[i] = Math.Max(widths[i], arr2[i].ToString().Length);
            }
        }

        var sb = new StringBuilder();
        sb.Append('[');
        for (int i = 0; i < arr.Length; i++)
        {
            string s = arr[i].ToString();
            if (i < 2)
            {
                s += ',';
                sb.Append(s.PadRight(widths[i] + 2));
            }
            else
            {
                sb.Append(s);
            }
        }
        sb.Append(']');
        if (endComma)
        {
            sb.Append(',');
        }
        return sb.ToString();
    }

    private class Pose
    {
        public short[] Offset { get; set; }
        public short[][] Rots { get; set; }

        public Pose(TRAnimation animation)
        {
            var frame = animation.Frames[0];
            Offset = new short[]
            {
                frame.OffsetX,
                frame.OffsetY,
                frame.OffsetZ,
            };
            Rots = frame.Rotations.Select(r => new short[]
            {
                (short)(r.X << 6),
                (short)(r.Y << 6),
                (short)(r.Z << 6),
            }).ToArray();
        }
    }
}
