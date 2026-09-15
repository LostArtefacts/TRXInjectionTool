using System.Drawing;
using TRImageControl;
using TRLevelControl;
using TRLevelControl.Model;
using TRXInjectionTool.Types.Contrib.Util;

namespace TRXInjectionTool.Types.Contrib.Misc;

// The BFG as it comes out of a Quake II archive: the weapon, the boxes it is
// fed, the pages they are drawn from, and the glow shell wrapped around the
// weapon. The archive is a local input and never ships, so this runs only
// where the game is installed.
public class BFGAssets
{
    // The weapon and its ammunition, each already textured against the pages
    // below. Face textures index Textures, which the caller rebases as it
    // folds the pages into a level of its own.
    public TRMesh Gun { get; init; }
    public TRMesh Cells { get; init; }
    public List<TRTexImage16> Pages { get; init; }
    public List<TRObjectTexture> Textures { get; init; }

    private const string _md2Entry = "models/weapons/g_bfg/tris.md2";
    // What the weapon is fed, which Quake II carries as its own pickup.
    private const string _cellsEntry = "models/items/ammo/cells/medium/tris.md2";
    private const int _cellsSize = 180;

    // The shell the weapon glows through: the model again a little larger,
    // drawn added to what is behind it rather than covering it. It shows only
    // where the weapon's own skin carries the blue of its cells, in the color
    // the plasma burns, so the rest of the weapon stays as it is.
    private const float _glowScale = 1.02f;

    // How far the light reaches around the cells, in page pixels, and how much
    // of it survives each step outwards.
    private const int _glowSpread = 6;
    private const float _glowFalloff = 0.72f;
    private static readonly (int R, int G, int B) _glowTint = (48, 255, 96);

    // How brightly the cells burn at each step of the pulse. A face reads the
    // same place of the same shape every step and only the page beneath it
    // changes, so the light rises and falls without the shape moving.
    private static readonly float[] _glowPulse = [0.35f, 0.6f, 0.85f, 1.0f];

    private const int _barrelLength = 330;
    private const float _brightness = 1.7f;
    private const float _saturation = 1.5f;

    public static BFGAssets Import(string pakPath)
    {
        var import = Md2Importer.Import(
            Md2Importer.ReadPakEntry(pakPath, _md2Entry), _barrelLength);
        var skin = Md2Importer.ImportSkin(
            Md2Importer.ReadPakEntry(pakPath, import.SkinName), TRConsts.TPageWidth,
            _brightness, _saturation);

        // The model imports grip up; turning it about the barrel puts the
        // grip under the weapon, where a hand closes on it.
        foreach (var v in import.Mesh.Vertices)
        {
            v.X = (short)-v.X;
            v.Z = (short)-v.Z;
        }
        import.Mesh.SelfCalculateBounds();

        var cells = Md2Importer.Import(
            Md2Importer.ReadPakEntry(pakPath, _cellsEntry), _cellsSize);
        var cellsSkin = Md2Importer.ImportSkin(
            Md2Importer.ReadPakEntry(pakPath, cells.SkinName), TRConsts.TPageWidth,
            _brightness, _saturation);
        Centre(cells.Mesh);

        var pages = new List<TRTexImage16>
        {
            new() { Pixels = skin.ToRGB555() },
            new() { Pixels = cellsSkin.ToRGB555() },
        };
        var textures = new List<TRObjectTexture>();
        for (int i = 0; i < import.TriangleUVs.Count; i++)
        {
            textures.Add(CreateTexture(import.TriangleUVs[i], 0));
            import.Mesh.TexturedTriangles[i].Texture = (ushort)i;
            import.Mesh.TexturedTriangles[i].DoubleSided = true;
        }
        var cellsBase = textures.Count;
        for (int i = 0; i < cells.TriangleUVs.Count; i++)
        {
            textures.Add(CreateTexture(cells.TriangleUVs[i], 1));
            cells.Mesh.TexturedTriangles[i].Texture = (ushort)(cellsBase + i);
            cells.Mesh.TexturedTriangles[i].DoubleSided = true;
        }

        AddGlowShell(pages, textures, import.Mesh, import.TriangleUVs, skin);

        return new()
        {
            Gun = import.Mesh,
            Cells = cells.Mesh,
            Pages = pages,
            Textures = textures,
        };
    }

    // Wraps the weapon in a slightly larger copy of itself, drawn added to
    // what is behind it, so the weapon reads as lit from within. The copy
    // reads the same places of the skin the weapon does, through a page that
    // holds only what glows, so the shell shows where the cells are and
    // nowhere else.
    private static void AddGlowShell(
        List<TRTexImage16> pages, List<TRObjectTexture> textures, TRMesh mesh,
        List<PointF[]> uvs, TRImage skin)
    {
        // One page to each step of the pulse, the same mask at each strength.
        var atlases = new ushort[_glowPulse.Length];
        for (int i = 0; i < _glowPulse.Length; i++)
        {
            var glow = MakeGlowPage(skin, _glowPulse[i]);
            pages.Add(new() { Pixels = glow.ToRGB555() });
            atlases[i] = (ushort)(pages.Count - 1);
        }

        var b = mesh.GetBounds();
        var cx = (b.MinX + b.MaxX) / 2.0f;
        var cy = (b.MinY + b.MaxY) / 2.0f;
        var cz = (b.MinZ + b.MaxZ) / 2.0f;

        var shellBase = mesh.Vertices.Count;
        var originals = mesh.Vertices.ToList();
        for (int i = 0; i < originals.Count; i++)
        {
            var v = originals[i];
            mesh.Vertices.Add(new()
            {
                X = (short)Math.Round(cx + (v.X - cx) * _glowScale),
                Y = (short)Math.Round(cy + (v.Y - cy) * _glowScale),
                Z = (short)Math.Round(cz + (v.Z - cz) * _glowScale),
            });
            mesh.Normals.Add(mesh.Normals[i]);
        }

        var faces = mesh.TexturedTriangles.ToList();
        for (int f = 0; f < faces.Count; f++)
        {
            var face = faces[f];

            // One texture per step, each reading the page further along, and a
            // range naming them so the engine walks the face through them.
            var first = textures.Count;
            foreach (var step in atlases)
            {
                textures.Add(CreateTexture(uvs[f], step, 2));
            }
            mesh.TexturedTriangles.Add(new()
            {
                Type = TRFaceType.Triangle,
                Vertices =
                [
                    (ushort)(face.Vertices[0] + shellBase),
                    (ushort)(face.Vertices[1] + shellBase),
                    (ushort)(face.Vertices[2] + shellBase),
                ],
                Texture = (ushort)first,
                DoubleSided = true,
            });
        }
        mesh.SelfCalculateBounds();
    }

    // Keeps the cyan of the weapon's cells and throws the rest away, then
    // lets it reach a little past where it sits, so the light appears around
    // what glows rather than only on it. Black adds nothing to what is behind
    // it, so everything else goes dark.
    private static TRImage MakeGlowPage(TRImage skin, float strength)
    {
        var w = skin.Width;
        var h = skin.Height;

        // How cyan each pixel is: how much green and blue it has over red.
        var mask = new float[w * h];
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                var c = Color.FromArgb((int)skin[x, y]);
                var cyan = Math.Min(c.G, c.B) - c.R;
                mask[y * w + x] = cyan <= 24
                    ? 0.0f
                    : Math.Min(1.0f, (cyan - 24) / 72.0f);
            }
        }

        mask = Spread(mask, w, h);

        var glow = new TRImage(skin.Size);
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                var lit = mask[y * w + x] * strength;
                glow[x, y] = (uint)Color.FromArgb(
                    255,
                    (int)(_glowTint.R * lit),
                    (int)(_glowTint.G * lit),
                    (int)(_glowTint.B * lit)).ToArgb();
            }
        }
        return glow;
    }

    // Grows what is lit outwards, each step reaching one pixel further and
    // carrying less than the one before, so the light fades off its edges.
    private static float[] Spread(float[] mask, int w, int h)
    {
        var current = mask;
        for (int step = 0; step < _glowSpread; step++)
        {
            var next = (float[])current.Clone();
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    var best = 0.0f;
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            var nx = x + dx;
                            var ny = y + dy;
                            if (nx < 0 || ny < 0 || nx >= w || ny >= h)
                            {
                                continue;
                            }
                            best = Math.Max(best, current[ny * w + nx]);
                        }
                    }
                    next[y * w + x] =
                        Math.Max(current[y * w + x], best * _glowFalloff);
                }
            }
            current = next;
        }
        return current;
    }

    // Sits a pickup on its own middle, so it turns about itself in the ring.
    private static void Centre(TRMesh mesh)
    {
        var b = mesh.GetBounds();
        var dx = (short)((b.MinX + b.MaxX) / 2);
        var dy = (short)((b.MinY + b.MaxY) / 2);
        var dz = (short)((b.MinZ + b.MaxZ) / 2);
        foreach (var v in mesh.Vertices)
        {
            v.X -= dx;
            v.Y -= dy;
            v.Z -= dz;
        }
        mesh.SelfCalculateBounds();
    }

    private static TRObjectTexture CreateTexture(
        PointF[] uvs, ushort atlas, ushort blend = 0)
    {
        var max = TRConsts.TPageWidth - 1;
        var texture = new TRObjectTexture
        {
            Atlas = atlas,
            IsTriangle = true,
            BlendingMode = (TRBlendingMode)blend,
            Vertices = [.. uvs.Select(uv => new TRObjectTextureVert(
                (ushort)Math.Clamp((int)Math.Round(uv.X * max), 0, max),
                (ushort)Math.Clamp((int)Math.Round(uv.Y * max), 0, max)))],
        };
        texture.Vertices.Add(new());
        return texture;
    }
}
