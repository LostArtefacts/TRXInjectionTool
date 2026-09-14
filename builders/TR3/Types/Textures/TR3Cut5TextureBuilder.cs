using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;
using TRXInjectionTool.Util;

namespace TRXInjectionTool.Types.TR3.Textures;

public class TR3Cut5TextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.ALDWYCH_CUT}");
        var data = CreateBaseData();
        CreateDefaultTests(data, $"TR3/{TR3LevelNames.ALDWYCH_CUT}");

        data.RoomEdits.AddRange(FixPunkDoor(level));

        return [data];
    }

    private static IEnumerable<TRRoomTextureEdit> FixPunkDoor(TR3Level level)
    {
        // The punk walks through a "door" in OG. Alter the room mesh so it looks
        // more believable. The fake door is replaced with a real one, and is activated
        // by default.
        const short roomIdx = 0;
        var room = level.Rooms[roomIdx];
        var vtxPos = new List<ushort>();

        TRRoomVertexCreate MakeVertex(int faceIdx, int vertIdx, short zShift)
        {
            var vtx = room.Mesh.Vertices[room.Mesh.Rectangles[faceIdx].Vertices[vertIdx]];
            vtx.AlterColour(0.5f);
            vtxPos.Add((ushort)room.Mesh.Vertices.Count);
            var vertex = CreateVertex(roomIdx, room, vtx, shift: 0);
            vertex.Vertex.Vertex.Z += zShift;
            return vertex;
        }

        {
            // West wall 1
            yield return MakeVertex(29, 1, 1024);
            yield return MakeVertex(29, 2, 1024);
            yield return CreateFace(roomIdx, 0, 29, TRMeshFaceType.TexturedQuad,
            [
                room.Mesh.Rectangles[29].Vertices[1],
                vtxPos[0], vtxPos[1],
                room.Mesh.Rectangles[29].Vertices[2],
            ]);
        }

        {
            // East wall 1
            yield return MakeVertex(52, 0, 1024);
            yield return MakeVertex(52, 3, 1024);
            yield return CreateFace(roomIdx, 0, 52, TRMeshFaceType.TexturedQuad,
            [
                vtxPos[2],
                room.Mesh.Rectangles[52].Vertices[0],
                room.Mesh.Rectangles[52].Vertices[3],
                vtxPos[3],
            ]);
        }

        {
            // West wall 2
            yield return MakeVertex(29, 1, 2048);
            yield return MakeVertex(29, 2, 2048);
            yield return CreateFace(roomIdx, 0, 29, TRMeshFaceType.TexturedQuad,
            [
                vtxPos[0], vtxPos[4], vtxPos[5], vtxPos[1],
            ]);
        }

        {
            // East wall 2
            yield return MakeVertex(52, 0, 2048);
            yield return MakeVertex(52, 3, 2048);
            yield return CreateFace(roomIdx, 0, 52, TRMeshFaceType.TexturedQuad,
            [
                vtxPos[6], vtxPos[2], vtxPos[3], vtxPos[7],
            ]);
        }

        // Back wall
        yield return CreateFace(roomIdx, 0, 29, TRMeshFaceType.TexturedQuad,
        [
            vtxPos[4], vtxPos[6], vtxPos[7], vtxPos[5],
        ]);

        {
            // Floor 1
            yield return new TRRoomTextureMove
            {
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 30,
                VertexRemap =
                [
                    new() { Index = 0, NewVertexIndex = vtxPos[1] },
                    new() { Index = 1, NewVertexIndex = vtxPos[3] },
                ],
            };

            var floorTex = room.Mesh.Rectangles[28].Texture;
            yield return Reface(level, 0, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, floorTex, 30);
            yield return Rotate(0, TRMeshFaceType.TexturedQuad, 30, 2);

            // Floor 2
            yield return CreateFace(roomIdx, 0, 28, TRMeshFaceType.TexturedQuad,
            [
                vtxPos[3], vtxPos[1], vtxPos[5], vtxPos[7],
            ]);
        }

        {
            // Ceiling 1
            yield return CreateFace(roomIdx, 0, 29, TRMeshFaceType.TexturedQuad,
            [
                room.Mesh.Rectangles[30].Vertices[0],
                room.Mesh.Rectangles[30].Vertices[1],
                vtxPos[2], vtxPos[0],
            ]);

            // Ceiling 2
            yield return CreateFace(roomIdx, 0, 29, TRMeshFaceType.TexturedQuad,
            [
                vtxPos[0], vtxPos[2], vtxPos[6], vtxPos[4],
            ]);
        }
    }

    private static InjectionData CreateBaseData()
    {
        var level = _control3.Read($"Resources/TR3/{TR3LevelNames.LUDS}");
        CreateModelLevel(level, TR3Type.Door2);
        level.SoundEffects.Clear();

        var door = level.Models[TR3Type.Door2];
        door.Animations[3].Frames = [door.Animations[3].Frames[^1]];
        door.Animations[3].FrameEnd = 0;
        door.Animations[3].Commands.Clear();

        var data = InjectionData.Create(level, InjectionType.TextureFix, "cut5_textures");
        data.FloorEdits.Add(new()
        {
            Fixes = [new FDTrigItem
            {
                Item = new()
                {
                    TypeID = (TR1Type)(TR3Type.Door2),
                    X = 88576,
                    Y = -17920,
                    Z = 50688,
                    Intensity = -1,
                    Angle = -32768,
                    Flags = 0x3E00,
                },
            }],
        });

        return data;
    }
}
