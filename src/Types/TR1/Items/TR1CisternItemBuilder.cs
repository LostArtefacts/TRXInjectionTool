﻿using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Items;

public class TR1CisternItemBuilder : ItemBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level cistern = _control1.Read($@"Resources\{TR1LevelNames.CISTERN}");
        InjectionData data = InjectionData.Create(InjectionType.ItemRotation, "cistern_itemrots");

        data.ItemEdits = new()
        {
            SetAngle(cistern, 24, -16384),
            SetAngle(cistern, 46, -16384),
            SetAngle(cistern, 100, -16384),
        };

        return new() { data };
    }
}