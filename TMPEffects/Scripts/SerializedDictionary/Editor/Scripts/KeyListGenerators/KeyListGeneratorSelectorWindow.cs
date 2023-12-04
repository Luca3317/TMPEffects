using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AYellowpaper.SerializedCollections.KeysGenerators
{
    public class KeyListGeneratorSelectorWindow : EditorWindow
    {
        [SerializeField]
        private int _selectedIndex;
        [SerializeField]
        private ModificationType _modificationType;

        private KeyListGenerator _generator;
        private UnityEditor.Editor _editor;
        private List<KeyListGeneratorData> _generatorsData;
        private Type _targetType;
        private int _undoStart;
        private Dictionary<Type, KeyListGenerator> _keysGenerators = new Dictionary<Type, KeyListGenerator>();
        private string _detailsText;

        public event Action<KeyListGenerator, ModificationType> OnApply;

        private void OnEnable()
        {
            VisualTreeAsset document = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Plugins/SerializedCollections/Editor/Assets/KeysGeneratorSelectorWindow.uxml");
            var element = document.CloneTree();
            element.style.height = new StyleLength(new Length(100, LengthUnit.Percent));
            rootVisualElement.Add(element);
        }

        public void Initialize(IEnumerable<KeyListGeneratorData> generatorsData, Type type)
        {
            _targetType = type;
            _selectedIndex = 0;
            _modificationType = ModificationType.Add;
            _undoStart = Undo.GetCurrentGroup();
            _generatorsData = new List<KeyListGeneratorData>(generatorsData);
            SetGeneratorIndex(0);
            Undo.undoRedoPerformed += HandleUndoCallback;

            rootVisualElement.Q<Button>(className: "sc-close-button").clicked += Close;

            rootVisualElement.Q<RadioButton>(name = "add-modification").userData = ModificationType.Add;
            rootVisualElement.Q<RadioButton>(name = "remove-modification").userData = ModificationType.Remove;
            rootVisualElement.Q<RadioButton>(name = "confine-modification").userData = ModificationType.Confine;

            var modificationToggles = rootVisualElement.Query<RadioButton>(className: "sc-modification-toggle");
            modificationToggles.ForEach(InitializeModificationToggle);

            rootVisualElement.Q<IMGUIContainer>(name = "imgui-inspector").onGUIHandler = EditorGUIHandler;
            rootVisualElement.Q<Button>(name = "apply-button").clicked += ApplyButtonClicked;

            var generatorsContent = rootVisualElement.Q<ScrollView>(name = "generators-content");
            var radioButtonGroup = new RadioButtonGroup();
            radioButtonGroup.name = "generators-group";
            radioButtonGroup.AddToClassList("sc-radio-button-group");
            generatorsContent.Add(radioButtonGroup);

            for (int i = 0; i < _generatorsData.Count; i++)
            {
                var generatorData = _generatorsData[i];

                var radioButton = new RadioButton(generatorData.Name);
                radioButton.value = i == 0;
                radioButton.AddToClassList("sc-text-toggle");
                radioButton.AddToClassList("sc-generator-toggle");
                radioButton.userData = i;
                radioButton.RegisterValueChangedCallback(OnGeneratorClicked);
                radioButtonGroup.Add(radioButton);
            }
        }

        private void ApplyButtonClicked()
        {
            OnApply?.Invoke(_editor.target as KeyListGenerator, _modificationType);
            OnApply = null;
            Close();
        }

        private void EditorGUIHandler()
        {
            EditorGUI.BeginChangeCheck();
            _editor.OnInspectorGUI();
            if (EditorGUI.EndChangeCheck())
            {
                UpdateDetailsText();
            }
        }

        private void InitializeModificationToggle(RadioButton obj)
        {
            if ((ModificationType)obj.userData == _modificationType)
                obj.value = true;
            obj.RegisterValueChangedCallback(OnModificationToggleClicked);
        }

        private void OnModificationToggleClicked(ChangeEvent<bool> evt)
        {
            if (!evt.newValue)
                return;

            var modificationType = (ModificationType)((VisualElement)evt.target).userData;
            _modificationType = modificationType;
        }

        private void UpdateDetailsText()
        {
            var enumerable = _generator.GetKeys(_targetType);
            int count = 0;
            var enumerator = enumerable.GetEnumerator();
            while (enumerator.MoveNext())
            {
                count++;
                if (count > 100)
                {
                    _detailsText = "over 100 Elements";
                    return;
                }
            }
            _detailsText = $"{count} Elements";

            rootVisualElement.Q<Label>(name = "generated-count-label").text = _detailsText;
        }

        private void OnDestroy()
        {
            Undo.undoRedoPerformed -= HandleUndoCallback;
            Undo.RevertAllDownToGroup(_undoStart);
            foreach (var keyGenerator in _keysGenerators)
                DestroyImmediate(keyGenerator.Value);
        }

        private void OnGeneratorClicked(ChangeEvent<bool> evt)
        {
            if (!evt.newValue)
                return;

            SetGeneratorIndex((int)(evt.target as VisualElement).userData);
        }

        private void HandleUndoCallback()
        {
            UpdateGeneratorAndEditorIfNeeded();
            Repaint();
        }

        private void SetGeneratorIndex(int index)
        {
            Undo.RecordObject(this, "Change Window");
            _selectedIndex = index;
            UpdateGeneratorAndEditorIfNeeded();
        }

        private void UpdateGeneratorAndEditorIfNeeded()
        {
            var targetType = _generatorsData[_selectedIndex].GeneratorType;
            if (_generator != null && _generator.GetType() == targetType)
                return;

            _generator = GetOrCreateKeysGenerator(targetType);
            if (_editor != null)
                DestroyImmediate(_editor);
            _editor = UnityEditor.Editor.CreateEditor(_generator);

            UpdateDetailsText();
        }

        private KeyListGenerator GetOrCreateKeysGenerator(Type type)
        {
            if (!_keysGenerators.ContainsKey(type))
            {
                var so = (KeyListGenerator)CreateInstance(type);
                so.hideFlags = HideFlags.DontSave;
                _keysGenerators.Add(type, so);
            }
            return _keysGenerators[type];
        }
    }
}