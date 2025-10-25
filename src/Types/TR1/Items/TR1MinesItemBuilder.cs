using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Items;

public class TR1MinesItemBuilder : ItemBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level mines = _control1.Read($"Resources/{TR1LevelNames.MINES}");

        return new()
        {
            CreateItemRots(mines),
            FixMeshPositions(),
        };
    }

    private static InjectionData CreateItemRots(TR1Level mines)
    {
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.ItemRotation, "mines_itemrots");
        CreateDefaultTests(data, TR1LevelNames.MINES);

        data.ItemPosEdits.Add(SetAngle(mines, 52, 16384));
        data.ItemPosEdits.Add(SetAngle(mines, 71, 16384));
        data.ItemPosEdits.Add(SetAngle(mines, 80, -16384));

        return data;
    }

    private static InjectionData FixMeshPositions()
    {
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.General, "mines_meshfixes");
        CreateDefaultTests(data, TR1LevelNames.MINES);

        // Shift the bottom of the motorboat down to fix z-fighting with the water.
        {
            TRMeshEdit edit = new()
            {
                ModelID = (uint)TR1Type.Furniture0,
                MeshIndex = 0,
                VertexEdits = new(),
            };
            data.MeshEdits.Add(edit);

            for (short i = 16; i < 20; i++)
            {
                edit.VertexEdits.Add(CreateVertexShift(i, y: 3));
            }
        }

        {
            TRMeshEdit edit = new()
            {
                ModelID = (uint)TR1Type.Furniture1,
                MeshIndex = 0,
                VertexEdits = new(),
            };
            data.MeshEdits.Add(edit);

            for (short i = 3; i < 6; i++)
            {
                edit.VertexEdits.Add(CreateVertexShift(i, y: 1));
            }

            foreach (short i in new short[] { 0, 2, 3, 5 })
            {
                edit.VertexEdits.Add(CreateVertexShift(i, x: 8));
            }
        }

        return data;
    }
}
