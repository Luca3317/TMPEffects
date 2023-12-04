using AYellowpaper.SerializedCollections.Editor.Search;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System;
using static AYellowpaper.SerializedCollections.Editor.SerializedDictionaryDrawer;

namespace AYellowpaper.SerializedCollections.Editor.States
{
    internal class SearchListState : ListState
    {
        public override int ListSize => _searchResults.Count;
        public override string NoElementsText => "No Results";
        public bool OnlyShowMatchingValues { get; set; }

        private string _lastSearch = string.Empty;
        private List<SearchResultEntry> _searchResults = new List<SearchResultEntry>();
        private HashSet<string> _foundProperties;
        private Color _previousColor;

        public SearchListState(SerializedDictionaryInstanceDrawer serializedDictionaryDrawer) : base(serializedDictionaryDrawer)
        {
        }

        public override void DrawElement(Rect rect, SerializedProperty property, DisplayType displayType)
        {
            SerializedDictionaryInstanceDrawer.DrawElement(rect, property, displayType, BeforeDrawingProperty, AfterDrawingProperty);
        }

        private void BeforeDrawingProperty(SerializedProperty obj)
        {
            _previousColor = GUI.backgroundColor;
            if (_foundProperties.Contains(obj.propertyPath))
            {
                GUI.backgroundColor = Color.blue;
            }
        }

        private void AfterDrawingProperty(SerializedProperty obj)
        {
            GUI.backgroundColor = _previousColor;
        }

        public override void OnEnter()
        {
            Drawer.ReorderableList.draggable = false;
            UpdateSearch();
        }

        public override void OnExit()
        {
        }

        public override ListState OnUpdate()
        {
            if (Drawer.SearchText.Length == 0)
                return Drawer.DefaultState;

            UpdateSearch();

            return this;
        }

        private void UpdateSearch()
        {
            if (_lastSearch != Drawer.SearchText)
            {
                _lastSearch = Drawer.SearchText;
                PerformSearch(Drawer.SearchText);
            }
        }

        public void PerformSearch(string searchString)
        {
            var query = new SearchQuery(Matchers.RegisteredMatchers);
            query.SearchString = searchString;
            _searchResults.Clear();
            _searchResults.AddRange(query.ApplyToArrayProperty(Drawer.ListProperty));

            _foundProperties = _searchResults.SelectMany(x => x.MatchingResults, (x, y) => y.Property.propertyPath).ToHashSet();
        }

        public override SerializedProperty GetPropertyAtIndex(int index)
        {
            return _searchResults[index].Property;
        }

        public override float GetHeightAtIndex(int index, bool drawKeyAsList, bool drawValueAsList)
        {
            return base.GetHeightAtIndex(index, drawKeyAsList, drawValueAsList);
        }

        public override void RemoveElementAt(int index)
        {
            var indexToDelete = _searchResults[index].Index;
            Drawer.ListProperty.DeleteArrayElementAtIndex(indexToDelete);
            PerformSearch(_lastSearch);
        }

        public override void InserElementAt(int index)
        {
            var indexToAdd = _searchResults[index].Index;
            Drawer.ListProperty.InsertArrayElementAtIndex(indexToAdd);
            PerformSearch(_lastSearch);
        }
    }
}