using System.Drawing;
using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRXInjectionTool.Control;
using TRXInjectionTool.Util;

namespace TRXInjectionTool.Types.TR2.Misc;

public class TR2BoatBitsBuilder : InjectionBuilder
{
    public override List<InjectionData> Build()
    {
        TR2Level venice = _control2.Read($"Resources/{TR2LevelNames.VENICE}");
        TRModel boatBits = CreateBoatBits(venice.Models[TR2Type.Boat]);

        TR2TexturePacker packer = new(venice);
        var regions = packer.GetMeshRegions(boatBits.Meshes).Values.SelectMany(v => v);
        List<TRObjectTexture> originalInfos = new(venice.ObjectTextures);

        List<Color> basePalette = new(venice.Palette.Select(c => c.ToTR1Color()));
        ResetLevel(venice, 1);

        packer = new(venice);
        packer.AddRectangles(regions);
        packer.Pack(true);

        venice.Models[TR2Type.BoatBits] = boatBits;
        venice.ObjectTextures.AddRange(regions.SelectMany(r => r.Segments.Select(s => s.Texture as TRObjectTexture)));
        boatBits.Meshes
            .SelectMany(m => m.TexturedFaces)
            .ToList()
            .ForEach(f =>
            {
                f.Texture = (ushort)venice.ObjectTextures.IndexOf(originalInfos[f.Texture]);
            });

        GenerateImages8(venice, basePalette);

        _control2.Write(venice, MakeOutputPath(TRGameVersion.TR2, "Debug/BoatBits.tr2"));
        InjectionData data = InjectionData.Create(venice, InjectionType.TextureFix, "boat_bits", false);
        return new() { data };
    }

    private static TRModel CreateBoatBits(TRModel boat)
    {
        const short width = 100;
        const short height = 70;
        const short depth = 40;
        const int sizeShift = 30;
        List<ushort> textures = new() { 512, 507, 502, 500, };
        Random rand = new(20250418);

        List<List<ushort>> faceVerts = new()
        {
            new() { 0, 1, 2, 3 },
            new() { 6, 5, 4, 7 },
            new() { 3, 2, 6, 7 },
            new() { 2, 1, 5, 6 },
            new() { 1, 0, 4, 5 },
            new() { 0, 3, 7, 4 },
        };

        TRMesh CreateMesh()
        {
            int wr = rand.Next(sizeShift * 2) - sizeShift;
            int hr = rand.Next(sizeShift * 2) - sizeShift;
            int dr = rand.Next(sizeShift * 2) - sizeShift;
            short wp = (short)(width + wr);
            short hp = (short)(height + hr);
            short dp = (short)(depth + dr);
            short wn = (short)-wp;
            short hn = (short)-hp;
            short dn = (short)-dp;

            TRMesh mesh = new()
            {
                Vertices = new()
                {
                    new() { X = wn, Y = hn, Z = dn, },
                    new() { X = wn, Y = hn, Z = dp, },
                    new() { X = wp, Y = hn, Z = dp, },
                    new() { X = wp, Y = hn, Z = dn, },
                    new() { X = wn, Y = hp, Z = dn, },
                    new() { X = wn, Y = hp, Z = dp, },
                    new() { X = wp, Y = hp, Z = dp, },
                    new() { X = wp, Y = hp, Z = dn, },
                },
                Normals = new(Enumerable.Repeat(new TRVertex(), 8)),
            };

            ushort tex = textures[rand.Next(textures.Count)];
            mesh.TexturedRectangles.AddRange(faceVerts.Select(v =>
            {
                return new TRMeshFace
                {
                    Vertices = v,
                    Texture = tex,
                };
            }));

            mesh.SelfCalculateBounds();
            return mesh;
        }

        TRModel boatBits = new();
        boatBits.Animations.Add(new()
        {
            Accel = new(),
            Speed = new(),
            FrameRate = 1,
            FrameEnd = 1,
            Frames = new()
            {
                new()
                {
                    Rotations = new(),
                }
            }
        });

        // Steering wheel
        boatBits.Meshes.Add(boat.Meshes[1].Clone());
        boatBits.Animations[0].Frames[0].Rotations.Add(new() { Y = 768 });

        // Semi-random tree of bits
        for (int i = 0; i < 10; i++)
        {
            boatBits.Meshes.Add(CreateMesh());
            boatBits.MeshTrees.Add(new()
            {
                Flags = (uint)(i == 0 ? 2 : (i == 5 ? 3 : 0)),
                OffsetX = width * 2,
                OffsetZ = i > 5 ? depth * 2 : 0,
            });
            boatBits.Animations[0].Frames[0].Rotations.Add(new());
        }

        for (int i = 0; i < 5; i++)
        {
            boatBits.Meshes.Add(CreateMesh());
            boatBits.MeshTrees.Add(new()
            {
                Flags = (uint)(i == 0 ? 3 : 0),
                OffsetX = width * 2,
                OffsetZ = -depth * 2,
            });
            boatBits.Animations[0].Frames[0].Rotations.Add(new());
        }

        for (int i = 0; i < 10; i++)
        {
            boatBits.Meshes.Add(CreateMesh());
            boatBits.MeshTrees.Add(new()
            {
                Flags = (uint)(i == 0 || i == 5 ? 3 : 0),
                OffsetX = -width * 2,
                OffsetZ = i > 5 ? depth * 2 : 0,
            });
            boatBits.Animations[0].Frames[0].Rotations.Add(new());
        }

        for (int i = 0; i < 5; i++)
        {
            boatBits.Meshes.Add(CreateMesh());
            boatBits.MeshTrees.Add(new()
            {
                Flags = (uint)(i == 0 ? 3 : 0),
                OffsetX = -width * 2,
                OffsetZ = -depth * 2,
            });
            boatBits.Animations[0].Frames[0].Rotations.Add(new());
        }

        boatBits.Animations[0].Frames[0].Bounds = boatBits.GetBounds();
        return boatBits;
    }
}
