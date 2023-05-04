using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Colorlinker
{
    public class MaterialPropertyWizard : ScriptableWizard
    {
        private static Material _activeMaterial;

        private string _guidString;
        private List<string> _colorProperties = new List<string>();
        private List<int> _selectors = new List<int>();
        private List<string> _groups = new List<string>() { "None" };

        [MenuItem("CONTEXT/Material/Link Colors", false, 0)]
        public static void CreateWizard(MenuCommand command)
        {
            if (_activeMaterial != null)
            {
                Debug.LogWarning("Please close an active material property wizard before opening a new one");
                return;
            }

            _activeMaterial = (Material)command.context;

            DisplayWizard<MaterialPropertyWizard>("Link Colors", "Link");
        }

        private void OnEnable()
        {
            _guidString = GlobalObjectId.GetGlobalObjectIdSlow(_activeMaterial).ToString();

            var materialProperties = MaterialEditor.GetMaterialProperties(new Material[] { _activeMaterial });
            foreach (var property in materialProperties)
            {
                if (property.type == MaterialProperty.PropType.Color)
                {
                    _colorProperties.Add(property.name);
                    _selectors.Add(0);
                }
            }

            for (int i = 0; i < PaletteObject.instance.ColorGroups.Count; i++)
            {
                _groups.Add(PaletteObject.instance.ColorGroups[i].Name);
                foreach (var property in PaletteObject.instance.ColorGroups[i].Properties)
                {
                    if (property.GuidString != _guidString) continue;
                    _selectors[_colorProperties.IndexOf(property.PropertyPath)] = i + 1;
                }
            }
        }

        protected override bool DrawWizardGUI()
        {
            for (int i = 0; i < _colorProperties.Count; i++)
            {
                _selectors[i] = EditorGUILayout.Popup(_colorProperties[i], _selectors[i], _groups.ToArray());
            }

            return true;
        }

        private void OnWizardCreate()
        {
            for (int i = 0; i < _selectors.Count; i++)
            {
                if (_selectors[i] == 0)
                {
                    foreach (var colorGroup in PaletteObject.instance.ColorGroups) colorGroup.RemoveProperty(_guidString, _colorProperties[i]);
                    continue;
                }
                PaletteObject.instance.AddProperty(PaletteObject.instance.ColorGroups[_selectors[i] - 1], new ColorProperty(_guidString, _colorProperties[i], ColorProperty.Type.Material));
            }
            PaletteObject.instance.ApplyColors();
        }

        private void OnDestroy()
        {
            _activeMaterial = null;
        }
    }
}