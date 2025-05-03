using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Misc;

public  class TR1PickupSpriteBuilder : InjectionBuilder
{
    private static readonly Dictionary<TR1Type, TRSpriteAlignment> _alignmentFixes = new()
    {
        [TR1Type.Pistols_S_P]
            = new() { Left = -144, Top = -84, Right = 144, Bottom = 28, },
        [TR1Type.Shotgun_S_P]
            = new() { Left = -240, Top = -96, Right = 240, Bottom = 0, },
        [TR1Type.Magnums_S_P]
            = new() { Left = -152, Top = -130, Right = 152, Bottom = 14, },
        [TR1Type.Uzis_S_P]
            = new() { Left = -128, Top = -88, Right = 128, Bottom = 24, },
        [TR1Type.ShotgunAmmo_S_P]
            = new() { Left = -136, Top = -96, Right = 136, Bottom = 0, },
        [TR1Type.MagnumAmmo_S_P]
            = new() { Left = -120, Top = -104, Right = 120, Bottom = 8, },
        [TR1Type.SmallMed_S_P]
            = new() { Left = -88, Top = -88, Right = 88, Bottom = 22, },
    };

    public override List<InjectionData> Build()
    {
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.General, "sprite_alignment");
        data.SpriteEdits.AddRange(_alignmentFixes.Select(a =>
        {
            return new TRSpriteEdit
            {
                ID = (int)a.Key,
                Alignment = a.Value,
            };
        }));
        return new() { data };
    }
}
