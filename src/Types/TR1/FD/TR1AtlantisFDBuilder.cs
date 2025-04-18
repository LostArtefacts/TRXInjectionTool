﻿using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.FD;

public class TR1AtlantisFDBuilder : FDBuilder
{
    public override List<InjectionData> Build()
    {
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.FDFix, "atlantis_fd");
        CreateDefaultTests(data, TR1LevelNames.ATLANTIS);
        data.FloorEdits = new()
        {
            MakeMusicOneShot(59, 1, 1),
        };

        return new() { data };
    }
}
