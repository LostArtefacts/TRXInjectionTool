# TRX Assets

TR1 and TR2 ship with several default injections to fix or extend base-level
data. These include enhancements such as Lara's additional animations, extended
fonts, and the PDA model. While compatible as-is with custom levels, you may
wish to adapt them for your own needs. Assets are available to download and
import into **WadTool**:

**Asset ZIP files:**  
- https://lostartefacts.dev/pub/tr1-assets.zip  
- https://lostartefacts.dev/pub/tr2-assets.zip  

---

## Tomb Editor Catalogs
The above zips contain `Moveables.xml` and/or `SpriteSequences.xml` catalog
files for TombEditor. This allows the TRX object names to be shown, and allows
for such things as placing TR2 guns in TR1 levels. Refer to the README file
contained within the zip files.

## Common Steps for Importing Assets

1. Open your level's **WAD** file in **WadTool**.
2. Open the extracted `.wad2` file for the applicable game as the
**source level** in WadTool.
3. Move the required assets from the source to the destination, replacing the
existing ones.
4. Follow any **asset-specific extra steps** listed in the table below.
5. Update your **TRX gameflow** to remove references to the asset's `.bin` file.

---

## Asset Reference Table

<table>
  <thead>
    <tr>
      <th>Asset</th>
      <th>TR1 slot</th>
      <th>TR2 slot</th>
      <th><code>.bin</code> file</th>
      <th>Comments/extra steps</th>
    </tr>
  </thead>
  <tbody>
    <tr valign="top">
      <td><code>Lara</code></td>
      <td>0</td>
      <td>0</td>
      <td><code>lara_animations.bin</code></td>
      <td>
        - If Lara's appearance is customised, move the source object to another slot and replace meshes manually.<br>
        - <strong>TR1 only:</strong> In TombEditor, add <code>wet-feet.xml</code> from the zip file above to the sound catalogue (adds sound IDs 15 &amp; 17).<br>
        - <strong>TR1 only:</strong> Point TombEditor to the extracted wet feet <code>.wav</code> files extracted from above, or otherwise, provide your own samples for these SFX.
      </td>
    </tr>
    <tr valign="top">
      <td><code>Guns</code></td>
      <td>2, 201-212</td>
      <td>N/A</td>
      <td><code>lara_guns.bin</code></td>
      <td>
        Relevant to TR1 only. OG TR1 had Lara's shotgun as part of her torso mesh in the Lara Shotgun Animation object. Later games separated this out to allow for different rifle type guns.
        If you wish to replace the shotgun and/or Lara's hands while equipping it, you can use the provided WAD. It also contains all guns and related objects/sprites from TR2.
      </td>
    </tr>
    <tr valign="top">
      <td><code>Lara misc</code></td>
      <td>5</td>
      <td>12</td>
      <td><code>lara_extra.bin</code></td>
      <td>
        This is a combined object with all extra animations shared between TR1 and TR2. The meshes are not used for meshswaps, refer to later objects listed below.
      </td>
    </tr>
    <tr valign="top">
      <td><code>PDA model</code></td>
      <td>82</td>
      <td>134</td>
      <td><code>pda_model.bin</code></td>
      <td>
        Replaces the original PDA model with one that animates - used in-game for the Gameplay options UI.
      </td>
    </tr>
    <tr valign="top">
      <td><code>Flares</code></td>
      <td>187, 188, 192, 193, 194</td>
      <td>N/A</td>
      <td><code>lara_guns.bin</code></td>
      <td>
        Relevant to TR1 only - this makes all TR2 guns and flares available.
      </td>
    </tr>
    <tr valign="top">
      <td><code>Lara T-rex extra skin</code></td>
      <td>195</td>
      <td>270</td>
      <td><code>lara_extra.bin</code></td>
      <td>
        Mesh swap for Lara when she is mauled by the t-rex. No default meshes exist for TR2, but this slot can be used if desired.
      </td>
    </tr>
    <tr valign="top">
      <td><code>Lara Midas extra skin</code></td>
      <td>196</td>
      <td>271</td>
      <td><code>lara_extra.bin</code></td>
      <td>
        Mesh swap for Lara when she stands on the Midas hand.
      </td>
    </tr>
    <tr valign="top">
      <td><code>Lara&nbsp;dagger&nbsp;extra&nbsp;skin&nbsp;1</code></td>
      <td>197</td>
      <td>272</td>
      <td><code>lara_extra.bin</code></td>
      <td>
        Mesh swap for Lara's right hand when she pulls the dagger from the dragon.
      </td>
    </tr>
    <tr valign="top">
      <td><code>Lara&nbsp;dagger&nbsp;extra&nbsp;skin&nbsp;2</code></td>
      <td>198</td>
      <td>273</td>
      <td><code>lara_extra.bin</code></td>
      <td>
        Mesh swap for Lara's right hand and hips if the extra anims 15 or 16 are used (Home Sweet Home).
      </td>
    </tr>
    <tr valign="top">
      <td><code>Lara ponytail mesh swap</code></td>
      <td>199</td>
      <td>274</td>
      <td><code>lara_extra.bin</code></td>
      <td>
        Mesh swap for Lara's ponytail when she stands on the Midas hand.
      </td>
    </tr>
    <tr valign="top">
      <td><code>Lara ponytail and body swap</code></td>
      <td>189, 200</td>
      <td>N/A</td>
      <td><code>braid.bin</code></td>
      <td>
        Lara's braid for TR1 along with mesh swaps for her torso and head
        (normal, angry and t-rex death) when the braid is active in-game.
      </td>
    </tr>
    <tr valign="top">
      <td><code>Font</code></td>
      <td>190</td>
      <td>255</td>
      <td><code>font.bin</code></td>
      <td>Extends the original font sequence with several additional characters and icons.</td>
    </tr>
    <tr valign="top">
      <td><code>Secrets</code></td>
      <td>N/A</td>
      <td>275-277</td>
      <td><code>secret_models_*.bin</code></td>
      <td>3D secret pickup models for the dragons in OG and the treasure in Golden Mask.</td>
    </tr>
  </tbody>
</table>

---

## Updating the Asset Files (Internal)

Run any of the following to generate the source ZIP files above:

```
TRXInjectionTool.exe tr1-lara-anims
TRXInjectionTool.exe tr2-lara-anims
TRXInjectionTool.exe tr1-font
TRXInjectionTool.exe tr2-font
TRXInjectionTool.exe tr1-pda
TRXInjectionTool.exe tr2-pda
```

## Poses

Additional poses can be added to `Resources/lara_ext.phd` using WadTool and then
the following should be run to generate the JSON file for the games.

```
TRXInjectionTool.exe pose
```
