﻿using System.Drawing;
using TRImageControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Textures;

public class TR1ColosseumTextureBuilder : TextureBuilder
{
    public override List<InjectionData> Build()
    {
        TR1Level colosseum = _control1.Read($"Resources/{TR1LevelNames.COLOSSEUM}");
        InjectionData data = InjectionData.Create(InjectionType.TextureFix, "colosseum_textures");

        data.RoomEdits.AddRange(CreateVertices(colosseum));
        data.RoomEdits.AddRange(CreateFillers(colosseum));
        data.RoomEdits.AddRange(CreateRefacings());
        data.RoomEdits.AddRange(CreateVertexShifts(colosseum));

        FixRoofTextures(data);

        return new() { data };
    }

    private static List<TRRoomVertexCreate> CreateVertices(TR1Level colosseum)
    {
        TRRoomVertex copy = colosseum.Rooms[2].Mesh.Vertices[colosseum.Rooms[2].Mesh.Rectangles[13].Vertices[1]];
        return new()
        {
            new()
            {
                RoomIndex = 2,
                Vertex = new()
                {
                    Lighting = colosseum.Rooms[7].Mesh.Vertices[colosseum.Rooms[7].Mesh.Rectangles[1].Vertices[0]].Lighting,
                    Vertex = new()
                    {
                        X = copy.Vertex.X,
                        Y = (short)(copy.Vertex.Y + 512),
                        Z = copy.Vertex.Z
                    }
                }
            },
        };
    }

    private static List<TRRoomTextureCreate> CreateFillers(TR1Level colosseum)
    {
        return new()
        {
            new()
            {
                RoomIndex = 2,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 2,
                SourceIndex = 1,
                Vertices = new()
                {
                    (ushort)colosseum.Rooms[2].Mesh.Vertices.Count,
                    colosseum.Rooms[2].Mesh.Rectangles[13].Vertices[1],
                    colosseum.Rooms[2].Mesh.Rectangles[2].Vertices[0],
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
                RoomIndex = 82,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 82,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 3,
                TargetIndex = 5
            },
            new()
            {
                RoomIndex = 37,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 37,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 36,
                TargetIndex = 46
            },
            new()
            {
                RoomIndex = 75,
                FaceType = TRMeshFaceType.TexturedQuad,
                SourceRoom = 75,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 34,
                TargetIndex = 12
            },
            new()
            {
                RoomIndex = 67,
                FaceType = TRMeshFaceType.TexturedTriangle,
                SourceRoom = 67,
                SourceFaceType = TRMeshFaceType.TexturedQuad,
                SourceIndex = 7,
                TargetIndex = 0
            }
        };
    }

    private static List<TRRoomVertexMove> CreateVertexShifts(TR1Level colosseum)
    {
        return new()
        {
            new()
            {
                RoomIndex = 2,
                VertexIndex = colosseum.Rooms[2].Mesh.Rectangles[2].Vertices[0],
                VertexChange = new() { Y = -256 }
            },
        };
    }

    private static void FixRoofTextures(InjectionData data)
    {
        // Replace the Midas textures used on the roof of Colosseum with
        // those from the beta.
        TRImage betaRoof = new("Resources/TR1/Colosseum/roof.png");
        List<Color> palette = new()
        {
            Color.FromArgb(0),
        };
        betaRoof.Read((c, x, z) =>
        {
            if (c.A == 0)
            {
                return;
            }

            if (!palette.Contains(c))
            {
                palette.Add(c);
            }
        });

        while (palette.Count < 256)
        {
            palette.Add(Color.Black);
        }

        List<TRColour> trPalette = new(palette.Select(c => c.ToTRColour()));
        byte[] pixels = betaRoof.ToRGB(trPalette);

        for (int i = 0; i < data.Palette.Count; i++)
        {
            data.Palette[i].Red = trPalette[i].Red;
            data.Palette[i].Green = trPalette[i].Green;
            data.Palette[i].Blue = trPalette[i].Blue;
        }

        data.TextureOverwrites.Add(new()
        {
            Page = 3,
            X = 128,
            Y = 0,
            Width = 128,
            Height = 64,
            Data = pixels,
        });
    }
}