# Lara animation injections

TR1 ships with a default injection file to replace Lara's entire animation set,
`lara_animations.bin`. This is required to support jump-twist, responsive
jumping, wading and so on, with all relevant default animation state changes and
commands updated.

For custom levels, this means that if you modify Lara's animations but continue
to use the provided injection file, your modifications will be overwritten. To
resolve this, you can follow these steps.

1. Download and extract the following zip file.
https://lostartefacts.dev/pub/tr1_lara_anim_ext.zip

2. Open your level's WAD file in WadTool.

3. Open the `lara_anim_ext.phd` file extracted from above as the source level in
WadTool.

4. Move Lara from the source to the destination to replace her animation set. If
you have customised Lara's appearance, you can move the source object to a
different slot and replace the meshes manually.

5. In TombEditor, go into your level settings and add a sound catalogue
reference to the `wet-feet.xml` file extracted from above. This will add a
reference to sound ID 15 (wet feet) and ID 17 (wading).

6. You will also need to point TombEditor to the extracted wet feet WAV files,
or otherwise, provide your own samples for these SFX.

7. Update your TR1X gameflow to remove the reference(s) to
`lara_animations.bin`.

##  Updating the asset file (internal)

Run `TRXInjectionTool.exe tr1-lara-anims` to generate the above zip file.
