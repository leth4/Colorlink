# Colorlink

An editor tool that allows to quickly swap the entire color palette of the game. Works with materials, components, prefabs and assets, by directly linking SerializeProperties and material properties to the palette — no extra components needed!

The tool is **editor only** and can't be used in builds.

## Compatibility

Unity 2020.3 or higher.

// TODO: Try older versions

## Installation

Add the package to your project via
[UPM](https://docs.unity3d.com/Manual/upm-ui.html) using the Git URL
`https://github.com/letharqic/Colorlink.git`. You can also clone the repository
and point UPM to your local copy.

## Usage

// TODO: Add images

### Creating a palette

1. Open the palette editor via Window → Palette.
2. Click on the `+` button to create palette elements, each with the name and color assign. Give every element a unique name.

### Linking serialized properties

1. Right-click on any color property in the inspector of any component, prefab or asset.
2. In the context menu, select `Link Color` and then the palette element name.
3. A little `→` next to the name will show that the property is now linked! Its color will now change every time you change it in the Palette Window.

You can freely move and rename objects with linked properties. The references to the properties are kept the same way that SerializeFields would keep them, and they are not easily lost. 

Note that properties in the custom editors will only work if created using SerializedProperty.

If you want to apply the same linkings to a component of the same type, right-click on the first component header and select `Copy Color Links`. For the next component, select `Paste Color Links`. The same changes will apply.

### Linking material properties

1. Right-click on the material name at the top of the inspector.
2. In the context menu, select "Link Colors". An additional window will open.
3. For each property you want to link, select the palette element from the drop-down menu.
4. Click "Apply" at the bottom of the window. The properties are now linked!

### Managing the palette

To see linked properties, click on the eye icon and unfold the palette elements in question. **You can drag and drop properties** between elements to re-link them!

When you edit your palette, changes to GameObject on the current scene and materials apply automatically. Changing assets and different scenes takes time, so you'll need to click the corresponding buttons to apply changes.

You can move palette elements with arrow buttons on the right. You can also rename them freely — the linkings won't be lost.

The palette stores its data in the `Palette.asset` file in the `ProjectSettings` folder, which is a part of the version control.

### Using palette presets

1. Click on the button with a save icon under palette elements. That will create a new palette preset.
2. You can swap the current palette for a preset by clicking the hand icon next to a preset, or delete it by clicking the cross icon.

Make sure the image has a `Sprite` texture type and `Read/Write` is enabled!

You can also turn a palette image into a preset by dragging it directly on the preset area. Note that only the first 30 colors found will be added to the preset.