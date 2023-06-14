using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Colorlink
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

            var type = (guid.identifierType == 2 && !PrefabStageUtility.GetCurrentPrefabStage()) ? ColorProperty.Type.GameObject : ColorProperty.Type.Asset;
            var guidString = VerifyGUIDForPrefabStage(guid.ToString());

            foreach (var colorGroup in PaletteObject.instance.ColorGroups)
            {
                menu.AddItem(new GUIContent($"Link Color/{colorGroup.Name}"), colorGroup.Contains(guidString, propertyCopy.propertyPath), () =>
                {
                    PaletteObject.instance.AddProperty(colorGroup, new ColorProperty(guidString, propertyCopy.propertyPath, type));
                    PaletteObject.instance.ApplyColors(type == ColorProperty.Type.Asset);
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
                var guidString = VerifyGUIDForPrefabStage(guid.ToString());
                foreach (var colorGroup in PaletteObject.instance.ColorGroups)
                {
                    if (colorGroup.Contains(guidString, property.propertyPath))
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

        private static string VerifyGUIDForPrefabStage(string guid)
        {
            if (PrefabStageUtility.GetCurrentPrefabStage() == null) return guid;

            var prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabStageUtility.GetCurrentPrefabStage().assetPath);
            var prefabGUID = GlobalObjectId.GetGlobalObjectIdSlow(prefabAsset);
            return guid.ToString().Replace("-2-", "-1-").Replace("00000000000000000000000000000000", prefabGUID.assetGUID.ToString());
        }
    }
}
