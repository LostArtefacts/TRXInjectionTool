# TRX Assets

TR1 and TR2 ship with several default injections to fix or extend base-level
data. These include enhancements such as Lara's additional animations, extended
fonts, and the PDA model. While compatible as-is with custom levels, you may
wish to adapt them for your own needs. Assets are available to download and
import into **WadTool**:

**Asset ZIP files:**  
- https://lostartefacts.dev/pub/tr1-ext.zip  
- https://lostartefacts.dev/pub/tr2-ext.zip  

---

## Common Steps for Importing Assets

1. Open your level's **WAD** file in **WadTool**.
2. Open the extracted `.phd` or `.tr2` file for the asset as the
**source level** in WadTool.
3. Move the required asset from the source to the destination, replacing the
existing one.
4. Follow any **asset-specific extra steps** listed in the table below.
5. Update your **TRX gameflow** to remove references to the asset's `.bin` file.

---

## Asset Reference Table

<table>
  <thead>
    <tr>
      <th>Asset</th>
      <th>Source file</th>
      <th><code>.bin</code> file</th>
      <th>Extra steps</th>
    </tr>
  </thead>
  <tbody>
    <tr valign="top">
      <td><strong>Lara animations</strong></td>
      <td><code>lara.phd</code> / <code>lara.tr2</code></td>
      <td><code>lara_animations.bin</code></td>
      <td>
        - If Lara's appearance is customised, move the source object to another slot and replace meshes manually.<br>
        - <strong>TR1 only:</strong> In TombEditor, add <code>wet-feet.xml</code> from the zip file above to the sound catalogue (adds sound IDs 15 &amp; 17).<br>
        - <strong>TR1 only:</strong> Point TombEditor to the extracted wet feet <code>.wav</code> files extracted from above, or otherwise, provide your own samples for these SFX.
      </td>
    </tr>
    <tr valign="top">
      <td><strong>Font</strong></td>
      <td><code>font.phd</code> / <code>font.tr2</code></td>
      <td><code>font.bin</code></td>
      <td><em>None</em></td>
    </tr>
    <tr valign="top">
      <td><strong>PDA model</strong></td>
      <td><code>pda.phd</code> / <code>pda.tr2</code></td>
      <td><code>pda_model.bin</code></td>
      <td><em>None</em></td>
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
