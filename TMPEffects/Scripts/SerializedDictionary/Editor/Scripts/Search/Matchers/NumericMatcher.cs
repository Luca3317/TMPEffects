using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace AYellowpaper.SerializedCollections.Editor.Search
{
    public class NumericMatcher : Matcher
    {
        public override string ProcessSearchString(string searchString)
        {
            return searchString.Replace(',', '.');
        }

        public override bool IsMatch(SerializedProperty property)
        {
            if (property.propertyType == SerializedPropertyType.Float)
            {
                return IsFloatMatch(property.floatValue);
            }
            else if (property.propertyType == SerializedPropertyType.Integer)
            {
                return IsIntMatch(property.intValue);
            }
            else if (property.propertyType == SerializedPropertyType.Quaternion)
            {
                var quat = property.quaternionValue;
                return IsFloatMatch(quat.x) || IsFloatMatch(quat.y) || IsFloatMatch(quat.z) || IsFloatMatch(quat.w);
            }
            else if (property.propertyType == SerializedPropertyType.Bounds)
            {
                var bounds = property.boundsValue;
                return IsVector3Match(bounds.center) || IsVector3Match(bounds.size);
            }
            else if (property.propertyType == SerializedPropertyType.BoundsInt)
            {
                var bounds = property.boundsIntValue;
                return IsVector3Match(bounds.center) || IsVector3IntMatch(bounds.size);
            }
            else if (property.propertyType == SerializedPropertyType.Rect)
            {
                var rect = property.rectValue;
                return IsVector2Match(rect.size) || IsVector2Match(rect.position);
            }
            else if (property.propertyType == SerializedPropertyType.RectInt)
            {
                var rect = property.rectIntValue;
                return IsVector2IntMatch(rect.size) || IsVector2IntMatch(rect.position);
            }
            else if (property.propertyType == SerializedPropertyType.Vector2)
            {
                return IsVector2Match(property.vector2Value);
            }
            else if (property.propertyType == SerializedPropertyType.Vector2Int)
            {
                return IsVector2IntMatch(property.vector2IntValue);
            }
            else if (property.propertyType == SerializedPropertyType.Vector3)
            {
                return IsVector3Match(property.vector3Value);
            }
            else if (property.propertyType == SerializedPropertyType.Vector3Int)
            {
                return IsVector3IntMatch(property.vector3IntValue);
            }
            else if (property.propertyType == SerializedPropertyType.Vector4)
            {
                return IsVector4Match(property.vector4Value);
            }
            return false;
        }

        private bool IsFloatMatch(float val)
        {
            var str = val.ToString(CultureInfo.InvariantCulture);
            return str.Contains(SearchString, System.StringComparison.OrdinalIgnoreCase);
        }

        private bool IsIntMatch(int val)
        {
            var str = val.ToString(CultureInfo.InvariantCulture);
            return str.Contains(SearchString, System.StringComparison.OrdinalIgnoreCase);
        }

        private bool IsVector2Match(Vector2 vector)
        {
            return IsFloatMatch(vector.x) || IsFloatMatch(vector.y);
        }

        private bool IsVector2IntMatch(Vector2Int vector)
        {
            return IsIntMatch(vector.x) || IsIntMatch(vector.y);
        }

        private bool IsVector3Match(Vector3 vector)
        {
            return IsFloatMatch(vector.x) || IsFloatMatch(vector.y) || IsFloatMatch(vector.z);
        }

        private bool IsVector3IntMatch(Vector3Int vector)
        {
            return IsIntMatch(vector.x) || IsIntMatch(vector.y) || IsIntMatch(vector.z);
        }

        private bool IsVector4Match(Vector4 vector)
        {
            return IsFloatMatch(vector.x) || IsFloatMatch(vector.y) || IsFloatMatch(vector.z) || IsFloatMatch(vector.w);
        }
    }
}