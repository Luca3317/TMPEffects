using System.Globalization;
using UnityEditor;

namespace AYellowpaper.SerializedCollections.Editor.Search
{
    public class StringMatcher : Matcher
    {
        public override bool IsMatch(SerializedProperty property)
        {
            if ((property.propertyType is SerializedPropertyType.String or SerializedPropertyType.Character) && property.stringValue.Contains(SearchString, System.StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }
    }
}