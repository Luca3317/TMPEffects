using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace AYellowpaper.SerializedCollections.Editor.Search
{
    public class SearchResultEntry
    {
        public readonly int Index;
        public readonly SerializedProperty Property;
        public readonly IEnumerable<PropertySearchResult> MatchingResults;

        public SearchResultEntry(int index, SerializedProperty property, IEnumerable<PropertySearchResult> matchingResults)
        {
            Index = index;
            Property = property;
            MatchingResults = matchingResults;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{Index}: {Property.propertyPath}");
            foreach (var matchingResult in MatchingResults)
                sb.AppendLine(matchingResult.ToString());
            return sb.ToString();
        }
    }
}