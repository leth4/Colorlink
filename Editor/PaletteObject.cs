using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Colorlink
{
    [FilePath("/ProjectSettings/Palette.asset", FilePathAttribute.Location.ProjectFolder)]
    public class PaletteObject : ScriptableSingleton<PaletteObject>
    {
        public List<ColorGroup> ColorGroups = new List<ColorGroup>();
        public List<PalettePreset> Presets = new List<PalettePreset>();

        public void SaveChanges() => Save(true);

        public void SavePreset()
        {
            var colors = new List<Color>();
            foreach (var colorGroup in ColorGroups)
            {
                colors.Add(colorGroup.Color);
            }
            Presets.Insert(0, new PalettePreset(colors));
        }

        public void AddPreset(List<Color> colors)
        {
            if (Presets == null) Presets = new List<PalettePreset>();
            Presets.Add(new PalettePreset(colors));
        }

        public void ChangeColorGroupIndex(ColorGroup colorGroup, int change)
        {
            var index = ColorGroups.IndexOf(colorGroup);
            if (index + change < 0 || index + change >= ColorGroups.Count) return;
            var temp = ColorGroups[index];
            ColorGroups[index] = ColorGroups[index + change];
            ColorGroups[index + change] = temp;
        }

        public void AddColorGroup()
        {
            ColorGroups.Add(new ColorGroup() { Name = "New Color Group" });
        }

        public void RemoveColorGroup(ColorGroup colorGroup)
        {
            ColorGroups.Remove(colorGroup);
        }

        public void AddProperty(ColorGroup colorGroup, ColorProperty property)
        {
            foreach (var group in ColorGroups)
            {
                if (group == colorGroup)
                    group.ToggleProperty(property);
                else
                    group.RemoveProperty(property);
            }
        }

        public void ApplyColors(bool includeAssets = false)
        {
            foreach (var colorGroup in ColorGroups)
            {
                var propertiesToRemove = new List<ColorProperty>();
                foreach (var property in colorGroup.Properties)
                {
                    if (GlobalObjectId.TryParse(property.GuidString, out GlobalObjectId guidObject))
                    {
                        if (property.ObjectType == ColorProperty.Type.GameObject && EditorSceneManager.GetActiveScene().path != AssetDatabase.GUIDToAssetPath(guidObject.assetGUID))
                        {
                            continue;
                        }

                        var obj = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(guidObject);
                        if (obj == null)
                        {
                            propertiesToRemove.Add(property);
                            continue;
                        }

                        property.ObjectName = obj.name;

                        if (!includeAssets && property.ObjectType == ColorProperty.Type.Asset) continue;

                        if (property.ObjectType == ColorProperty.Type.Material)
                        {
                            var mat = (Material)obj;
                            if (!((Material)obj).HasProperty(property.PropertyPath))
                            {
                                propertiesToRemove.Add(property);
                            }
                            else
                            {
                                ((Material)obj).SetColor(property.PropertyPath, colorGroup.Color);
                            }
                            continue;
                        }

                        var serializedObject = new UnityEditor.SerializedObject(guidObject.identifierType == 3 ? obj : (Component)obj);
                        var serializedProperty = serializedObject.FindProperty(property.PropertyPath);

                        if (serializedProperty == null)
                        {
                            propertiesToRemove.Add(property);
                            continue;
                        }

                        serializedProperty.colorValue = colorGroup.Color;
                        EditorUtility.SetDirty(serializedObject.targetObject);
                        serializedObject.ApplyModifiedProperties();
                    }
                    else
                    {
                        propertiesToRemove.Add(property);
                    }
                }
                foreach (var property in propertiesToRemove)
                {
                    colorGroup.RemoveProperty(property);
                }
            }

            if (includeAssets) EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), EditorSceneManager.GetActiveScene().path);
        }

        public void ApplyColorsOnAllScenes()
        {
            var initialScenePath = EditorSceneManager.GetActiveScene().path;
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), initialScenePath);

            foreach (var colorGroup in ColorGroups)
            {
                var propertiesToRemove = new List<ColorProperty>();
                foreach (var property in colorGroup.Properties)
                {
                    if (GlobalObjectId.TryParse(property.GuidString, out GlobalObjectId guidObject))
                    {
                        if (property.ObjectType != ColorProperty.Type.GameObject) continue;

                        EditorSceneManager.OpenScene(AssetDatabase.GUIDToAssetPath(guidObject.assetGUID));

                        var obj = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(guidObject);

                        if (obj == null)
                        {
                            propertiesToRemove.Add(property);
                            continue;
                        }

                        var serializedObject = new UnityEditor.SerializedObject((Component)obj);
                        var serializedProperty = serializedObject.FindProperty(property.PropertyPath);

                        if (serializedProperty == null)
                        {
                            propertiesToRemove.Add(property);
                            continue;
                        }

                        serializedProperty.colorValue = colorGroup.Color;
                        EditorUtility.SetDirty(serializedObject.targetObject);
                        serializedObject.ApplyModifiedProperties();

                        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), AssetDatabase.GUIDToAssetPath(guidObject.assetGUID));
                    }
                    else
                    {
                        propertiesToRemove.Add(property);
                    }
                }

                foreach (var property in propertiesToRemove)
                {
                    colorGroup.RemoveProperty(property);
                }
            }

            EditorSceneManager.OpenScene(initialScenePath);
        }

    }

    [System.Serializable]
    public struct PalettePreset
    {
        public List<Color> Colors;

        public PalettePreset(List<Color> colors)
        {
            Colors = colors;
        }
    }
}
