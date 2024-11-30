﻿using System.Drawing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Textures;

public class TR1CatTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level cat = _control1.Read($"Resources/{TR1LevelNames.CAT}");
        InjectionData data = InjectionData.Create(TRGameVersion.TR1, InjectionType.TextureFix, "cat_textures");

        data.RoomEdits.AddRange(CreateFillers(cat));
        data.RoomEdits.AddRange(CreateRefacings());
        data.RoomEdits.AddRange(CreateRotations());

        FixTransparentTextures(cat, data);

        return new() { data };
    }

    private static List<TRRoomTextureCreate> CreateFillers(TR1Level cat)
    {
        return new()
        {
            new()
            {
                RoomIndex = 75,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 75,
                SourceIndex = 4,
                Vertices = new()
                {
                    cat.Rooms[75].Mesh.Rectangles[196].Vertices[2],
                    cat.Rooms[75].Mesh.Rectangles[174].Vertices[3],
                    cat.Rooms[75].Mesh.Rectangles[196].Vertices[3],
                }
            },
        };
    }

    private static List<TRRoomTextureReface> CreateRefacings()
    {
        return new()
        {
            new()
            {
                RoomIndex = 50,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 50,
                SourceIndex = 56,
                TargetIndex = 81,
            },
            new()
            {
                RoomIndex = 71,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 76,
                SourceIndex = 81,
                TargetIndex = 76,
            },
            new()
            {
                RoomIndex = 87,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 87,
                SourceIndex = 47,
                TargetIndex = 100,
            }
        };
    }

    private static List<TRRoomTextureRotate> CreateRotations()
    {
        return new()
        {
            Rotate(70, TRMeshFaceType.TexturedQuad, 98, 3),
            Rotate(78, TRMeshFaceType.TexturedQuad, 112, 3),
            Rotate(71, TRMeshFaceType.TexturedQuad, 73, 3),
            Rotate(76, TRMeshFaceType.TexturedQuad, 78, 3),
            Rotate(96, TRMeshFaceType.TexturedQuad, 5, 3),
            Rotate(96, TRMeshFaceType.TexturedQuad, 73, 3),
            Rotate(96, TRMeshFaceType.TexturedQuad, 27, 1),
        };
    }

    private static void FixTransparentTextures(TR1Level cat, InjectionData data)
    {
        TR1CommonTextureBuilder.FixTransparentPixels(cat, data,
            cat.Rooms[2].Mesh.Rectangles[128], Color.FromArgb(188, 140, 64));
        TR1CommonTextureBuilder.FixTransparentPixels(cat, data,
            cat.Rooms[7].Mesh.Rectangles[19], Color.FromArgb(188, 140, 64));
    }
}
