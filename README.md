# Colorlink

An editor tool that allows to quickly swap the entire color palette of the game. Works with materials, GameObjects, prefabs and assets, by directly linking SerializeProperties and material properties to the palette.

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

// Applying changes

### Creating a palette

1. Open the palette editor via Window → Palette.
2. Click on the "+" button to create palette elements, each with the name and color assign. Give every element a unique name.

// Can move colors
// Renaming is fine

### Linking serialized properties

// Make sure that created a palette

1. Right-click on any color property in the inspector of any GameObject, prefab or asset.
2. In the context menu, select "Link Color" and then the palette element name.
3. A little "→" next to the name will show that the property is now linked! Its color will now change every time you change it in the Palette Window.

You can freely move and rename objects with linked properties. The references to the properties are kept the same way that SerializeFields would keep them, and they are not easily lost. 

Note that properties in the custom editors will only work if created using SerializedProperty.

### Linking material properties

// TODO

### Using palette presets

1. Click on the button with a save icon under palette elements. That will create a new palette preset.
2. You can swap the current palette for a preset by clicking the hand icon next to a preset, or delete it by clicking the cross icon.

You can also turn a palette image into a preset by dragging it directly on the preset area. Note that only the first 30 colors found will be added to the preset.

