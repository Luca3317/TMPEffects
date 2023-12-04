using AYellowpaper.SerializedCollections.Editor.Data;
using AYellowpaper.SerializedCollections.Editor.States;
using AYellowpaper.SerializedCollections.KeysGenerators;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine;

namespace AYellowpaper.SerializedCollections.Editor
{
    public class SerializedDictionaryInstanceDrawer
    {
        private FieldInfo _fieldInfo;
        private ReorderableList _unexpandedList;
        private SingleEditingData _singleEditingData;
        private FieldInfo _keyFieldInfo;
        private GUIContent _label;
        private Rect _totalRect;
        private GUIStyle _keyValueStyle;
        private SerializedDictionaryAttribute _dictionaryAttribute;
        private PropertyData _propertyData;
        private bool _propertyListSettingsInitialized = false;
        private List<int> _pagedIndices;
        private PagingElement _pagingElement;
        private int _lastListSize = -1;
        private IReadOnlyList<KeyListGeneratorData> _keyGeneratorsWithoutWindow;
        private IReadOnlyList<KeyListGeneratorData> _keyGeneratorsWithWindow;
        private SearchField _searchField;
        private GUIContent _shortDetailsContent;
        private GUIContent _detailsContent;
        private bool _showSearchBar = false;
        private ListState _activeState;

        internal ReorderableList ReorderableList { get; private set; }
        internal SerializedProperty ListProperty { get; private set; }
        internal string SearchText { get; private set; } = string.Empty;
        internal SearchListState SearchState { get; private set; }
        internal DefaultListState DefaultState { get; private set; }

        private class SingleEditingData
        {
            public bool IsValid => LookupTable != null;
            public IKeyable LookupTable;

            public void Invalidate()
            {
                LookupTable = null;
            }
        }

        public SerializedDictionaryInstanceDrawer(SerializedProperty property, FieldInfo fieldInfo)
        {
            _fieldInfo = fieldInfo;
            ListProperty = property.FindPropertyRelative(SerializedDictionaryDrawer.SerializedListName);

            _keyValueStyle = new GUIStyle(EditorStyles.toolbarButton);
            _keyValueStyle.padding = new RectOffset(0, 0, 0, 0);
            _keyValueStyle.border = new RectOffset(0, 0, 0, 0);
            _keyValueStyle.alignment = TextAnchor.MiddleCenter;

            DefaultState = new DefaultListState(this);
            SearchState = new SearchListState(this);
            _activeState = DefaultState;

            _dictionaryAttribute = _fieldInfo.GetCustomAttribute<SerializedDictionaryAttribute>();

            _propertyData = SCEditorUtility.GetPropertyData(ListProperty);
            _propertyData.GetElementData(SCEditorUtility.KeyFlag).Settings.DisplayName = _dictionaryAttribute?.KeyName ?? "Key";
            _propertyData.GetElementData(SCEditorUtility.ValueFlag).Settings.DisplayName = _dictionaryAttribute?.ValueName ?? "Value";
            SavePropertyData();

            _pagingElement = new PagingElement();
            _pagedIndices = new List<int>();
            UpdatePaging();

            ReorderableList = MakeList();
            _unexpandedList = MakeUnexpandedList();
            _searchField = new SearchField();

            var listField = _fieldInfo.FieldType.GetField(SerializedDictionaryDrawer.SerializedListName, BindingFlags.Instance | BindingFlags.NonPublic);
            var entryType = listField.FieldType.GetGenericArguments()[0];
            _keyFieldInfo = entryType.GetField(SerializedDictionaryDrawer.KeyName);

            _singleEditingData = new SingleEditingData();

            var keyGenerators = KeyListGeneratorCache.GetPopulatorsForType(_keyFieldInfo.FieldType);
            _keyGeneratorsWithWindow = keyGenerators.Where(x => x.NeedsWindow).ToList();
            _keyGeneratorsWithoutWindow = keyGenerators.Where(x => !x.NeedsWindow).ToList();

            UpdateAfterInput();
        }

        public void OnGUI(Rect position, GUIContent label)
        {
            _totalRect = position;
            _label = new GUIContent(label);

            EditorGUI.BeginChangeCheck();
            DoList(position);
            if (EditorGUI.EndChangeCheck())
            {
                ListProperty.serializedObject.ApplyModifiedProperties();
            }
        }

        public float GetPropertyHeight(GUIContent label)
        {
            if (!ListProperty.isExpanded)
                return SerializedDictionaryDrawer.TopHeaderClipHeight;

            return ReorderableList.GetHeight();
        }

        private void DoList(Rect position)
        {
            if (ListProperty.isExpanded)
                ReorderableList.DoList(position);
            else
            {
                using (new GUI.ClipScope(new Rect(0, position.y, position.width + position.x, SerializedDictionaryDrawer.TopHeaderClipHeight)))
                {
                    _unexpandedList.DoList(position.WithY(0));
                }
            }
        }

        private void ProcessState()
        {
            var newState = _activeState.OnUpdate();
            if (newState != null && newState != _activeState)
            {
                _activeState.OnExit();
                _activeState = newState;
                newState.OnEnter();
            }
        }

        private SerializedProperty GetElementProperty(SerializedProperty property, bool fieldFlag)
        {
            return property.FindPropertyRelative(fieldFlag == SerializedDictionaryDrawer.KeyFlag ? SerializedDictionaryDrawer.KeyName : SerializedDictionaryDrawer.ValueName);
        }

        internal static float CalculateHeightOfElement(SerializedProperty property, bool drawKeyAsList, bool drawValueAsList)
        {
            SerializedProperty keyProperty = property.FindPropertyRelative(SerializedDictionaryDrawer.KeyName);
            SerializedProperty valueProperty = property.FindPropertyRelative(SerializedDictionaryDrawer.ValueName);
            return Mathf.Max(SCEditorUtility.CalculateHeight(keyProperty, drawKeyAsList), SCEditorUtility.CalculateHeight(valueProperty, drawValueAsList));
        }

        private void UpdateAfterInput()
        {
            InitializeSettingsIfNeeded();
            ProcessState();
            CheckPaging();
            var elementsPerPage = EditorUserSettings.Get().ElementsPerPage;
            int pageCount = Mathf.Max(1, Mathf.CeilToInt((float)DefaultState.ListSize / elementsPerPage));
            ToggleSearchBar(_propertyData.AlwaysShowSearch ? true : SCEditorUtility.ShouldShowSearch(pageCount));
        }

        private void InitializeSettingsIfNeeded()
        {
            void InitializeSettings(bool fieldFlag)
            {
                var genericArgs = _fieldInfo.FieldType.GetGenericArguments();
                var firstProperty = ListProperty.GetArrayElementAtIndex(0);
                var keySettings = CreateDisplaySettings(GetElementProperty(firstProperty, fieldFlag), genericArgs[fieldFlag == SCEditorUtility.KeyFlag ? 0 : 1]);
                var settings = _propertyData.GetElementData(fieldFlag).Settings;
                settings.DisplayType = keySettings.displayType;
                settings.HasListDrawerToggle = keySettings.canToggleListDrawer;
            }

            if (!_propertyListSettingsInitialized && ListProperty.minArraySize > 0)
            {
                _propertyListSettingsInitialized = true;
                InitializeSettings(SCEditorUtility.KeyFlag);
                InitializeSettings(SCEditorUtility.ValueFlag);
                SavePropertyData();
            }
        }

        private void CheckPaging()
        {
            // TODO: Is there a better solution to check for Revert/delete/add?
            if (_lastListSize != _activeState.ListSize)
            {
                _lastListSize = _activeState.ListSize;
                UpdateSingleEditing();
                UpdatePaging();
            }
        }

        private void SavePropertyData()
        {
            SCEditorUtility.SavePropertyData(ListProperty, _propertyData);
        }

        private void UpdateSingleEditing()
        {
            if (ListProperty.serializedObject.isEditingMultipleObjects && _singleEditingData.IsValid)
                _singleEditingData.Invalidate();
            else if (!ListProperty.serializedObject.isEditingMultipleObjects && !_singleEditingData.IsValid)
            {
                var dictionary = SCEditorUtility.GetPropertyValue(ListProperty, ListProperty.serializedObject.targetObject);
                _singleEditingData.LookupTable = GetLookupTable(dictionary);
            }
        }

        private IKeyable GetLookupTable(object dictionary)
        {
            var propInfo = dictionary.GetType().GetProperty(SerializedDictionaryDrawer.LookupTableName, BindingFlags.Instance | BindingFlags.NonPublic);
            return (IKeyable)propInfo.GetValue(dictionary);
        }

        private void UpdatePaging()
        {
            var elementsPerPage = EditorUserSettings.Get().ElementsPerPage;
            _pagingElement.PageCount = Mathf.Max(1, Mathf.CeilToInt((float)_activeState.ListSize / elementsPerPage));

            _pagedIndices.Clear();
            _pagedIndices.Capacity = Mathf.Max(elementsPerPage, _pagedIndices.Capacity);

            int startIndex = (_pagingElement.Page - 1) * elementsPerPage;
            int endIndex = Mathf.Min(startIndex + elementsPerPage, _activeState.ListSize);
            for (int i = startIndex; i < endIndex; i++)
                _pagedIndices.Add(i);

            string shortDetailsString = (_activeState.ListSize + " " + (_pagedIndices.Count == 1 ? "Element" : "Elements"));
            string detailsString = _pagingElement.PageCount > 1
                ? $"{_pagedIndices[0] + 1}..{_pagedIndices.Last() + 1} / {_activeState.ListSize} Elements"
                : shortDetailsString;
            _detailsContent = new GUIContent(detailsString);
            _shortDetailsContent = new GUIContent(shortDetailsString);
        }

        private ReorderableList MakeList()
        {
            var list = new ReorderableList(_pagedIndices, typeof(int), true, true, true, true);
            list.onAddCallback += OnAdd;
            list.onRemoveCallback += OnRemove;
            list.onReorderCallbackWithDetails += OnReorder;
            list.drawElementCallback += OnDrawElement;
            list.elementHeightCallback += OnGetElementHeight;
            list.drawHeaderCallback += OnDrawHeader;
            list.drawNoneElementCallback += OnDrawNoneElement;
            return list;
        }

        private ReorderableList MakeUnexpandedList()
        {
            var list = new ReorderableList(SerializedDictionaryDrawer.NoEntriesList, typeof(int));
            list.drawHeaderCallback = DrawUnexpandedHeader;
            return list;
        }

        private void ToggleSearchBar(bool flag)
        {
            _showSearchBar = flag;
            ReorderableList.headerHeight = SerializedDictionaryDrawer.TopHeaderClipHeight + SerializedDictionaryDrawer.KeyValueHeaderHeight + (_showSearchBar ? SerializedDictionaryDrawer.SearchHeaderHeight : 0);
            if (!_showSearchBar)
            {
                if (_searchField.HasFocus())
                    GUI.FocusControl(null);
                SearchText = string.Empty;
            }
        }

        private void OnDrawNoneElement(Rect rect)
        {
            EditorGUI.LabelField(rect, EditorGUIUtility.TrTextContent(_activeState.NoElementsText));
        }

        private (DisplayType displayType, bool canToggleListDrawer) CreateDisplaySettings(SerializedProperty property, Type type)
        {
            bool hasCustomEditor = SCEditorUtility.HasDrawerForType(type);
            bool isGenericWithChildren = property.propertyType == SerializedPropertyType.Generic && property.hasVisibleChildren;
            bool isArray = property.isArray && property.propertyType != SerializedPropertyType.String;
            bool canToggleListDrawer = isArray || (isGenericWithChildren && hasCustomEditor);
            DisplayType displayType = DisplayType.PropertyNoLabel;
            if (canToggleListDrawer)
                displayType = DisplayType.Property;
            else if (!isArray && isGenericWithChildren && !hasCustomEditor)
                displayType = DisplayType.List;
            return (displayType, canToggleListDrawer);
        }

        private void DrawUnexpandedHeader(Rect rect)
        {
            EditorGUI.BeginProperty(rect, _label, ListProperty);
            ListProperty.isExpanded = EditorGUI.Foldout(rect.WithX(rect.x - 5), ListProperty.isExpanded, _label, true);

            var detailsStyle = EditorStyles.miniLabel;
            var detailsRect = rect.AppendRight(0).AppendLeft(detailsStyle.CalcSize(_shortDetailsContent).x);
            GUI.Label(detailsRect, _shortDetailsContent, detailsStyle);

            EditorGUI.EndProperty();
        }

        private void DoPaging(Rect rect)
        {
            EditorGUI.BeginChangeCheck();
            _pagingElement.OnGUI(rect);
            if (EditorGUI.EndChangeCheck())
            {
                ReorderableList.ClearSelection();
                UpdatePaging();
            }
        }

        private void OnDrawHeader(Rect rect)
        {
            Rect topRect = rect.WithHeight(SerializedDictionaryDrawer.TopHeaderHeight);
            Rect adjustedTopRect = topRect.WithXAndWidth(_totalRect.x + 1, _totalRect.width - 1);

            DoMainHeader(adjustedTopRect.CutLeft(topRect.x - adjustedTopRect.x));
            if (_showSearchBar)
            {
                adjustedTopRect = adjustedTopRect.AppendDown(SerializedDictionaryDrawer.SearchHeaderHeight);
                DoSearch(adjustedTopRect);
            }
            DoKeyValueRect(adjustedTopRect.AppendDown(SerializedDictionaryDrawer.KeyValueHeaderHeight));

            UpdateAfterInput();
        }

        private void DoMainHeader(Rect rect)
        {
            Rect lastTopRect = rect.AppendRight(0).WithHeight(EditorGUIUtility.singleLineHeight);

            lastTopRect = lastTopRect.AppendLeft(20);
            DoOptionsButton(lastTopRect);
            lastTopRect = lastTopRect.AppendLeft(5);

            if (_pagingElement.PageCount > 1)
            {
                lastTopRect = lastTopRect.AppendLeft(_pagingElement.GetDesiredWidth());
                DoPaging(lastTopRect);
            }

            var detailsStyle = EditorStyles.miniLabel;
            lastTopRect = lastTopRect.AppendLeft(detailsStyle.CalcSize(_detailsContent).x, 5);
            GUI.Label(lastTopRect, _detailsContent, detailsStyle);

            if (!_singleEditingData.IsValid)
            {
                lastTopRect = lastTopRect.AppendLeft(lastTopRect.height + 5);
                var guicontent = EditorGUIUtility.TrIconContent(EditorGUIUtility.Load("d_console.infoicon") as Texture, "Conflict checking, duplicate key removal and populators not supported in multi object editing mode.");
                GUI.Label(lastTopRect, guicontent);
            }

            EditorGUI.BeginProperty(rect, _label, ListProperty);
            ListProperty.isExpanded = EditorGUI.Foldout(rect.WithXAndWidth(rect.x - 5, lastTopRect.x - rect.x), ListProperty.isExpanded, _label, true);
            EditorGUI.EndProperty();
        }

        private void DoOptionsButton(Rect rect)
        {
            var screenRect = GUIUtility.GUIToScreenRect(rect);
            if (GUI.Button(rect, EditorGUIUtility.IconContent("pane options@2x"), EditorStyles.iconButton))
            {
                var gm = new GenericMenu();
                SCEditorUtility.AddGenericMenuItem(gm, false, ListProperty.minArraySize > 0, new GUIContent("Clear"), () => QueueAction(ClearList));
                SCEditorUtility.AddGenericMenuItem(gm, false, true, new GUIContent("Remove Conflicts"), () => QueueAction(RemoveConflicts));
                SCEditorUtility.AddGenericMenuItem(gm, false, _keyGeneratorsWithWindow.Count > 0, new GUIContent("Bulk Edit..."), () => OpenKeysGeneratorSelectorWindow(screenRect));
                if (_keyGeneratorsWithoutWindow.Count > 0)
                {
                    gm.AddSeparator(string.Empty);
                    foreach (var generatorData in _keyGeneratorsWithoutWindow)
                    {
                        SCEditorUtility.AddGenericMenuItem(gm, false, true, new GUIContent(generatorData.Name), OnPopulatorDataSelected, generatorData);
                    }
                }
                gm.AddSeparator(string.Empty);
                SCEditorUtility.AddGenericMenuItem(gm, _propertyData.AlwaysShowSearch, true, new GUIContent("Always Show Search"), ToggleAlwaysShowSearchPropertyData);
                gm.AddItem(new GUIContent("Preferences..."), false, () => SettingsService.OpenUserPreferences(EditorUserSettingsProvider.PreferencesPath));
                gm.DropDown(rect);
            }
        }

        private void OnPopulatorDataSelected(object userData)
        {
            var data = (KeyListGeneratorData)userData;
            var so = (KeyListGenerator)ScriptableObject.CreateInstance(data.GeneratorType);
            so.hideFlags = HideFlags.DontSave;
            ApplyPopulatorQueued(so, ModificationType.Add);
        }

        private void OpenKeysGeneratorSelectorWindow(Rect rect)
        {
            var window = ScriptableObject.CreateInstance<KeyListGeneratorSelectorWindow>();
            window.Initialize(_keyGeneratorsWithWindow, _keyFieldInfo.FieldType);
            window.ShowAsDropDown(rect, new Vector2(400, 200));
            window.OnApply += ApplyPopulatorQueued;
        }

        private void ToggleAlwaysShowSearchPropertyData()
        {
            _propertyData.AlwaysShowSearch = !_propertyData.AlwaysShowSearch;
            SavePropertyData();
        }

        private void DoKeyValueRect(Rect rect)
        {
            float width = EditorGUIUtility.labelWidth + 22;
            Rect leftRect = rect.WithWidth(width);
            Rect rightRect = leftRect.AppendRight(rect.width - width);

            if (Event.current.type == EventType.Repaint && _propertyData != null)
            {
                _keyValueStyle.Draw(leftRect, EditorGUIUtility.TrTextContent(_propertyData.GetElementData(SerializedDictionaryDrawer.KeyFlag).Settings.DisplayName), false, false, false, false);
                _keyValueStyle.Draw(rightRect, EditorGUIUtility.TrTextContent(_propertyData.GetElementData(SerializedDictionaryDrawer.ValueFlag).Settings.DisplayName), false, false, false, false);
            }

            if (ListProperty.minArraySize > 0)
            {
                DoDisplayTypeToggle(leftRect, SerializedDictionaryDrawer.KeyFlag);
                DoDisplayTypeToggle(rightRect, SerializedDictionaryDrawer.ValueFlag);
            }

            EditorGUI.DrawRect(rect.AppendDown(1, -1), SerializedDictionaryDrawer.BorderColor);
        }

        private void DoSearch(Rect rect)
        {
            EditorGUI.DrawRect(rect.AppendLeft(1), SerializedDictionaryDrawer.BorderColor);
            EditorGUI.DrawRect(rect.AppendRight(1, -1), SerializedDictionaryDrawer.BorderColor);
            EditorGUI.DrawRect(rect.AppendDown(1, -1), SerializedDictionaryDrawer.BorderColor);

            SearchText = _searchField.OnToolbarGUI(rect.CutTop(2).CutHorizontal(6), SearchText);
        }

        private void ApplyPopulatorQueued(KeyListGenerator populator, ModificationType modificationType)
        {
            var array = populator.GetKeys(_keyFieldInfo.FieldType).OfType<object>().ToArray();
            QueueAction(() => ApplyPopulator(array, modificationType));
        }

        private void QueueAction(EditorApplication.CallbackFunction action)
        {
            EditorApplication.delayCall += action;
        }

        private void ApplyPopulator(IEnumerable<object> elements, ModificationType modificationType)
        {
            foreach (var targetObject in ListProperty.serializedObject.targetObjects)
            {
                Undo.RecordObject(targetObject, "Populate");
                var dictionary = SCEditorUtility.GetPropertyValue(ListProperty, targetObject);
                var lookupTable = GetLookupTable(dictionary);

                if (modificationType == ModificationType.Add)
                    AddElements(lookupTable, elements);
                else if (modificationType == ModificationType.Remove)
                    RemoveElements(lookupTable, elements);
                else if (modificationType == ModificationType.Confine)
                    ConfineElements(lookupTable, elements);

                lookupTable.RecalculateOccurences();
                PrefabUtility.RecordPrefabInstancePropertyModifications(targetObject);
            }

            ListProperty.serializedObject.Update();
            ActiveEditorTracker.sharedTracker.ForceRebuild();
        }

        private static void AddElements(IKeyable lookupTable, IEnumerable<object> elements)
        {
            foreach (var key in elements)
            {
                var occurences = lookupTable.GetOccurences(key);
                if (occurences.Count > 0)
                    continue;
                lookupTable.AddKey(key);
            }
        }

        private static void ConfineElements(IKeyable lookupTable, IEnumerable<object> elements)
        {
            var keysToRemove = lookupTable.Keys.OfType<object>().ToHashSet();
            foreach (var key in elements)
                keysToRemove.Remove(key);

            RemoveElements(lookupTable, keysToRemove);
        }

        private static void RemoveElements(IKeyable lookupTable, IEnumerable<object> elements)
        {
            var indicesToRemove = elements.SelectMany(x => lookupTable.GetOccurences(x)).OrderByDescending(index => index);
            foreach (var index in indicesToRemove)
            {
                lookupTable.RemoveAt(index);
            }
        }

        private void ClearList()
        {
            ListProperty.ClearArray();
            ListProperty.serializedObject.ApplyModifiedProperties();
        }

        private void RemoveConflicts()
        {
            foreach (var targetObject in ListProperty.serializedObject.targetObjects)
            {
                Undo.RecordObject(targetObject, "Remove Conflicts");
                var dictionary = SCEditorUtility.GetPropertyValue(ListProperty, targetObject);
                var lookupTable = GetLookupTable(dictionary);

                List<int> duplicateIndices = new List<int>();

                foreach (var key in lookupTable.Keys)
                {
                    var occurences = lookupTable.GetOccurences(key);
                    for (int i = 1; i < occurences.Count; i++)
                        duplicateIndices.Add(occurences[i]);
                }

                foreach (var indexToRemove in duplicateIndices.OrderByDescending(x => x))
                {
                    lookupTable.RemoveAt(indexToRemove);
                }

                lookupTable.RecalculateOccurences();
                PrefabUtility.RecordPrefabInstancePropertyModifications(targetObject);
            }

            ListProperty.serializedObject.Update();
            ActiveEditorTracker.sharedTracker.ForceRebuild();
        }

        private void DoDisplayTypeToggle(Rect contentRect, bool fieldFlag)
        {
            var displayData = _propertyData.GetElementData(fieldFlag);

            if (displayData.Settings.HasListDrawerToggle)
            {
                Rect rightRectToggle = new Rect(contentRect);
                rightRectToggle.x += rightRectToggle.width - 18;
                rightRectToggle.width = 18;
                EditorGUI.BeginChangeCheck();
                bool newValue = GUI.Toggle(rightRectToggle, displayData.IsListToggleActive, SerializedDictionaryDrawer.DisplayTypeToggleContent, EditorStyles.toolbarButton);
                if (EditorGUI.EndChangeCheck())
                {
                    displayData.IsListToggleActive = newValue;
                    SavePropertyData();
                }
            }
        }

        private float OnGetElementHeight(int index)
        {
            int actualIndex = _pagedIndices[index];
            var element = _activeState.GetPropertyAtIndex(actualIndex);
            return CalculateHeightOfElement(element, _propertyData.GetElementData(SerializedDictionaryDrawer.KeyFlag).EffectiveDisplayType == DisplayType.List ? true : false, _propertyData.GetElementData(SerializedDictionaryDrawer.ValueFlag).EffectiveDisplayType == DisplayType.List ? true : false);
        }

        private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            const int lineLeftSpace = 2;
            const int lineWidth = 1;
            const int lineRightSpace = 12;
            const int totalSpace = lineLeftSpace + lineWidth + lineRightSpace;

            int actualIndex = _pagedIndices[index];

            SerializedProperty kvp = _activeState.GetPropertyAtIndex(actualIndex);
            Rect keyRect = rect.WithSize(EditorGUIUtility.labelWidth - lineLeftSpace, EditorGUIUtility.singleLineHeight);
            Rect lineRect = keyRect.WithXAndWidth(keyRect.x + keyRect.width + lineLeftSpace, lineWidth).WithHeight(rect.height);
            Rect valueRect = keyRect.AppendRight(rect.width - keyRect.width - totalSpace, totalSpace);

            var keyProperty = kvp.FindPropertyRelative(SerializedDictionaryDrawer.KeyName);
            var valueProperty = kvp.FindPropertyRelative(SerializedDictionaryDrawer.ValueName);

            Color prevColor = GUI.color;
            if (_singleEditingData.IsValid)
            {
                var keyObject = _keyFieldInfo.GetValue(_singleEditingData.LookupTable.GetKeyAt(actualIndex));
                var occurences = _singleEditingData.LookupTable.GetOccurences(keyObject);
                if (occurences.Count > 1)
                {
                    GUI.color = occurences[0] == actualIndex ? Color.yellow : Color.red;
                }
                if (!SerializedCollectionsUtility.IsValidKey(keyObject))
                {
                    GUI.color = Color.red;
                }
            }

            var keyDisplayData = _propertyData.GetElementData(SerializedDictionaryDrawer.KeyFlag);
            DrawGroupedElement(keyRect, 20, keyProperty, keyDisplayData.EffectiveDisplayType);

            EditorGUI.DrawRect(lineRect, new Color(36 / 255f, 36 / 255f, 36 / 255f));
            GUI.color = prevColor;

            var valueDisplayData = _propertyData.GetElementData(SerializedDictionaryDrawer.ValueFlag);
            DrawGroupedElement(valueRect, lineRightSpace, valueProperty, valueDisplayData.EffectiveDisplayType);
        }

        private void DrawGroupedElement(Rect rect, int spaceForProperty, SerializedProperty property, DisplayType displayType)
        {
            using (new LabelWidth(rect.width * 0.4f))
            {
                float height = SCEditorUtility.CalculateHeight(property.Copy(), displayType);
                Rect groupRect = rect.CutLeft(-spaceForProperty).WithHeight(height);
                GUI.BeginGroup(groupRect);

                Rect elementRect = new Rect(spaceForProperty, 0, rect.width, height);
                _activeState.DrawElement(elementRect, property, displayType);

                DrawInvisibleProperty(rect.WithWidth(spaceForProperty), property);

                GUI.EndGroup();
            }
        }

        internal static void DrawInvisibleProperty(Rect rect, SerializedProperty property)
        {
            const int propertyOffset = 5;

            GUI.BeginClip(rect.CutLeft(-propertyOffset));
            EditorGUI.BeginProperty(rect, GUIContent.none, property);
            EditorGUI.EndProperty();
            GUI.EndClip();
        }

        internal static void DrawElement(Rect rect, SerializedProperty property, DisplayType displayType, Action<SerializedProperty> BeforeDrawingCallback = null, Action<SerializedProperty> AfterDrawingCallback = null)
        {
            switch (displayType)
            {
                case DisplayType.Property:
                    BeforeDrawingCallback?.Invoke(property);
                    EditorGUI.PropertyField(rect, property, true);
                    AfterDrawingCallback?.Invoke(property);
                    break;
                case DisplayType.PropertyNoLabel:
                    BeforeDrawingCallback?.Invoke(property);
                    EditorGUI.PropertyField(rect, property, GUIContent.none, true);
                    AfterDrawingCallback?.Invoke(property);
                    break;
                case DisplayType.List:
                    Rect childRect = rect.WithHeight(0);
                    foreach (SerializedProperty prop in SCEditorUtility.GetChildren(property.Copy()))
                    {
                        childRect = childRect.AppendDown(EditorGUI.GetPropertyHeight(prop, true));
                        BeforeDrawingCallback?.Invoke(prop);
                        EditorGUI.PropertyField(childRect, prop, true);
                        AfterDrawingCallback?.Invoke(prop);
                    }
                    break;
                default:
                    break;
            }
        }

        private void OnAdd(ReorderableList list)
        {
            int targetIndex = list.selectedIndices.Count > 0 && list.selectedIndices[0] >= 0 ? list.selectedIndices[0] : 0;
            int actualTargetIndex = targetIndex < _pagedIndices.Count ? _pagedIndices[targetIndex] : 0;
            _activeState.InserElementAt(actualTargetIndex);
        }

        private void OnReorder(ReorderableList list, int oldIndex, int newIndex)
        {
            UpdatePaging();
            ListProperty.MoveArrayElement(_pagedIndices[oldIndex], _pagedIndices[newIndex]);
        }

        private void OnRemove(ReorderableList list)
        {
            _activeState.RemoveElementAt(_pagedIndices[list.index]);
            UpdatePaging();
            //int actualIndex = _pagedIndices[list.index];
            //ListProperty.DeleteArrayElementAtIndex(actualIndex);
            //UpdatePaging();
            //if (actualIndex >= ListProperty.minArraySize)
            //    list.index = _pagedIndices.Count - 1;
        }
    }
}