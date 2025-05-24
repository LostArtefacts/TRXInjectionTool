using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Textures;

public class TR2KingdomTextureBuilder : TextureBuilder
{
    public override string ID => "kingdom_textures";

    public override List<InjectionData> Build()
    {
        TR2Level kingdom = _control2.Read($"Resources/{TR2LevelNames.KINGDOM}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.TextureFix, ID);
        CreateDefaultTests(data, TR2LevelNames.KINGDOM);

        data.FloorEdits.Add(new()
        {
            RoomIndex = 63,
            Fixes = new()
            {
                new FDRoomProperties
                {
                    Flags = kingdom.Rooms[63].Flags | TRRoomFlag.Skybox,
                }
            }
        });

        return new() { data };
    }
}
