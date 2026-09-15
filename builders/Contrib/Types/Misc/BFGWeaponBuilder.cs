using System.Drawing;
using System.IO.Compression;
using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl;
using TRLevelControl.Model;
using TRXInjectionTool.Actions;
using TRXInjectionTool.Control;
using TRXInjectionTool.Model;
using TRXInjectionTool.Util;

namespace TRXInjectionTool.Types.Contrib.Misc;

// Builds the BFG9000 as content the game has no number for. The file names the
// three objects it brings in its symbol table, and the object records state a
// symbol index in place of a slot, so nothing here depends on which slots the
// game or another mod happens to use.
//
// The models are the rocket launcher's, which supplies working animations and
// frames; only the meshes are the Quake II model. Reusing them is what keeps
// the file to meshes and textures rather than a skeleton of its own.
public class BFGWeaponBuilder : InjectionBuilder
{
    // The Quake archives the model and the sounds are read from, dropped into
    // src/Resources/Quake by whoever owns the games. They are a local input
    // and never ship, so the builder is skipped where they are absent.
    private const string _modelPakPath = "Resources/Quake/pak0.pak";
    private const string _soundPakPath = "Resources/Quake/pak0.pk3";
    private const string _levelPath = "Resources/TR3/AREA51.TR2";

    // The names the script mints and the blob answers to.
    private const string _gunSymbol = "dash:bfg_item";
    private const string _ammoSymbol = "dash:bfg_ammo_item";
    private const string _ballSymbol = "dash:bfg_ball";
    // What Lara wears: the weapon in her hand and the weapon on her back. It
    // is kept apart from the pickup because the inventory draws the pickup
    // whole, and these two are never seen together.
    private const string _heldSymbol = "dash:bfg_held";
    private const string _fireSymbol = "dash:bfg_fire";
    private const string _arriveSymbol = "dash:bfg_arrive";

    // The weapon's report, lifted from the Quake III archive. Quake II's is
    // the charge the weapon winds up with, which is far longer than a shot.
    private const string _fireEntry = "sound/weapons/bfg/bfg_fire.wav";
    private const string _arriveEntry = "sound/items/holdable.wav";

    // Lara's outfit for TR3's jungle levels, and the meshes gun map 2 gives
    // the MP5: the weapon in her hand and the part of it on her torso. The BFG
    // takes their place, so the weapon she holds is drawn as the BFG.
    private const TR3Type _outfit = TR3Type.LaraSkin_H;
    private const int _handMesh = 57;
    private const int _torsoMesh = 63;

    // Nudges applied after the import is anchored on the mesh it replaces.
    private const short _offsetX = 30;
    private const short _offsetY = 140;
    private const short _offsetZ = 40;

    // The slots the models are carried in while the file is built. They are
    // replaced by symbol indices before it is written, so nothing ships them.
    private const TR3Type _heldCarrier = TR3Type.Grenades_M_H;
    private const TR3Type _gunCarrier = TR3Type.RocketLauncher_M_H;
    private const TR3Type _ammoCarrier = TR3Type.Rockets_M_H;
    private const TR3Type _ballCarrier = TR3Type.RocketSingle;

    private static readonly TR3Type[] _carriers =
    [
        TR3Type.RocketLauncher_M_H,
        TR3Type.Rockets_M_H,
        TR3Type.RocketSingle,
        TR3Type.Grenades_M_H,
    ];

    private static readonly string[] _symbols =
    [
        _gunSymbol,
        _ammoSymbol,
        _ballSymbol,
        _heldSymbol,
    ];

    public override string ID => "bfg";

    public override IEnumerable<string> RequiredResources =>
        [_modelPakPath, _soundPakPath];

    public override List<InjectionData> Build()
    {
        return [BuildUnified()];
    }

    // One file for every game: the canonical injection format carries each
    // record in a single layout, the models are named through the symbol
    // table rather than slots, and the samples ride inline - so the same
    // bytes serve TR1 through TR4. The TR3 donor supplies the skeleton the
    // weapon borrows; the engine adapts the records to whichever game runs.
    private static InjectionData BuildUnified()
    {
        var source = BFGAssets.Import(_modelPakPath);
        var bfgMesh = source.Gun;
        var cellsMesh = source.Cells;

        var level = _control3.Read(_levelPath);
        var models = new TRDictionary<TR3Type, TRModel>();
        foreach (var type in _carriers)
        {
            if (!level.Models.ContainsKey(type))
            {
                throw new Exception($"{_levelPath} carries no {type}");
            }
            models[type] = level.Models[type];
        }

        // The weapon, the boxes it is fed, and the ball. The ball is drawn by
        // its sparks alone, as Willard's is, so it carries nothing.
        for (int m = 0; m < _carriers.Length; m++)
        {
            var model = models[_carriers[m]];
            if (_carriers[m] == _heldCarrier)
            {
                // The first mesh is held, the second rides her back. The
                // joints they hang from face different ways, so one mesh
                // cannot serve both, which is how the shipped rifles do it.
                model.Meshes = [Anchor(bfgMesh.Clone()), AnchorBack(bfgMesh.Clone())];
                // Every mesh past the first hangs off a joint, and a model
                // whose joints run out is walked past its end as it is drawn.
                model.MeshTrees =
                    [new() { Flags = 0, OffsetX = 0, OffsetY = 0, OffsetZ = 0 }];
            }
            else
            {
                var part = _carriers[m] == _gunCarrier ? Anchor(bfgMesh.Clone())
                    : _carriers[m] == _ammoCarrier ? cellsMesh.Clone()
                    : null;
                for (int i = 0; i < model.Meshes.Count; i++)
                {
                    model.Meshes[i] = part != null && i == 0 ? part : EmptyMesh();
                }
            }

            // The import carries no flag for it, so the faces are told here
            // where the format has one.
            model.Meshes.SelectMany(x => x.TexturedFaces).ToList()
                .ForEach(f => f.DoubleSided = true);
        }

        ResetLevel(level, 1);
        level.Models = models;

        // The import's pages are taken whole rather than packed. Its textures
        // are one per triangle, and a packer given hundreds of them has far
        // more area to place than a page holds, so they land over each other.
        var atlasBase = level.Images16.Count;
        var textureBase = level.ObjectTextures.Count;
        level.Images16.AddRange(source.Pages);
        foreach (var texture in source.Textures)
        {
            texture.Atlas += (ushort)atlasBase;
            level.ObjectTextures.Add(texture);
        }
        foreach (var face in level.Models.Values
            .SelectMany(m => m.Meshes)
            .Where(m => m != null)
            .SelectMany(m => m.TexturedFaces)
            .Distinct())
        {
            face.Texture += (ushort)textureBase;
        }

        GenerateImages8(level, [.. level.Palette.Select(c => c.ToTR1Color())]);

        var data = InjectionData.Create(level, InjectionType.General, "bfg");
        data.AppliesToAllGames = true;
        NameTheObjects(data, [.. _carriers.Select(c => (uint)c)]);
        NameTheSound(data);
        SlideTheGlow(data);
        return data;
    }

    // Maps each carrier slot to the name it stands for, so that the slots
    // used while building leave no trace in what ships.
    private static void NameTheObjects(InjectionData data, uint[] carriers)
    {
        for (int i = 0; i < _symbols.Length; i++)
        {
            if (data.Models.Find(m => m.ID == carriers[i]) == null)
            {
                throw new Exception($"{_symbols[i]} lost its model");
            }
            data.Symbols.Add(new()
            {
                Context = SymbolContext.Objects,
                Slot = (int)carriers[i],
                Name = _symbols[i],
            });
        }
    }

    // The weapon's glow rises and falls. Every texture drawn added to what is
    // behind it belongs to the shell, and they were written in runs of one per
    // step, so each run becomes a range the engine walks a face through.
    private const int _glowSteps = 4;

    private static void SlideTheGlow(InjectionData data)
    {
        var shell = Enumerable.Range(0, data.ObjectTextures.Count)
            .Where(i => data.ObjectTextures[i].Attribute == 2)
            .ToList();
        if (shell.Count == 0 || shell.Count % _glowSteps != 0)
        {
            throw new Exception(
                $"the glow holds {shell.Count} textures, which is no whole "
                + $"number of {_glowSteps}-step runs");
        }

        for (int i = 0; i < shell.Count; i += _glowSteps)
        {
            // Up and back down again: a range is walked in a circle, so
            // without the way back the light would fall dark in one step.
            var range = new TRAnimTextureAdd();
            for (int step = 0; step < _glowSteps; step++)
            {
                range.Textures.Add((ushort)shell[i + step]);
            }
            for (int step = _glowSteps - 2; step > 0; step--)
            {
                range.Textures.Add((ushort)shell[i + step]);
            }
            data.AnimTextureAdds.Add(range);
        }
    }

    // Brings the weapon's report and the delivery chime in under names, so
    // the file needs no sound slot from the game.
    private static void NameTheSound(InjectionData data)
    {
        short slot = 0;
        foreach (var (symbol, entry) in new[]
        {
            (_fireSymbol, _fireEntry),
            (_arriveSymbol, _arriveEntry),
        })
        {
            data.Symbols.Add(new()
            {
                Context = SymbolContext.Samples,
                Slot = slot,
                Name = symbol,
            });

            data.SFX.Add(new()
            {
                ID = slot++,
                Volume = 0x7FFF,
                Chance = 0,
                Characteristics = 4,
                Range = 10,
                Pitch = 0,
                Data = [ReadZipEntry(_soundPakPath, entry)],
            });
        }
    }

    // Reads one file out of a Quake III archive, which is an ordinary zip.
    private static byte[] ReadZipEntry(string path, string entry)
    {
        using var archive = ZipFile.OpenRead(path);
        var found = archive.GetEntry(entry)
            ?? throw new Exception($"{path} carries no {entry}");
        using var stream = found.Open();
        using MemoryStream ms = new();
        stream.CopyTo(ms);
        return ms.ToArray();
    }

    // Moves the import so it sits where the hand already holds a weapon: the
    // handle goes to the hand joint, which is the mesh origin. The handle is
    // the narrow part reaching furthest below the body.
    private static TRMesh Anchor(TRMesh mesh)
    {
        var b = mesh.GetBounds();
        var band = b.MaxZ - (b.MaxZ - b.MinZ) / 5;
        var handle = mesh.Vertices.Where(v => v.Z >= band).ToList();
        var dx = -(int)handle.Average(v => v.X);
        var dy = -(int)handle.Average(v => v.Y);
        var dz = -(int)handle.Average(v => v.Z);

        foreach (var v in mesh.Vertices)
        {
            v.X = (short)(v.X + dx + _offsetX);
            v.Y = (short)(v.Y + dy + _offsetY);
            v.Z = (short)(v.Z + dz + _offsetZ);
        }

        mesh.SelfCalculateBounds();
        return mesh;
    }

    // Lays the weapon across her back, where a rifle hangs along her
    // shoulders rather than pointing away from a hand. The place is the one
    // the shipped rifles occupy in the outfit's own gun meshes.
    private static readonly (short X, short Y, short Z) _backCentre = (-22, -76, -80);

    private static TRMesh AnchorBack(TRMesh mesh)
    {
        foreach (var v in mesh.Vertices)
        {
            (v.X, v.Y) = ((short)-v.Y, v.X);
        }
        mesh.SelfCalculateBounds();

        var b = mesh.GetBounds();
        var dx = (short)(_backCentre.X - (b.MinX + b.MaxX) / 2);
        var dy = (short)(_backCentre.Y - (b.MinY + b.MaxY) / 2);
        var dz = (short)(_backCentre.Z - (b.MinZ + b.MaxZ) / 2);
        foreach (var v in mesh.Vertices)
        {
            v.X += dx;
            v.Y += dy;
            v.Z += dz;
        }
        mesh.SelfCalculateBounds();
        return mesh;
    }

    private static TRMesh EmptyMesh()
    {
        return new()
        {
            Vertices = [new()],
            Normals = [new() { Y = 16300 }],
        };
    }
}
