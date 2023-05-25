using System.Collections.Generic;
using UnityEngine;

namespace Colorlink
{
    [System.Serializable]
    public class ColorGroup
    {
        public string Name;
        public Color Color = Color.white;
        public List<ColorProperty> Properties = new List<ColorProperty>();

        public void RemoveProperty(ColorProperty property)
        {
            RemoveProperty(property.GuidString, property.PropertyPath);
        }

        public void RemoveProperty(string guidString, string propertyPath)
        {
            for (int i = 0; i < Properties.Count; i++)
            {
                if (Properties[i].GuidString != guidString) continue;
                if (Properties[i].PropertyPath != propertyPath) continue;
                Properties.RemoveAt(i);
                break;
            }
        }

        public void ToggleProperty(ColorProperty property)
        {
            if (Contains(property.GuidString, property.PropertyPath))
            {
                if (property.ObjectType != ColorProperty.Type.Material)
                    RemoveProperty(property);
                return;
            }
            Properties.Add(property);
        }

        public bool Contains(string guid, string propertyPath)
        {
            foreach (var property in Properties)
            {
                if (property.GuidString == guid && property.PropertyPath == propertyPath)
                    return true;
            }
            return false;
        }
    }

    [System.Serializable]
    public class ColorProperty
    {
        public string GuidString;
        public string PropertyPath;
        public string ObjectName;
        public Type ObjectType;

        public ColorProperty(string guid, string name, Type path)
        {
            GuidString = guid;
            PropertyPath = name;
            ObjectName = "";
            ObjectType = path;
        }

        public enum Type
        {
            GameObject,
            Material,
            Asset
        }
    }
}
