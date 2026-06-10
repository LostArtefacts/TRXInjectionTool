# TRXInjectionTool

TRXInjectionTool is the tool used in TRX for creating the injections

In TRX, an *injection* is a binary patch file (`.bin`) that the engine applies
to level data at load time. Injections are used to fix or extend base game data
in a way that stays compatible with custom levels (unless you intentionally
replace the same data in your own WAD).

Most builders only need injections for the "default TRX assets" (extra Lara
animations, extended fonts, PDA model, etc).

## How injections are configured (gameflow)

Gameflow JSON supports injections in two places:

- **Global injections**: applied to all levels.
- **Per-level injections**: applied only for that level, optionally inheriting global injections.

By default, injections defined in the global gameflow are applied to every
level. If a level defines its own injections, those are merged with the global
set when the level loads.

Individual levels can set `inherit_injections` to `false`. In that case, global
injection files are not used. If such a level defines its own `injections`,
only those are applied; if it defines none, nothing is injected.

Relevant keys (names may differ slightly per gameflow version):

```json5
{
  // global
  "injections": [
    "data/injections/lara_extra.bin",
    "data/injections/font.bin"
  ],

  "levels": [
    {
      "path": "data/levels/MY_LEVEL.TR2",
      "inherit_injections": true,
      "injections": [
        "data/injections/pda_model.bin"
      ]
    }
  ]
}
```

> [!WARNING]
> If you **import** the assets into your level WAD (see below), you should then
> **remove** the corresponding `.bin` from gameflow to avoid
> double-applying/replacing data!

> [!NOTE]
> If a level should **not** receive the global injections, set
> `"inherit_injections": false` (or omit inheritance, depending on the
> schema/version you're targeting).

> [!NOTE]
> The gameflow ignores referenced injection files that do not exist, but it's
> best practice to remove references to keep gameflow clean.
>
## Builder workflow: keep the `.bin`, or bake into your WAD

You can handle TRX default assets in two ways:

1. **Keep using injections** (recommended for most cases):
   ship the `.bin` files and reference them in gameflow.
2. **Bake assets into your WAD**:
   import the provided assets into your level's WAD, then remove the related
   `.bin` references from gameflow.

## TRX Assets

[See Here](docs/ASSETS.md) for how to use TRX Injections Assets with your custom level
