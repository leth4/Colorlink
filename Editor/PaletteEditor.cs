using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Colorlink
{
    public class PaletteEditor : EditorWindow
    {
        private PaletteObject Palette => PaletteObject.instance;
        private Dictionary<ColorGroup, bool> _foldoutStates;
        private Vector2 _scrollPosition = Vector2.zero;
        private bool _showDetails;

        [MenuItem("Window/Palette", false, 10000)]
        public static void ShowWindow() => GetWindow<PaletteEditor>("Palette");


        private void OnEnable()
        {
            FillFoldoutStateDictionary();
            Undo.undoRedoPerformed += ApplyChanges;
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= ApplyChanges;
        }

        private void OnGUI()
        {
            Undo.RecordObject(Palette, "Changed Palette");

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, false, false);

            for (int i = 0; i < Palette.ColorGroups.Count; i++)
            {
                GUILayout.BeginHorizontal();
                DrawColorGroup(Palette.ColorGroups[i]);
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(3);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Plus")))
            {
                Palette.AddColorGroup();
                FillFoldoutStateDictionary();
            }

            if (Palette.ColorGroups.Count != 0)
            {
                if (GUILayout.Button(EditorGUIUtility.IconContent("d_SaveAs"))) Palette.SavePreset();
                if (GUILayout.Button(EditorGUIUtility.IconContent(_showDetails ? "d_SceneViewVisibility" : "ViewToolOrbit On"))) _showDetails = !_showDetails;
            }

            GUILayout.EndHorizontal();

            if (GUILayout.Button("Apply to Assets")) Palette.ApplyColors(true);
            if (GUILayout.Button("Apply to all Scenes")) Palette.ApplyColorsOnAllScenes();

            GUILayout.Space(3);

            using (var vertical = new EditorGUILayout.VerticalScope())
            {
                CreateDropArea(vertical.rect, () =>
                {
                    foreach (var draggedObject in DragAndDrop.objectReferences)
                    {
                        if (!(draggedObject is Texture2D)) continue;
                        Palette.AddPreset(ColorsFromImage((Texture2D)draggedObject));
                    }
                });

                for (int i = 0; i < Palette.Presets.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    DrawPalettePreset(Palette.Presets[i]);
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.EndScrollView();

            if (GUI.changed) ApplyChanges();
        }

        private void ApplyChanges()
        {
            Palette.ApplyColors();
            Palette.SaveChanges();
        }

        private void FillFoldoutStateDictionary()
        {
            _foldoutStates = new Dictionary<ColorGroup, bool>();
            foreach (var colorGroup in Palette.ColorGroups)
                _foldoutStates.Add(colorGroup, false);
        }

        public void DrawColorGroup(ColorGroup colorGroup)
        {
            if (_showDetails)
                GUILayout.BeginVertical(new GUIStyle("Box") { padding = new RectOffset(1, 1, 0, 1) });
            else
                GUILayout.BeginVertical();

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            if (_showDetails) _foldoutStates[colorGroup] = EditorGUILayout.Toggle(_foldoutStates[colorGroup], "foldout", GUILayout.Width(15));
            colorGroup.Color = EditorGUILayout.ColorField(colorGroup.Color, GUILayout.MaxWidth(80), GUILayout.Height(20));
            GUILayout.Space(2);
            colorGroup.Name = EditorGUILayout.TextField(colorGroup.Name, GUILayout.Height(22));

            GUILayout.EndHorizontal();

            if (_showDetails)
            {
                using (var vertical = new EditorGUILayout.VerticalScope())
                {
                    CreateDropArea(vertical.rect, () =>
                    {
                        var data = DragAndDrop.GetGenericData("property");
                        if (data == null) return;
                        if (colorGroup.Contains(((ColorProperty)data).GuidString, ((ColorProperty)data).PropertyPath)) return;

                        Palette.AddProperty(colorGroup, (ColorProperty)data);
                        Palette.ApplyColors();
                    });

                    GUILayout.Space(5);

                    if (_foldoutStates[colorGroup])
                    {

                        for (int i = 0; i < colorGroup.Properties.Count; i++)
                        {
                            DrawColorProperty(colorGroup.Properties[i], colorGroup);
                        }
                    }
                }
            }

            GUILayout.EndVertical();

            var buttonStyle = new GUIStyle("Button") { fixedWidth = 25, fixedHeight = 25, margin = new RectOffset(3, 3, 5, 0) };

            if (GUILayout.Button("↑", buttonStyle))
            {
                Palette.ChangeColorGroupIndex(colorGroup, -1);
            }
            if (GUILayout.Button("↓", buttonStyle))
            {
                Palette.ChangeColorGroupIndex(colorGroup, +1);
            }
            if (GUILayout.Button(EditorGUIUtility.IconContent("d_winbtn_win_close"), buttonStyle))
            {
                Palette.RemoveColorGroup(colorGroup);
                FillFoldoutStateDictionary();
            }

        }

        private void DrawColorProperty(ColorProperty property, ColorGroup colorGroup)
        {
            GUILayout.BeginHorizontal();

            using (var horizontal = new EditorGUILayout.HorizontalScope())
            {
                var evt = Event.current;
                if (evt.button == 0 && evt.isMouse && horizontal.rect.Contains(evt.mousePosition))
                {
                    DragAndDrop.SetGenericData("property", property);
                    DragAndDrop.StartDrag("");
                }

                if (property.ObjectType == ColorProperty.Type.Material) GUILayout.Label(EditorGUIUtility.IconContent("Material On Icon"), GUILayout.Height(20), GUILayout.Width(20));
                if (property.ObjectType == ColorProperty.Type.GameObject) GUILayout.Label(EditorGUIUtility.IconContent("GameObject On Icon"), GUILayout.Height(20), GUILayout.Width(20));
                if (property.ObjectType == ColorProperty.Type.Asset) GUILayout.Label(EditorGUIUtility.IconContent("d_Prefab On Icon"), GUILayout.Height(20), GUILayout.Width(20));

                GUILayout.BeginHorizontal("HelpBox", GUILayout.MaxWidth(200));
                GUILayout.Label(property.ObjectName);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal("HelpBox", GUILayout.MaxWidth(200));
                GUILayout.Label(property.PropertyPath);
                GUILayout.EndHorizontal();
            }
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(EditorGUIUtility.IconContent("ViewToolOrbit On"), GUILayout.Width(30), GUILayout.Height(22)))
            {
                if (GlobalObjectId.TryParse(property.GuidString, out GlobalObjectId guidObject))
                    EditorGUIUtility.PingObject(GlobalObjectId.GlobalObjectIdentifierToObjectSlow(guidObject));
            }

            if (GUILayout.Button(EditorGUIUtility.IconContent("d_winbtn_win_close"), GUILayout.Width(30), GUILayout.Height(22)))
            {
                colorGroup.RemoveProperty(property);
            }

            GUILayout.EndHorizontal();
        }

        private void DrawPalettePreset(PalettePreset preset)
        {
            var colors = preset.Colors;

            GUILayout.BeginVertical("HelpBox");

            var colorBoxStyle = new GUIStyle("box")
            {
                padding = new RectOffset(2, 2, 2, 2),
                margin = new RectOffset(0, 0, 0, 0),
                fixedHeight = 20,
                fixedWidth = 20
            };

            var squares = Mathf.Clamp(Mathf.RoundToInt((EditorGUIUtility.currentViewWidth - 100) / 20), 1, 100);
            GUILayout.BeginHorizontal();
            for (int i = 0; i < colors.Count; i++)
            {
                GUILayout.Box(TextureFromColor(colors[i], 20), colorBoxStyle);
                if (i % squares == squares - 1)
                {
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            if (GUILayout.Button(EditorGUIUtility.IconContent("d_scenepicking_pickable_hover"), GUILayout.Width(25), GUILayout.Height(25)))
            {
                for (int i = 0; i < Mathf.Min(Palette.ColorGroups.Count, colors.Count); i++)
                {
                    Palette.ColorGroups[i].Color = colors[i];
                }
            }

            if (GUILayout.Button(EditorGUIUtility.IconContent("d_winbtn_win_close"), GUILayout.Width(25), GUILayout.Height(25)))
            {
                Palette.Presets.Remove(preset);
            }
        }

        private void CreateDropArea(Rect rect, Action callback)
        {
            var evt = Event.current;

            if (evt.type != EventType.DragPerform && evt.type != EventType.DragUpdated) return;
            if (!rect.Contains(evt.mousePosition)) return;

            DragAndDrop.visualMode = DragAndDropVisualMode.Move;

            if (evt.type != EventType.DragPerform) return;

            DragAndDrop.AcceptDrag();

            callback.Invoke();
        }

        private static List<Color> ColorsFromImage(Texture2D image)
        {
            var colors = new List<Color>();
            var pixels = image.GetPixels();

            foreach (var pixel in pixels)
            {
                if (!colors.Contains(pixel)) colors.Add(pixel);
                if (colors.Count >= 30)
                {
                    Debug.Log("Selected image has too many unique colors!");
                    break;
                }
            }

            return colors;
        }

        private static Texture2D TextureFromColor(Color color, int size)
        {
            Color[] pix = new Color[size * size];
            for (int i = 0; i < pix.Length; i++)
            {
                pix[i] = color;
            }
            Texture2D texture = new Texture2D(size, size);
            texture.SetPixels(pix);
            texture.Apply();
            return texture;
        }
    }
}