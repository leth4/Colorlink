using System;
using System.Collections.Generic;
using UnityEditor;

namespace Colorlink
{
    public static class ComponentContextMenu
    {
        public static Dictionary<Type, Dictionary<string, ColorGroup>> Buffer = new Dictionary<Type, Dictionary<string, ColorGroup>>();

        [MenuItem("CONTEXT/Component/Copy Color Links", false, 10000)]
        private static void CopyColorLinks(MenuCommand command)
        {
            var guid = GlobalObjectId.GetGlobalObjectIdSlow(command.context);
            var type = command.context.GetType();
            var serializedObject = new SerializedObject(command.context);

            var properties = new List<string>();
            var serializedProperty = serializedObject.GetIterator();
            while (serializedProperty.NextVisible(true))
            {
                properties.Add(serializedProperty.propertyPath);
            }

            foreach (var colorGroup in PaletteObject.instance.ColorGroups)
            {
                foreach (var property in properties)
                {
                    if (!colorGroup.Contains(guid.ToString(), property)) continue;

                    if (!Buffer.ContainsKey(type))
                        Buffer.Add(type, new Dictionary<string, ColorGroup>());

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

            var properties = new List<SerializedProperty>();
            var serializedProperty = serializedObject.GetIterator();
            while (serializedProperty.NextVisible(true))
            {
                properties.Add(serializedProperty.Copy());
            }

            foreach (var propertyLink in Buffer[type])
            {
                foreach (var property in properties)
                {
                    if (property.propertyPath != propertyLink.Key) continue;

                    var propertyType = (guid.identifierType == 2) ? ColorProperty.Type.GameObject : ColorProperty.Type.Asset;
                    PaletteObject.instance.AddProperty(propertyLink.Value, new ColorProperty(guid.ToString(), propertyLink.Key, propertyType));
                    PaletteObject.instance.ApplyColors();
                }
            }
        }
    }
}
