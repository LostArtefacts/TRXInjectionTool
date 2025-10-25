using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Items;

public class TR1ObeliskItemBuilder : ItemBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level obelisk = _control1.Read($"Resources/{TR1LevelNames.OBELISK}");
        
        return new()
        {
            CreateItemRots(obelisk),
            FixMeshPositions(obelisk),
        };
    }

    private static InjectionData CreateItemRots(TR1Level obelisk)
    {
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.ItemRotation, "obelisk_itemrots");
        CreateDefaultTests(data, TR1LevelNames.OBELISK);

        data.ItemPosEdits = new()
        {
            SetAngle(obelisk, 15, 16384),
            SetAngle(obelisk, 17, -16384),
        };

        return data;
    }

    private static InjectionData FixMeshPositions(TR1Level obelisk)
    {
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.General, "obelisk_meshfixes");
        CreateDefaultTests(data, TR1LevelNames.OBELISK);

        FixBridge(obelisk, data);
        FixStatics(obelisk, data);

        return data;
    }

    private static void FixBridge(TR1Level obelisk, InjectionData data)
    {
        TRMesh mesh = obelisk.Models[TR1Type.LiftingDoor].Meshes[0];
        TRMeshEdit bridgeEdit = new()
        {
            ModelID = (uint)TR1Type.LiftingDoor,
            MeshIndex = 0,
            VertexEdits = new(),
        };
        data.MeshEdits.Add(bridgeEdit);

        // Make the bridge flush with the floor when it's open, and extend it to touch
        // the wall behind the artefacts.
        short minY = mesh.Vertices.Min(v => v.Y);
        for (short i = 0; i < mesh.Vertices.Count; i++)
        {
            bridgeEdit.VertexEdits.Add(new()
            {
                Index = i,
                Change = new()
                {
                    Y = (short)(mesh.Vertices[i].Y == minY ? -5 : 0),
                    Z = -1,
                }
            });
        }

        // Rotate the bridge completely 90 degress when open. OG is 255 ~= 89.6.
        data.FrameEdits.Add(new()
        {
            ModelID = (uint)TR1Type.LiftingDoor,
            AnimIndex = 3,
            Rotation = new() { X = 256 },
        });
    }

    private static void FixStatics(TR1Level obelisk, InjectionData data)
    {
        {
            // Move the artefact stands down slightly and push them against the wall.
            TRMesh mesh = obelisk.StaticMeshes[TR1Type.Debris1].Mesh;
            TRMeshEdit edit = new()
            {
                ModelID = (uint)TR1Type.Debris1,
                MeshIndex = 0,
                VertexEdits = new(),
            };
            data.MeshEdits.Add(edit);

            for (short i = 0; i < mesh.Vertices.Count; i++)
            {
                edit.VertexEdits.Add(new()
                {
                    Index = i,
                    Change = new()
                    {
                        Y = 5,
                        Z = -13,
                    }
                });
            }
        }

        {
            // Move the senet table up
            TRMesh mesh = obelisk.StaticMeshes[TR1Type.Furniture1].Mesh;
            TRMeshEdit edit = new()
            {
                ModelID = (uint)TR1Type.Furniture1,
                MeshIndex = 0,
                VertexEdits = new(),
            };
            data.MeshEdits.Add(edit);

            for (short i = 0; i < mesh.Vertices.Count; i++)
            {
                edit.VertexEdits.Add(new()
                {
                    Index = i,
                    Change = new()
                    {
                        Y = -30,
                    }
                });
            }
        }

        data.MeshEdits.Add(FixEgyptToppledChair(TR1Type.Furniture3, obelisk));
        data.StaticMeshEdits.Add(FixEgyptPillar(obelisk));
    }
}
