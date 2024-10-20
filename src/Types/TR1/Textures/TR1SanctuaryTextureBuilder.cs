using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Textures;

public class TR1SanctuaryTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level sanctuary = _control1.Read($@"Resources\{TR1LevelNames.SANCTUARY}");
        InjectionData data = InjectionData.Create(InjectionType.TextureFix, "sanctuary_textures");

        data.RoomEdits.AddRange(CreateFillers(sanctuary));
        data.RoomEdits.AddRange(CreateRefacings());
        data.RoomEdits.AddRange(CreateVertexShifts(sanctuary));

        return new() { data };
    }

    private static List<TRRoomTextureCreate> CreateFillers(TR1Level sanctuary)
    {
        return new()
        {
            new()
            {
                RoomIndex = 0,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 1,
                SourceIndex = 29,
                Vertices = new()
                {
                    sanctuary.Rooms[0].Mesh.Rectangles[115].Vertices[1],
                    sanctuary.Rooms[0].Mesh.Rectangles[116].Vertices[1],
                    sanctuary.Rooms[0].Mesh.Rectangles[116].Vertices[0],
                }
            },
            new()
            {
                RoomIndex = 54,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 22,
                SourceIndex = 0,
                Vertices = new()
                {
                    sanctuary.Rooms[54].Mesh.Rectangles[4].Vertices[1],
                    sanctuary.Rooms[54].Mesh.Rectangles[4].Vertices[0],
                    sanctuary.Rooms[54].Mesh.Rectangles[3].Vertices[1],
                }
            },
            new()
            {
                RoomIndex = 53,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 21,
                SourceIndex = 15,
                Vertices = new()
                {
                    sanctuary.Rooms[53].Mesh.Rectangles[48].Vertices[3],
                    sanctuary.Rooms[53].Mesh.Rectangles[77].Vertices[2],
                    sanctuary.Rooms[53].Mesh.Rectangles[51].Vertices[0],
                }
            },
            new()
            {
                RoomIndex = 53,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 53,
                SourceIndex = 4,
                Vertices = new()
                {
                    sanctuary.Rooms[53].Mesh.Rectangles[74].Vertices[0],
                    sanctuary.Rooms[53].Mesh.Rectangles[74].Vertices[3],
                    sanctuary.Rooms[53].Mesh.Rectangles[48].Vertices[0],
                }
            },
            new()
            {
                RoomIndex = 53,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 53,
                SourceIndex = 4,
                Vertices = new()
                {
                    sanctuary.Rooms[53].Mesh.Rectangles[48].Vertices[0],
                    sanctuary.Rooms[53].Mesh.Rectangles[47].Vertices[0],
                    sanctuary.Rooms[53].Mesh.Rectangles[47].Vertices[3],
                }
            },
            new()
            {
                RoomIndex = 21,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 21,
                SourceIndex = 42,
                Vertices = new()
                {
                    sanctuary.Rooms[21].Mesh.Rectangles[205].Vertices[0],
                    sanctuary.Rooms[21].Mesh.Rectangles[173].Vertices[3],
                    sanctuary.Rooms[21].Mesh.Rectangles[173].Vertices[2],
                }
            },
            new()
            {
                RoomIndex = 53,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 21,
                SourceIndex = 42,
                Vertices = new()
                {
                    sanctuary.Rooms[53].Mesh.Rectangles[129].Vertices[0],
                    sanctuary.Rooms[53].Mesh.Rectangles[111].Vertices[3],
                    sanctuary.Rooms[53].Mesh.Rectangles[111].Vertices[2],
                }
            },
            new()
            {
                RoomIndex = 21,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 21,
                SourceIndex = 20,
                Vertices = new()
                {
                    sanctuary.Rooms[21].Mesh.Rectangles[25].Vertices[1],
                    sanctuary.Rooms[21].Mesh.Rectangles[20].Vertices[1],
                    sanctuary.Rooms[21].Mesh.Triangles[2].Vertices[2],
                    sanctuary.Rooms[21].Mesh.Rectangles[25].Vertices[2],
                }
            },
            new()
            {
                RoomIndex = 21,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 21,
                SourceIndex = 64,
                Vertices = new()
                {
                    sanctuary.Rooms[21].Mesh.Rectangles[62].Vertices[2],
                    sanctuary.Rooms[21].Mesh.Rectangles[64].Vertices[3],
                    sanctuary.Rooms[21].Mesh.Rectangles[64].Vertices[2],
                    sanctuary.Rooms[21].Mesh.Rectangles[62].Vertices[3],
                }
            },
            new()
            {
                RoomIndex = 53,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 21,
                SourceIndex = 20,
                Vertices = new()
                {
                    sanctuary.Rooms[53].Mesh.Rectangles[19].Vertices[1],
                    sanctuary.Rooms[53].Mesh.Rectangles[16].Vertices[1],
                    sanctuary.Rooms[53].Mesh.Triangles[2].Vertices[2],
                    sanctuary.Rooms[53].Mesh.Rectangles[19].Vertices[2],
                }
            },
            new()
            {
                RoomIndex = 53,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 21,
                SourceIndex = 64,
                Vertices = new()
                {
                    sanctuary.Rooms[53].Mesh.Rectangles[42].Vertices[2],
                    sanctuary.Rooms[53].Mesh.Rectangles[43].Vertices[3],
                    sanctuary.Rooms[53].Mesh.Rectangles[43].Vertices[2],
                    sanctuary.Rooms[53].Mesh.Rectangles[42].Vertices[3],
                }
            },
            new()
            {
                RoomIndex = 21,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 21,
                SourceIndex = 64,
                Vertices = new()
                {
                    sanctuary.Rooms[21].Mesh.Rectangles[350].Vertices[0],
                    sanctuary.Rooms[21].Mesh.Rectangles[382].Vertices[3],
                    sanctuary.Rooms[21].Mesh.Rectangles[384].Vertices[1],
                    sanctuary.Rooms[21].Mesh.Rectangles[350].Vertices[1],
                }
            },
            new()
            {
                RoomIndex = 53,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 21,
                SourceIndex = 64,
                Vertices = new()
                {
                    sanctuary.Rooms[53].Mesh.Rectangles[210].Vertices[0],
                    sanctuary.Rooms[53].Mesh.Rectangles[228].Vertices[3],
                    sanctuary.Rooms[53].Mesh.Rectangles[229].Vertices[1],
                    sanctuary.Rooms[53].Mesh.Rectangles[210].Vertices[1],
                }
            },
            new()
            {
                RoomIndex = 21,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 21,
                SourceIndex = 64,
                Vertices = new()
                {
                    sanctuary.Rooms[21].Mesh.Rectangles[382].Vertices[3],
                    sanctuary.Rooms[21].Mesh.Rectangles[350].Vertices[0],
                    sanctuary.Rooms[21].Mesh.Rectangles[347].Vertices[1],
                    sanctuary.Rooms[21].Mesh.Rectangles[347].Vertices[0],
                }
            },
            new()
            {
                RoomIndex = 53,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 21,
                SourceIndex = 64,
                Vertices = new()
                {
                    sanctuary.Rooms[53].Mesh.Rectangles[228].Vertices[3],
                    sanctuary.Rooms[53].Mesh.Rectangles[210].Vertices[0],
                    sanctuary.Rooms[53].Mesh.Rectangles[209].Vertices[1],
                    sanctuary.Rooms[53].Mesh.Rectangles[209].Vertices[0],
                }
            },
            new()
            {
                RoomIndex = 21,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 21,
                SourceIndex = 64,
                Vertices = new()
                {
                    sanctuary.Rooms[21].Mesh.Rectangles[411].Vertices[0],
                    sanctuary.Rooms[21].Mesh.Rectangles[411].Vertices[3],
                    sanctuary.Rooms[21].Mesh.Rectangles[414].Vertices[0],
                    sanctuary.Rooms[21].Mesh.Rectangles[414].Vertices[3],
                }
            },
            new()
            {
                RoomIndex = 53,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 21,
                SourceIndex = 64,
                Vertices = new()
                {
                    sanctuary.Rooms[53].Mesh.Rectangles[244].Vertices[0],
                    sanctuary.Rooms[53].Mesh.Rectangles[244].Vertices[3],
                    sanctuary.Rooms[53].Mesh.Rectangles[245].Vertices[0],
                    sanctuary.Rooms[53].Mesh.Rectangles[245].Vertices[3],
                }
            },
            new()
            {
                RoomIndex = 21,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 21,
                SourceIndex = 64,
                Vertices = new()
                {
                    sanctuary.Rooms[21].Mesh.Rectangles[441].Vertices[0],
                    sanctuary.Rooms[21].Mesh.Rectangles[411].Vertices[3],
                    sanctuary.Rooms[21].Mesh.Rectangles[472].Vertices[1],
                    sanctuary.Rooms[21].Mesh.Rectangles[472].Vertices[0],
                }
            },
            new()
            {
                RoomIndex = 53,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 21,
                SourceIndex = 64,
                Vertices = new()
                {
                    sanctuary.Rooms[53].Mesh.Rectangles[257].Vertices[0],
                    sanctuary.Rooms[53].Mesh.Rectangles[244].Vertices[3],
                    sanctuary.Rooms[53].Mesh.Rectangles[274].Vertices[1],
                    sanctuary.Rooms[53].Mesh.Rectangles[274].Vertices[0],
                }
            },
            new()
            {
                RoomIndex = 21,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 21,
                SourceIndex = 64,
                Vertices = new()
                {
                    sanctuary.Rooms[21].Mesh.Rectangles[411].Vertices[3],
                    sanctuary.Rooms[21].Mesh.Rectangles[411].Vertices[2],
                    sanctuary.Rooms[21].Mesh.Rectangles[469].Vertices[1],
                    sanctuary.Rooms[21].Mesh.Rectangles[472].Vertices[1],
                }
            },
            new()
            {
                RoomIndex = 53,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 21,
                SourceIndex = 64,
                Vertices = new()
                {
                    sanctuary.Rooms[53].Mesh.Rectangles[244].Vertices[3],
                    sanctuary.Rooms[53].Mesh.Rectangles[244].Vertices[2],
                    sanctuary.Rooms[53].Mesh.Rectangles[273].Vertices[1],
                    sanctuary.Rooms[53].Mesh.Rectangles[274].Vertices[1],
                }
            },
            new()
            {
                RoomIndex = 21,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 21,
                SourceIndex = 64,
                Vertices = new()
                {
                    sanctuary.Rooms[21].Mesh.Rectangles[504].Vertices[2],
                    sanctuary.Rooms[21].Mesh.Rectangles[504].Vertices[1],
                    sanctuary.Rooms[21].Mesh.Rectangles[472].Vertices[1],
                    sanctuary.Rooms[21].Mesh.Rectangles[469].Vertices[1],
                }
            },
            new()
            {
                RoomIndex = 53,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 21,
                SourceIndex = 64,
                Vertices = new()
                {
                    sanctuary.Rooms[53].Mesh.Rectangles[292].Vertices[2],
                    sanctuary.Rooms[53].Mesh.Rectangles[292].Vertices[1],
                    sanctuary.Rooms[53].Mesh.Rectangles[274].Vertices[1],
                    sanctuary.Rooms[53].Mesh.Rectangles[273].Vertices[1],
                }
            },
            new()
            {
                RoomIndex = 21,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 21,
                SourceIndex = 64,
                Vertices = new()
                {
                    sanctuary.Rooms[21].Mesh.Rectangles[411].Vertices[2],
                    sanctuary.Rooms[21].Mesh.Rectangles[411].Vertices[1],
                    sanctuary.Rooms[21].Mesh.Rectangles[408].Vertices[2],
                    sanctuary.Rooms[21].Mesh.Rectangles[408].Vertices[1],
                }
            },
            new()
            {
                RoomIndex = 53,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 21,
                SourceIndex = 64,
                Vertices = new()
                {
                    sanctuary.Rooms[53].Mesh.Rectangles[244].Vertices[2],
                    sanctuary.Rooms[53].Mesh.Rectangles[244].Vertices[1],
                    sanctuary.Rooms[53].Mesh.Rectangles[243].Vertices[2],
                    sanctuary.Rooms[53].Mesh.Rectangles[243].Vertices[1],
                }
            },
            new()
            {
                RoomIndex = 21,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 21,
                SourceIndex = 64,
                Vertices = new()
                {
                    sanctuary.Rooms[21].Mesh.Rectangles[408].Vertices[1],
                    sanctuary.Rooms[21].Mesh.Rectangles[469].Vertices[2],
                    sanctuary.Rooms[21].Mesh.Rectangles[469].Vertices[1],
                    sanctuary.Rooms[21].Mesh.Rectangles[411].Vertices[2],
                }
            },
            new()
            {
                RoomIndex = 53,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 21,
                SourceIndex = 64,
                Vertices = new()
                {
                    sanctuary.Rooms[53].Mesh.Rectangles[243].Vertices[1],
                    sanctuary.Rooms[53].Mesh.Rectangles[273].Vertices[2],
                    sanctuary.Rooms[53].Mesh.Rectangles[273].Vertices[1],
                    sanctuary.Rooms[53].Mesh.Rectangles[244].Vertices[2],
                }
            },
            new()
            {
                RoomIndex = 21,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 21,
                SourceIndex = 64,
                Vertices = new()
                {
                    sanctuary.Rooms[21].Mesh.Rectangles[406].Vertices[0],
                    sanctuary.Rooms[21].Mesh.Rectangles[406].Vertices[3],
                    sanctuary.Rooms[21].Mesh.Rectangles[465].Vertices[1],
                    sanctuary.Rooms[21].Mesh.Rectangles[465].Vertices[0],
                }
            },
            new()
            {
                RoomIndex = 53,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 21,
                SourceIndex = 64,
                Vertices = new()
                {
                    sanctuary.Rooms[53].Mesh.Rectangles[242].Vertices[0],
                    sanctuary.Rooms[53].Mesh.Rectangles[242].Vertices[3],
                    sanctuary.Rooms[53].Mesh.Rectangles[271].Vertices[1],
                    sanctuary.Rooms[53].Mesh.Rectangles[271].Vertices[0],
                }
            },
            new()
            {
                RoomIndex = 21,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 21,
                SourceIndex = 64,
                Vertices = new()
                {
                    sanctuary.Rooms[21].Mesh.Rectangles[467].Vertices[0],
                    sanctuary.Rooms[21].Mesh.Rectangles[408].Vertices[1],
                    sanctuary.Rooms[21].Mesh.Rectangles[408].Vertices[0],
                    sanctuary.Rooms[21].Mesh.Rectangles[467].Vertices[1],
                }
            },
            new()
            {
                RoomIndex = 53,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 21,
                SourceIndex = 64,
                Vertices = new()
                {
                    sanctuary.Rooms[53].Mesh.Rectangles[272].Vertices[0],
                    sanctuary.Rooms[53].Mesh.Rectangles[243].Vertices[1],
                    sanctuary.Rooms[53].Mesh.Rectangles[243].Vertices[0],
                    sanctuary.Rooms[53].Mesh.Rectangles[272].Vertices[1],
                }
            },
            new()
            {
                RoomIndex = 21,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 21,
                SourceIndex = 42,
                Vertices = new()
                {
                    sanctuary.Rooms[21].Mesh.Rectangles[221].Vertices[0],
                    sanctuary.Rooms[21].Mesh.Rectangles[223].Vertices[2],
                    sanctuary.Rooms[21].Mesh.Rectangles[223].Vertices[1],
                }
            },
            new()
            {
                RoomIndex = 53,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 21,
                SourceIndex = 42,
                Vertices = new()
                {
                    sanctuary.Rooms[53].Mesh.Rectangles[139].Vertices[0],
                    sanctuary.Rooms[53].Mesh.Rectangles[141].Vertices[2],
                    sanctuary.Rooms[53].Mesh.Rectangles[141].Vertices[1],
                }
            },
            new()
            {
                RoomIndex = 21,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 21,
                SourceIndex = 64,
                Vertices = new()
                {
                    sanctuary.Rooms[21].Mesh.Rectangles[274].Vertices[1],
                    sanctuary.Rooms[21].Mesh.Rectangles[274].Vertices[0],
                    sanctuary.Rooms[21].Mesh.Rectangles[238].Vertices[2],
                    sanctuary.Rooms[21].Mesh.Rectangles[306].Vertices[2],
                }
            },
            new()
            {
                RoomIndex = 53,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 21,
                SourceIndex = 64,
                Vertices = new()
                {
                    sanctuary.Rooms[53].Mesh.Rectangles[169].Vertices[1],
                    sanctuary.Rooms[53].Mesh.Rectangles[169].Vertices[0],
                    sanctuary.Rooms[53].Mesh.Rectangles[149].Vertices[2],
                    sanctuary.Rooms[53].Mesh.Rectangles[187].Vertices[2],
                }
            },
            new()
            {
                RoomIndex = 21,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 21,
                SourceIndex = 64,
                Vertices = new()
                {
                    sanctuary.Rooms[21].Mesh.Rectangles[304].Vertices[0],
                    sanctuary.Rooms[21].Mesh.Rectangles[304].Vertices[3],
                    sanctuary.Rooms[21].Mesh.Rectangles[236].Vertices[3],
                    sanctuary.Rooms[21].Mesh.Rectangles[236].Vertices[2],
                }
            },
            new()
            {
                RoomIndex = 53,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 21,
                SourceIndex = 64,
                Vertices = new()
                {
                    sanctuary.Rooms[53].Mesh.Rectangles[186].Vertices[0],
                    sanctuary.Rooms[53].Mesh.Rectangles[186].Vertices[3],
                    sanctuary.Rooms[53].Mesh.Rectangles[148].Vertices[3],
                    sanctuary.Rooms[53].Mesh.Rectangles[148].Vertices[2],
                }
            },
            new()
            {
                RoomIndex = 21,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 21,
                SourceIndex = 64,
                Vertices = new()
                {
                    sanctuary.Rooms[21].Mesh.Rectangles[197].Vertices[0],
                    sanctuary.Rooms[21].Mesh.Rectangles[197].Vertices[3],
                    sanctuary.Rooms[21].Mesh.Rectangles[270].Vertices[1],
                    sanctuary.Rooms[21].Mesh.Rectangles[270].Vertices[0],
                }
            },
            new()
            {
                RoomIndex = 53,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 21,
                SourceIndex = 64,
                Vertices = new()
                {
                    sanctuary.Rooms[53].Mesh.Rectangles[124].Vertices[0],
                    sanctuary.Rooms[53].Mesh.Rectangles[124].Vertices[3],
                    sanctuary.Rooms[53].Mesh.Rectangles[168].Vertices[1],
                    sanctuary.Rooms[53].Mesh.Rectangles[168].Vertices[0],
                }
            },
            new()
            {
                RoomIndex = 21,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 21,
                SourceIndex = 64,
                Vertices = new()
                {
                    sanctuary.Rooms[21].Mesh.Rectangles[197].Vertices[3],
                    sanctuary.Rooms[21].Mesh.Rectangles[197].Vertices[2],
                    sanctuary.Rooms[21].Mesh.Rectangles[194].Vertices[3],
                    sanctuary.Rooms[21].Mesh.Rectangles[194].Vertices[2],
                }
            },
            new()
            {
                RoomIndex = 53,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 21,
                SourceIndex = 64,
                Vertices = new()
                {
                    sanctuary.Rooms[53].Mesh.Rectangles[124].Vertices[3],
                    sanctuary.Rooms[53].Mesh.Rectangles[124].Vertices[2],
                    sanctuary.Rooms[53].Mesh.Rectangles[123].Vertices[3],
                    sanctuary.Rooms[53].Mesh.Rectangles[123].Vertices[2],
                }
            },
            new()
            {
                RoomIndex = 21,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 21,
                SourceIndex = 64,
                Vertices = new()
                {
                    sanctuary.Rooms[21].Mesh.Rectangles[20].Vertices[0],
                    sanctuary.Rooms[21].Mesh.Rectangles[20].Vertices[3],
                    sanctuary.Rooms[21].Mesh.Rectangles[98].Vertices[1],
                    sanctuary.Rooms[21].Mesh.Rectangles[98].Vertices[0],
                }
            },
            new()
            {
                RoomIndex = 53,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 21,
                SourceIndex = 64,
                Vertices = new()
                {
                    sanctuary.Rooms[53].Mesh.Rectangles[16].Vertices[0],
                    sanctuary.Rooms[53].Mesh.Rectangles[16].Vertices[3],
                    sanctuary.Rooms[53].Mesh.Rectangles[65].Vertices[1],
                    sanctuary.Rooms[53].Mesh.Rectangles[65].Vertices[0],
                }
            },
            new()
            {
                RoomIndex = 21,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 21,
                SourceIndex = 64,
                Vertices = new()
                {
                    sanctuary.Rooms[21].Mesh.Rectangles[94].Vertices[1],
                    sanctuary.Rooms[21].Mesh.Rectangles[94].Vertices[0],
                    sanctuary.Rooms[21].Mesh.Rectangles[99].Vertices[1],
                    sanctuary.Rooms[21].Mesh.Rectangles[99].Vertices[0],
                }
            },
            new()
            {
                RoomIndex = 53,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 21,
                SourceIndex = 64,
                Vertices = new()
                {
                    sanctuary.Rooms[53].Mesh.Rectangles[63].Vertices[1],
                    sanctuary.Rooms[53].Mesh.Rectangles[63].Vertices[0],
                    sanctuary.Rooms[53].Mesh.Rectangles[66].Vertices[1],
                    sanctuary.Rooms[53].Mesh.Rectangles[66].Vertices[0],
                }
            },
            new()
            {
                RoomIndex = 11,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 11,
                SourceIndex = 9,
                Vertices = new()
                {
                    sanctuary.Rooms[11].Mesh.Rectangles[49].Vertices[0],
                    sanctuary.Rooms[11].Mesh.Rectangles[41].Vertices[0],
                    sanctuary.Rooms[11].Mesh.Rectangles[41].Vertices[3],
                }
            },
            new()
            {
                RoomIndex = 52,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 52,
                SourceIndex = 0,
                Vertices = new()
                {
                    sanctuary.Rooms[52].Mesh.Rectangles[153].Vertices[1],
                    sanctuary.Rooms[52].Mesh.Rectangles[124].Vertices[2],
                    sanctuary.Rooms[52].Mesh.Rectangles[124].Vertices[1],
                }
            },
            new()
            {
                RoomIndex = 52,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = GetSource(sanctuary, TRMeshFaceType.TexturedQuad, 62).Room,
                SourceIndex = GetSource(sanctuary, TRMeshFaceType.TexturedQuad, 62).Face,
                Vertices = new()
                {
                    sanctuary.Rooms[52].Mesh.Rectangles[156].Vertices[2],
                    sanctuary.Rooms[52].Mesh.Rectangles[156].Vertices[1],
                    sanctuary.Rooms[52].Mesh.Rectangles[128].Vertices[3],
                    sanctuary.Rooms[52].Mesh.Rectangles[128].Vertices[2],
                }
            },
            new()
            {
                RoomIndex = 52,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = GetSource(sanctuary, TRMeshFaceType.TexturedQuad, 52).Room,
                SourceIndex = GetSource(sanctuary, TRMeshFaceType.TexturedQuad, 52).Face,
                Vertices = new()
                {
                    sanctuary.Rooms[52].Mesh.Rectangles[158].Vertices[2],
                    sanctuary.Rooms[52].Mesh.Rectangles[158].Vertices[1],
                    sanctuary.Rooms[52].Mesh.Rectangles[131].Vertices[1],
                    sanctuary.Rooms[52].Mesh.Rectangles[131].Vertices[0],
                }
            },
            new()
            {
                RoomIndex = 52,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = GetSource(sanctuary, TRMeshFaceType.TexturedTriangle, 42).Room,
                SourceIndex = GetSource(sanctuary, TRMeshFaceType.TexturedTriangle, 42).Room,
                Vertices = new()
                {
                    sanctuary.Rooms[52].Mesh.Rectangles[180].Vertices[2],
                    sanctuary.Rooms[52].Mesh.Rectangles[156].Vertices[0],
                    sanctuary.Rooms[52].Mesh.Rectangles[156].Vertices[3],
                }
            },
            new()
            {
                RoomIndex = 52,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = GetSource(sanctuary, TRMeshFaceType.TexturedQuad, 37).Room,
                SourceIndex = GetSource(sanctuary, TRMeshFaceType.TexturedQuad, 37).Face,
                Vertices = new()
                {
                    sanctuary.Rooms[52].Mesh.Rectangles[182].Vertices[2],
                    sanctuary.Rooms[52].Mesh.Rectangles[182].Vertices[1],
                    sanctuary.Rooms[52].Mesh.Rectangles[158].Vertices[0],
                    sanctuary.Rooms[52].Mesh.Rectangles[158].Vertices[3],
                }
            }
        };
    }

    private static List<TRRoomTextureReface> CreateRefacings()
    {
        return new()
        {
            new()
            {
                RoomIndex = 22,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 22,
                SourceIndex = 295,
                TargetIndex = 272,
            }
        };
    }

    private static List<TRRoomVertexMove> CreateVertexShifts(TR1Level sanctuary)
    {
        return new()
        {
            new()
            {
                RoomIndex = 54,
                VertexIndex = sanctuary.Rooms[54].Mesh.Rectangles[4].Vertices[1],
                VertexChange = new() { Y = -256 }
            },
            new()
            {
                RoomIndex = 54,
                VertexIndex = sanctuary.Rooms[54].Mesh.Rectangles[4].Vertices[0],
                VertexChange = new() { Y = -256 }
            },
        };
    }
}
