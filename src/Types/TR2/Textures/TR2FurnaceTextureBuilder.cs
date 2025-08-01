﻿using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR2.Textures;

public class TR2FurnaceTextureBuilder : TextureBuilder
{
    public override string ID => "furnace_textures";

    public override List<InjectionData> Build()
    {
        TR2Level furnace = _control2.Read($"Resources/{TR2LevelNames.FURNACE}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR2, InjectionType.TextureFix, ID);
        CreateDefaultTests(data, TR2LevelNames.FURNACE);

        FixWolfTransparency(furnace, data);

        data.RoomEdits.AddRange(CreateVertexShifts(furnace));
        data.RoomEdits.AddRange(CreateShifts(furnace));
        data.RoomEdits.AddRange(CreateFillers(furnace));
        data.RoomEdits.AddRange(CreateRefacings(furnace));

        FixPassport(furnace, data);
        FixPushButton(data);

        return new() { data };
    }

    private static List<TRRoomVertexMove> CreateVertexShifts(TR2Level level)
    {
        return new()
        {
            new()
            {
                RoomIndex = 117,
                VertexIndex = level.Rooms[117].Mesh.Rectangles[16].Vertices[2],
                VertexChange = new() { Y = -512 },
            },
            new()
            {
                RoomIndex = 117,
                VertexIndex = level.Rooms[117].Mesh.Rectangles[16].Vertices[3],
                VertexChange = new() { Y = -512 },
            },
        };
    }

    private static List<TRRoomTextureMove> CreateShifts(TR2Level level)
    {
        return new()
        {
            new()
            {
                RoomIndex = 10,
                FaceType = TRMeshFaceType.TexturedQuad,
                TargetIndex = 35,
                VertexRemap = new()
                {
                    new()
                    {
                        Index = 2,
                        NewVertexIndex = level.Rooms[10].Mesh.Rectangles[39].Vertices[0],
                    },
                    new()
                    {
                        Index = 3,
                        NewVertexIndex = level.Rooms[10].Mesh.Rectangles[32].Vertices[1],
                    }
                }
            },
        };
    }

    private static List<TRRoomTextureCreate> CreateFillers(TR2Level level)
    {
        return new()
        {
            new()
            {
                RoomIndex = 10,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 10,
                SourceIndex = 39,
                Vertices = new()
                {
                    level.Rooms[10].Mesh.Rectangles[32].Vertices[1],
                    level.Rooms[10].Mesh.Rectangles[39].Vertices[0],
                    level.Rooms[10].Mesh.Rectangles[39].Vertices[3],
                    level.Rooms[10].Mesh.Rectangles[32].Vertices[2],
                }
            },
            new()
            {
                RoomIndex = 117,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 117,
                SourceIndex = 4,
                Vertices = new()
                {
                    level.Rooms[117].Mesh.Rectangles[16].Vertices[0],
                    level.Rooms[117].Mesh.Rectangles[16].Vertices[3],
                    level.Rooms[117].Mesh.Rectangles[12].Vertices[2],
                }
            },
        };
    }

    private static List<TRRoomTextureReface> CreateRefacings(TR2Level level)
    {
        return new()
        {
            Reface(level, 97, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1412, 32),
            Reface(level, 97, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedTriangle, 1713, 38),
            Reface(level, 97, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1412, 134),
            Reface(level, 99, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedTriangle, 1420, 17),
            Reface(level, 99, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedTriangle, 1420, 20),
            Reface(level, 99, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1412, 130),
            Reface(level, 101, TRMeshFaceType.TexturedQuad, TRMeshFaceType.TexturedQuad, 1412, 11),
            Reface(level, 105, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedTriangle, 1420, 8),
            Reface(level, 108, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedTriangle, 1420, 7),
            Reface(level, 120, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedTriangle, 1420, 0),
            Reface(level, 124, TRMeshFaceType.TexturedTriangle, TRMeshFaceType.TexturedTriangle, 1750, 5),
        };
    }
}
