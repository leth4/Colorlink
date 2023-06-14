using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Colorlink
{
    public static class ComponentContextMenu
    {
        public static Dictionary<Type, Dictionary<string, ColorGroup>> Buffer = new Dictionary<Type, Dictionary<string, ColorGroup>>();

        [MenuItem("CONTEXT/Component/Copy Color Links", false, 10000)]
        private static void CopyColorLinks(MenuCommand command)
        {
            var guid = GlobalObjectId.GetGlobalObjectIdSlow(command.context);
            var guidString = ColorPropertyHandler.VerifyGUIDForPrefabStage(guid.ToString());
            var type = command.context.GetType();
            var serializedObject = new SerializedObject(command.context);

            var properties = new List<string>();
            var serializedProperty = serializedObject.GetIterator();
            while (serializedProperty.NextVisible(true))
            {
                properties.Add(serializedProperty.propertyPath);
            }

            Buffer[type] = new Dictionary<string, ColorGroup>();

            foreach (var colorGroup in PaletteObject.instance.ColorGroups)
            {
                foreach (var property in properties)
                {
                    if (!colorGroup.Contains(guidString, property)) continue;
                    Buffer[type].Add(property, colorGroup);
                }
            }
        }

        [MenuItem("CONTEXT/Component/Paste Color Links", false, 10000)]
        private static void PasteColorLinks(MenuCommand command)
        {
            var type = command.context.GetType();
            if (!Buffer.ContainsKey(type)) return;

            var guid = GlobalObjectId.GetGlobalObjectIdSlow(command.context);
            var serializedObject = new SerializedObject(command.context);
            var guidString = ColorPropertyHandler.VerifyGUIDForPrefabStage(guid.ToString());
            var isPrefabStage = PrefabStageUtility.GetCurrentPrefabStage();

            var properties = new List<SerializedProperty>();
            var serializedProperty = serializedObject.GetIterator();
            while (serializedProperty.NextVisible(true))
            {
                properties.Add(serializedProperty.Copy());
                PaletteObject.instance.RemoveProperty(guidString, properties[^1].propertyPath);
            }

            foreach (var propertyLink in Buffer[type])
            {
                foreach (var property in properties)
                {
                    if (property.propertyPath != propertyLink.Key) continue;

                    var propertyType = (guid.identifierType == 2 && !isPrefabStage) ? ColorProperty.Type.GameObject : ColorProperty.Type.Asset;
                    PaletteObject.instance.AddProperty(propertyLink.Value, new ColorProperty(guidString, propertyLink.Key, propertyType));
                    PaletteObject.instance.ApplyColors(propertyType == ColorProperty.Type.Asset);
                }
            }
        }
    }
}
