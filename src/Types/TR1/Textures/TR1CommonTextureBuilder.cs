﻿using System.Diagnostics;
using System.Drawing;
using TRImageControl;
using TRLevelControl.Model;
using TRXInjectionTool.Control;

namespace TRXInjectionTool.Types.TR1.Textures;

public static class TR1CommonTextureBuilder
{
    public static void FixWolfTransparency(TR1Level level, InjectionData data)
    {
        List<ushort> eyeVerts = new() { 20, 13, 12, 22 };
        TRMeshFace eyeFace = level.Models[TR1Type.Wolf]
            .Meshes[3]
            .TexturedRectangles.Find(t => t.Vertices.All(eyeVerts.Contains));

        FixTransparentPixels(level, data, eyeFace, Color.Black);
    }

    public static void FixBatTransparency(TR1Level level, InjectionData data)
    {
        List<ushort> eyeVerts = new() { 0, 1, 3 };
        TRMeshFace eyeFace = level.Models[TR1Type.Bat]
            .Meshes[4]
            .TexturedTriangles.Find(t => t.Vertices.All(eyeVerts.Contains));

        FixTransparentPixels(level, data, eyeFace, Color.Black);
    }

    public static void FixTransparentPixels(TR1Level level, InjectionData data, TRFace face, Color fillColour)
    {
        Debug.Assert(face != null);

        TRObjectTexture texInfo = level.ObjectTextures[face.Texture];
        TRImage tile = new(level.Images8[texInfo.Atlas].Pixels, level.Palette);
        TRImage img = tile.Export(texInfo.Bounds);

        img.Write((c, x, y) =>
        {
            c = c.A == 0 ? fillColour : c;
            return c;
        });

        data.TextureOverwrites.Add(new()
        {
            Page = texInfo.Atlas,
            X = (byte)texInfo.Bounds.X,
            Y = (byte)texInfo.Bounds.Y,
            Width = (ushort)texInfo.Size.Width,
            Height = (ushort)texInfo.Size.Height,
            Data = img.ToRGBA(),
        });
    }
}
