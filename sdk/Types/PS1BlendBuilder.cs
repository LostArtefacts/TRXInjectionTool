using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;
using TRXInjectionTool.Util;

namespace TRXInjectionTool.Types;

// Marks the faces that the PlayStation release mixes half and half with the
// scene. The lists come from TRX's tools/find_ps1_semi_faces, one per level,
// naming the faces by room or by object mesh as the PC level holds them.
public abstract class PS1BlendBuilder : InjectionBuilder
{
    // The face effect that TRX reads as PlayStation semi-transparency.
    private const ushort _semiTransparentEffect = 0x8000;

    protected abstract TRGameVersion GameVersion { get; }

    // The levels, by the stem of their file, with the name the injection
    // files of each level start with.
    protected abstract Dictionary<string, (string FileName, string Prefix)> Levels { get; }

    private sealed class FaceList
    {
        public Dictionary<string, Dictionary<string, List<short>>> Rooms { get; set; } = [];
        public Dictionary<string, Dictionary<string, Dictionary<string, List<short>>>> Objects { get; set; } = [];
    }

    public override List<InjectionData> Build()
    {
        string folder = $"Resources/{GameVersion}/PS1Blend";
        List<InjectionData> result = [];
        foreach (var (stem, (fileName, prefix)) in Levels)
        {
            string path = Path.Combine(folder, $"{stem}.json");
            if (!File.Exists(path))
            {
                continue;
            }

            FaceList faces = JsonUtils.DeserializeFile<FaceList>(path);
            InjectionData data = InjectionData.Create(GameVersion, InjectionType.PS1Blend, $"{prefix}_ps1_blend");
            CreateDefaultTests(data, fileName);

            foreach (var (room, groups) in faces.Rooms)
            {
                foreach (var (group, indices) in groups)
                {
                    foreach (short index in indices)
                    {
                        data.RoomEdits.Add(new TRRoomTexturePS1SemiTransparent
                        {
                            RoomIndex = short.Parse(room),
                            FaceType = ParseFaceType(group),
                            TargetIndex = index,
                            SemiTransparent = true,
                        });
                    }
                }
            }

            foreach (var (objectID, meshes) in faces.Objects)
            {
                foreach (var (meshIndex, groups) in meshes)
                {
                    TRMeshEdit edit = new()
                    {
                        ModelID = uint.Parse(objectID),
                        MeshIndex = short.Parse(meshIndex),
                    };
                    foreach (var (group, indices) in groups)
                    {
                        foreach (short index in indices)
                        {
                            edit.FaceEffects.Add(new()
                            {
                                FaceType = ParseFaceType(group),
                                FaceIndex = index,
                                Effects = _semiTransparentEffect,
                            });
                        }
                    }
                    data.MeshEdits.Add(edit);
                }
            }

            result.Add(data);
        }
        return result;
    }

    private static TRMeshFaceType ParseFaceType(string group) => group switch
    {
        "textured_quad" => TRMeshFaceType.TexturedQuad,
        "textured_triangle" => TRMeshFaceType.TexturedTriangle,
        "coloured_quad" => TRMeshFaceType.ColouredQuad,
        "coloured_triangle" => TRMeshFaceType.ColouredTriangle,
        _ => throw new InvalidDataException($"unknown face type {group}"),
    };
}
