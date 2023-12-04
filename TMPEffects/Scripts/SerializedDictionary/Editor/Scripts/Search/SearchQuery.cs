using System.Collections.Generic;
using UnityEditor;

namespace AYellowpaper.SerializedCollections.Editor.Search
{
    public class SearchQuery
    {
        public string SearchString
        {
            get => _text;
            set
            {
                if (_text == value)
                    return;

                _text = value;
                foreach (var matcher in _matchers)
                    matcher.Prepare(_text);
            }
        }

        private IEnumerable<Matcher> _matchers;
        private string _text;

        public SearchQuery(IEnumerable<Matcher> matchers)
        {
            _matchers = matchers;
        }

        public List<PropertySearchResult> ApplyToProperty(SerializedProperty property)
        {
            TryGetMatchingProperties(property.Copy(), out var properties);
            return properties;
        }

        public IEnumerable<SearchResultEntry> ApplyToArrayProperty(SerializedProperty property)
        {
            int arrayCount = property.arraySize;
            for (int i = 0; i < arrayCount; i++)
            {
                var prop = property.GetArrayElementAtIndex(i);
                if (TryGetMatchingProperties(prop.Copy(), out var properties))
                    yield return new SearchResultEntry(i, prop, properties);
            }
        }

        private bool TryGetMatchingProperties(SerializedProperty property, out List<PropertySearchResult> matchingProperties)
        {
            matchingProperties = null;
            foreach (var child in SCEditorUtility.GetChildren(property, true))
            {
                foreach (var matcher in _matchers)
                {
                    if (matcher.IsMatch(child))
                    {
                        if (matchingProperties == null)
                            matchingProperties = new();
                        matchingProperties.Add(new PropertySearchResult(child.Copy()));
                    }
                }
            }

            return matchingProperties != null;
        }
    }
}