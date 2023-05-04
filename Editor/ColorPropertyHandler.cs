using UnityEngine;
using UnityEditor;

namespace Colorlinker
{
    public static class ColorPropertyHandler
    {
        [InitializeOnLoadMethod]
        private static void Start()
        {
            EditorApplication.contextualPropertyMenu += OnPropertyContextMenu;
        }

        private static void OnPropertyContextMenu(GenericMenu menu, SerializedProperty property)
        {
            if (property.type != "Color") return;

            var propertyCopy = property.Copy();
            var guid = GlobalObjectId.GetGlobalObjectIdSlow(propertyCopy.serializedObject.targetObject);

            if (!(propertyCopy.serializedObject.targetObject is Component) && !(propertyCopy.serializedObject.targetObject is ScriptableObject)) return;

            foreach (var colorGroup in PaletteObject.instance.ColorGroups)
            {
                menu.AddItem(new GUIContent($"Link Color/{colorGroup.Name}"), colorGroup.Contains(guid.ToString(), propertyCopy.propertyPath), () =>
                {
                    var type = (guid.identifierType == 2) ? ColorProperty.Type.GameObject : ColorProperty.Type.Asset;
                    PaletteObject.instance.AddProperty(colorGroup, new ColorProperty(guid.ToString(), propertyCopy.propertyPath, type));
                    PaletteObject.instance.ApplyColors();
                });
            }
        }

        [CustomPropertyDrawer(typeof(Color))]
        public class ColorPropertyDrawer : PropertyDrawer
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                EditorGUI.BeginProperty(position, label, property);

                var guid = GlobalObjectId.GetGlobalObjectIdSlow(property.serializedObject.targetObject);
                foreach (var colorGroup in PaletteObject.instance.ColorGroups)
                {
                    if (colorGroup.Contains(guid.ToString(), property.propertyPath))
                    {
                        label.text = "→ " + property.displayName;
                        break;
                    }
                }

                position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
                property.colorValue = EditorGUI.ColorField(position, property.colorValue);

                EditorGUI.EndProperty();
            }
        }
    }
}
