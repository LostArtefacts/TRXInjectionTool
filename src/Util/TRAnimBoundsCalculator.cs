using System.Numerics;
using TRLevelControl.Model;

namespace TRXInjectionTool.Util;

public static class TRAnimBoundsCalculator
{
    private const float _angleToRadians = MathF.Tau / 1024f;

    public static TRBoundingBox ComputeFrameBounds(TRModel model, TRAnimFrame frame)
    {
        if (model.Meshes.Count == 0)
        {
            return new TRBoundingBox();
        }

        Stack<Matrix4x4> stack = new();
        Matrix4x4 current = Matrix4x4.CreateTranslation(frame.OffsetX, frame.OffsetY, frame.OffsetZ);

        int meshCount = model.Meshes.Count;
        int rotationCount = frame.Rotations.Count;

        int minX = int.MaxValue;
        int minY = int.MaxValue;
        int minZ = int.MaxValue;
        int maxX = int.MinValue;
        int maxY = int.MinValue;
        int maxZ = int.MinValue;

        for (int meshIndex = 0; meshIndex < meshCount; meshIndex++)
        {
            if (meshIndex > 0 && model.MeshTrees.Count >= meshIndex)
            {
                TRMeshTreeNode node = model.MeshTrees[meshIndex - 1];
                if ((node.Flags & 0x01) != 0 && stack.Count > 0)
                {
                    current = stack.Pop();
                }
                if ((node.Flags & 0x02) != 0)
                {
                    stack.Push(current);
                }
                current = Matrix4x4.CreateTranslation(node.OffsetX, node.OffsetY, node.OffsetZ) * current;
            }

            if (meshIndex < rotationCount)
            {
                TRAnimFrameRotation rot = frame.Rotations[meshIndex];
                current = CreateRotation(rot) * current;
            }

            foreach (TRVertex vertex in model.Meshes[meshIndex].Vertices)
            {
                Vector3 v = Vector3.Transform(new Vector3(vertex.X, vertex.Y, vertex.Z), current);
                minX = Math.Min(minX, (int)MathF.Floor(v.X));
                minY = Math.Min(minY, (int)MathF.Floor(v.Y));
                minZ = Math.Min(minZ, (int)MathF.Floor(v.Z));
                maxX = Math.Max(maxX, (int)MathF.Ceiling(v.X));
                maxY = Math.Max(maxY, (int)MathF.Ceiling(v.Y));
                maxZ = Math.Max(maxZ, (int)MathF.Ceiling(v.Z));
            }
        }

        if (minX == int.MaxValue)
        {
            return new TRBoundingBox();
        }

        return new TRBoundingBox
        {
            MinX = ClampToInt16(minX),
            MinY = ClampToInt16(minY),
            MinZ = ClampToInt16(minZ),
            MaxX = ClampToInt16(maxX),
            MaxY = ClampToInt16(maxY),
            MaxZ = ClampToInt16(maxZ),
        };
    }

    private static Matrix4x4 CreateRotation(TRAnimFrameRotation rotation)
    {
        float x = rotation.X * _angleToRadians;
        float y = rotation.Y * _angleToRadians;
        float z = rotation.Z * _angleToRadians;

        Matrix4x4 rx = Matrix4x4.CreateRotationX(x);
        Matrix4x4 ry = Matrix4x4.CreateRotationY(y);
        Matrix4x4 rz = Matrix4x4.CreateRotationZ(z);

        return rz * ry * rx;
    }

    private static short ClampToInt16(int value)
        => (short)Math.Clamp(value, short.MinValue, short.MaxValue);
}
