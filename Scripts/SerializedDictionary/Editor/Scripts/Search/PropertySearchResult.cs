using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TMPEffects.SerializedCollections.Editor.Search
{
    internal class PropertySearchResult
    {
        public SerializedProperty Property;

        public PropertySearchResult(SerializedProperty property)
        {
            Property = property;
        }

        public override string ToString()
        {
            return $"Found match in in {Property.propertyPath}";
        }
    }
}