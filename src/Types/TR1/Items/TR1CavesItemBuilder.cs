﻿using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Items;

public class TR1CavesItemBuilder : ItemBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level caves = _control1.Read($@"Resources\{TR1LevelNames.CAVES}");
        InjectionData data = InjectionData.Create(InjectionType.ItemRotation, "caves_itemrots");

        data.ItemEdits = new()
        {
            SetAngle(caves, 54, -32768),
        };

        return new() { data };
    }
}
