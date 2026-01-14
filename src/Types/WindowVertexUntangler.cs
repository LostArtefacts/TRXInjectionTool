using TRLevelControl.Model;

namespace TRXInjectionTool.Util;

public static class WindowVertexUntangler
{
    public static TRMesh UntangleFaces(TRMesh mesh)
    {
        if (mesh.Vertices.Count == 0)
        {
            return mesh.Clone();
        }

        var newMesh = new TRMesh
        {
            Centre = mesh.Centre.Clone(),
            CollRadius = mesh.CollRadius,
            Vertices = new(mesh.Vertices.Select(v => v.Clone())),
            Normals = mesh.Normals == null ? null : new(mesh.Normals.Select(n => n.Clone())),
            Lights = mesh.Lights == null ? null : new(mesh.Lights),
            TexturedRectangles = new(),
            TexturedTriangles = new(),
            ColouredRectangles = new(),
            ColouredTriangles = new(),
        };

        var usedVertexIndices = new HashSet<ushort>();

        foreach (TRMeshFace face in mesh.TexturedRectangles)
        {
            newMesh.TexturedRectangles.Add(CloneFaceWithUniqueVertices(face, mesh, newMesh, usedVertexIndices));
        }

        foreach (TRMeshFace face in mesh.TexturedTriangles)
        {
            newMesh.TexturedTriangles.Add(CloneFaceWithUniqueVertices(face, mesh, newMesh, usedVertexIndices));
        }

        foreach (TRMeshFace face in mesh.ColouredRectangles)
        {
            newMesh.ColouredRectangles.Add(CloneFaceWithUniqueVertices(face, mesh, newMesh, usedVertexIndices));
        }

        foreach (TRMeshFace face in mesh.ColouredTriangles)
        {
            newMesh.ColouredTriangles.Add(CloneFaceWithUniqueVertices(face, mesh, newMesh, usedVertexIndices));
        }

        if (newMesh.Normals != null) {
            foreach (var normal in newMesh.Normals) {
                Console.WriteLine($"{normal.X}, {normal.Y}, {normal.Z}");
            }
        }

        return newMesh;
    }

    private static TRMeshFace CloneFaceWithUniqueVertices(
        TRMeshFace face,
        TRMesh sourceMesh,
        TRMesh targetMesh,
        HashSet<ushort> usedVertexIndices)
    {
        var newFace = face.Clone();

        for (int i = 0; i < newFace.Vertices.Count; i++)
        {
            ushort originalIndex = newFace.Vertices[i];
            if (!usedVertexIndices.Add(originalIndex))
            {
                ushort newIndex = DuplicateVertexWithFlippedNormal(originalIndex, sourceMesh, targetMesh);
                newFace.Vertices[i] = newIndex;
            }
        }

        return newFace;
    }

    private static ushort DuplicateVertexWithFlippedNormal(ushort originalIndex, TRMesh sourceMesh, TRMesh targetMesh)
    {
        targetMesh.Vertices.Add(sourceMesh.Vertices[originalIndex].Clone());

        if (sourceMesh.Normals != null)
        {
            TRVertex normal = sourceMesh.Normals[originalIndex];
            targetMesh.Normals!.Add(FlipNormal(normal));
        }
        else if (sourceMesh.Lights != null)
        {
            targetMesh.Lights!.Add(sourceMesh.Lights[originalIndex]);
        }

        return (ushort)(targetMesh.Vertices.Count - 1);
    }

    private static TRVertex FlipNormal(TRVertex normal)
    {
        return new()
        {
            X = (short)-normal.X,
            Y = (short)-normal.Y,
            Z = (short)-normal.Z,
        };
    }
}

