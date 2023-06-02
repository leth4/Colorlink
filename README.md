# Colorlink

An editor tool that allows to quickly change the entire color palette of the game. Works with materials, components, prefabs and assets, by directly linking serialized properties and material properties to the palette — no extra components needed! The tool is **editor only** and can't be used in builds.

![image](https://user-images.githubusercontent.com/44412176/236405859-6416a5f9-b133-4374-9ac6-5ae11986c54f.gif)

## Compatibility

Unity 2020.3 or higher.

## Installation

Add the package to your project via the [Package Manager](https://docs.unity3d.com/Manual/upm-ui.html) using the Git URL
`https://github.com/letharqic/Colorlink.git`. You can also clone the repository and point the Package Manager to your local copy.

## Usage

### Creating a palette

1. Open the palette editor via Window → Palette.
2. Click on the `+` button to create palette elements, each with a name and a color assigned. Give every element a unique name.

### Linking serialized properties

1. Right-click on any color property in the inspector of any component, prefab or asset.
2. In the context menu, select `Link Color` and then the palette element name.
3. A little `→` next to the name will show that the property is now linked! Its color will now change every time you change it in the Palette Window. You can unlink it via the same menu.

![image](https://user-images.githubusercontent.com/44412176/236387862-a2e81ea4-11e4-4074-bbff-6e4cc952f2ea.png)
![image](https://user-images.githubusercontent.com/44412176/236388019-7dee1343-33ce-459c-8e12-3a002ee0a5b7.png)

You can freely move and rename objects with linked properties. The references to the properties are kept the same way that SerializeFields would keep them, and they are not easily lost. 

Note that properties in the custom editors can only be linked if created using SerializedProperty.

If you want to apply the same links to a component of the same type, right-click on the first component header and select `Copy Color Links`. For the next component, select `Paste Color Links`.

### Linking material properties

1. Right-click on the material name at the top of the inspector.
2. In the context menu, select "Link Colors". An additional window will open.
3. For each property you want to link, select the palette element from the drop-down menu.
4. Click `Link` at the bottom of the window. The properties are now linked!

![image](https://user-images.githubusercontent.com/44412176/236388605-813e4f86-54fa-4416-a420-17c0411e0c70.png)
![image](https://user-images.githubusercontent.com/44412176/236388615-57969e2c-f603-4644-a60e-1e25b8879fc2.png)

### Managing the palette

To see linked properties, click on the eye icon and unfold the palette elements in question. **You can drag and drop properties** between elements to re-link them!

When you edit your palette, changes to GameObjects on the current scene and materials apply automatically. Changing assets and other scenes takes time, so you'll need to click the corresponding buttons to apply changes.

You can move palette elements with arrow buttons on the right. You can also rename them freely — the links won't be lost.

The palette stores its data in the `Palette.asset` file in the `ProjectSettings` folder, which is subject to version control.

![image](https://user-images.githubusercontent.com/44412176/236402915-91264ec8-4278-4a2d-9118-8ca699fceeed.png)

### Using palette presets

1. Click on the button with a save icon under palette elements. That will create a new palette preset.
2. You can swap the current palette for a preset by clicking the hand icon next to a preset, or delete it by clicking the cross icon.

You can also turn a palette image into a preset by dragging it directly on the preset area. Note that only the first 30 colors found will be added to the preset. Make sure that the image has a `Sprite` texture type and `Read/Write` is enabled!
