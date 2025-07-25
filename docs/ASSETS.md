# Lara animation injections

TR1 and TR2 ship with a default injection file to replace Lara's entire
animation set, `lara_animations.bin`. This is required to support jump-twist,
responsive jumping, wading, slide-to-run and so on, with all relevant default
animation state changes and commands updated.

For custom levels, this means that if you modify Lara's animations but continue
to use the provided injection file, your modifications will be overwritten. To
resolve this, you can follow these steps.

1. Download and extract the following zip files.
    - https://lostartefacts.dev/pub/tr1-lara-anim-ext.zip
    - https://lostartefacts.dev/pub/tr2-lara-anim-ext.zip

2. Open your level's WAD file in WadTool.

3. Open the `lara_anim_ext.phd`/`lara_anim_ext.tr2` file extracted from above as
the source level in WadTool.

4. Move Lara from the source to the destination to replace her animation set. If
you have customised Lara's appearance, you can move the source object to a
different slot and replace the meshes manually.

5. **TR1 Only** In TombEditor, go into your level settings and add a sound
catalogue reference to the `wet-feet.xml` file extracted from above. This will
add a reference to sound ID 15 (wet feet) and ID 17 (wading).

6. **TR1 Only** You will also need to point TombEditor to the extracted wet feet
WAV files, or otherwise, provide your own samples for these SFX.

7. Update your TR1X gameflow to remove the reference(s) to
`lara_animations.bin`.

# PDA models
The PDA model is used in-game for gameplay config options. The original model
only has one mesh and no animation data, so if the default injection that ships
with TRX is not used, the animation will not work correctly. The modified models
can be downloaded below and added directly to your WAD to allow customisation.

- https://lostartefacts.dev/pub/tr1-pda.zip
- https://lostartefacts.dev/pub/tr2-pda.zip

##  Updating the asset files (internal)

Run the following to generate the above zip files.

- `TRXInjectionTool.exe tr1-lara-anims`
- `TRXInjectionTool.exe tr1-pda`
- `TRXInjectionTool.exe tr2-pda`

# Poses

Additional poses can be added to `Resources/pose.phd` using WadTool and then the
following should be run to generate the JSON file for the games.

- `TRXInjectionTool.exe pose`
