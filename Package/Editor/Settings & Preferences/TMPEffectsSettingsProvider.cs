using System.Collections.Generic;
using System.Linq;
using TMPEffects.Extensions;
using TMPEffects.Parameters;
using TMPEffects.TMPAnimations;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using AnimationUtility = TMPEffects.TMPAnimations.AnimationUtility;

namespace TMPEffects.Editor
{
    internal class TMPEffectsSettingsProvider : SettingsProvider
    {
        public const string SettingsPath = "Project/TMPEffects";

        private SerializedObject serializedObject;
        private SerializedProperty globalKeywordDatabaseProp;
        private SerializedProperty animationTagPrefixProp;
        private SerializedProperty showAnimationTagPrefixProp;
        private SerializedProperty hideAnimationTagPrefixProp;
        private SerializedProperty commandTagPrefixProp;
        private SerializedProperty eventTagPrefixProp;

        class Styles
        {
        }

        public static IEnumerable<SerializedProperty> GetChildren(SerializedProperty property)
        {
            property = property.Copy();
            var nextElement = property.Copy();
            bool hasNextElement = nextElement.NextVisible(false);
            if (!hasNextElement)
            {
                nextElement = null;
            }

            property.NextVisible(true);
            while (true)
            {
                if ((SerializedProperty.EqualContents(property, nextElement)))
                {
                    yield break;
                }

                yield return property;

                bool hasNext = property.NextVisible(false);
                if (!hasNext)
                {
                    break;
                }
            }
        }


        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            var provider = new TMPEffectsSettingsProvider(SettingsPath, SettingsScope.Project);
            provider.keywords = GetSearchKeywordsFromGUIContentProperties<Styles>();
            return provider;
        }

        public TMPEffectsSettingsProvider(string path, SettingsScope scope = SettingsScope.Project) : base(path, scope)
        {
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            EnsureSerializedObjectExists();
        }

        private bool EnsureSerializedObjectExists()
        {
            var settings = TMPEffectsSettings.Get();

            if (settings == null)
            {
                return false;
            }

            if (serializedObject == null)
            {
                serializedObject = new SerializedObject(settings);
                globalKeywordDatabaseProp = serializedObject.FindProperty("globalKeywordDatabase");
                animationTagPrefixProp = serializedObject.FindProperty("animationTagPrefix");
                showAnimationTagPrefixProp = serializedObject.FindProperty("showAnimationTagPrefix");
                hideAnimationTagPrefixProp = serializedObject.FindProperty("hideAnimationTagPrefix");
                commandTagPrefixProp = serializedObject.FindProperty("commandTagPrefix");
                eventTagPrefixProp = serializedObject.FindProperty("eventTagPrefix");
            }

            return true;
        }


#if UNITY_2022_2_OR_NEWER
        private void Reset<T>(SerializedProperty keywordsProp, Dictionary<string, T> builtin)
        {
            if (builtin == null) return;

            Dictionary<string, List<T>> vals = new Dictionary<string, List<T>>();

            for (int i = 0; i < keywordsProp.arraySize; i++)
            {
                var elem = keywordsProp.GetArrayElementAtIndex(i);
                var key = elem.FindPropertyRelative("Key").stringValue;
                var value = (T)elem.FindPropertyRelative("Value").boxedValue;

                if (vals.ContainsKey(key))
                {
                    vals[key].Add(value);
                }
                else
                {
                    vals.Add(key, new List<T> { value });
                }
            }

            foreach (var pair in builtin)
            {
                var key = pair.Key;
                var value = pair.Value;

                // If is already present with same value
                if (vals.TryGetValue(key, out var currentValue))
                    if (currentValue.Any(val => val.Equals(value)))
                        continue;

                keywordsProp.arraySize++;
                var elem = keywordsProp.GetArrayElementAtIndex(keywordsProp.arraySize - 1);
                elem.FindPropertyRelative("Key").stringValue = key;
                elem.FindPropertyRelative("Value").boxedValue = value;
            }
        }
#else
        private void ResetColors(SerializedProperty colorkeywordsProp)
        {
            Dictionary<string, List<Color>> vals = new Dictionary<string, List<Color>>();

            for (int i = 0; i < colorkeywordsProp.arraySize; i++)
            {
                var elem = colorkeywordsProp.GetArrayElementAtIndex(i);
                var key = elem.FindPropertyRelative("Key").stringValue;
                var value = elem.FindPropertyRelative("Value").colorValue;

                if (vals.ContainsKey(key))
                {
                    vals[key].Add(value);
                }
                else
                {
                    vals.Add(key, new List<Color> { value });
                }
            }

            foreach (var pair in ColorKeywords)
            {
                var key = pair.Key;
                var value = pair.Value;

                // If is already present with same value
                if (vals.TryGetValue(key, out var currentValue))
                    if (currentValue.Any(val => val.Equals(value)))
                        continue;

                colorkeywordsProp.arraySize++;
                var elem = colorkeywordsProp.GetArrayElementAtIndex(colorkeywordsProp.arraySize - 1);
                elem.FindPropertyRelative("Key").stringValue = key;
                elem.FindPropertyRelative("Value").colorValue = value;
            }
        }

        private void ResetFloats(SerializedProperty floatkeywordsProp)
        {
            Dictionary<string, List<float>> vals = new Dictionary<string, List<float>>();

            for (int i = 0; i < floatkeywordsProp.arraySize; i++)
            {
                var elem = floatkeywordsProp.GetArrayElementAtIndex(i);
                var key = elem.FindPropertyRelative("Key").stringValue;
                var value = elem.FindPropertyRelative("Value").floatValue;

                if (vals.ContainsKey(key))
                {
                    vals[key].Add(value);
                }
                else
                {
                    vals.Add(key, new List<float> { value });
                }
            }

            foreach (var pair in FloatKeywords)
            {
                var key = pair.Key;
                var value = pair.Value;

                // If is already present with same value
                if (vals.TryGetValue(key, out var currentValue))
                    if (currentValue.Any(val => val.Equals(value)))
                        continue;

                floatkeywordsProp.arraySize++;
                var elem = floatkeywordsProp.GetArrayElementAtIndex(floatkeywordsProp.arraySize - 1);
                elem.FindPropertyRelative("Key").stringValue = key;
                elem.FindPropertyRelative("Value").floatValue = value;
            }
        }

        private void ResetVector3s(SerializedProperty animCurveKeywordsProp)
        {
            Dictionary<string, List<Vector3>> vals = new Dictionary<string, List<Vector3>>();

            for (int i = 0; i < animCurveKeywordsProp.arraySize; i++)
            {
                var elem = animCurveKeywordsProp.GetArrayElementAtIndex(i);
                var key = elem.FindPropertyRelative("Key").stringValue;
                var value = elem.FindPropertyRelative("Value").vector3Value;

                if (vals.ContainsKey(key))
                {
                    vals[key].Add(value);
                }
                else
                {
                    vals.Add(key, new List<Vector3> { value });
                }
            }

            foreach (var pair in Vector3Keywords)
            {
                var key = pair.Key;
                var value = pair.Value;

                // If is already present with same value
                if (vals.TryGetValue(key, out var currentValue))
                    if (currentValue.Any(val => val.Equals(value)))
                        continue;

                animCurveKeywordsProp.arraySize++;
                var elem = animCurveKeywordsProp.GetArrayElementAtIndex(animCurveKeywordsProp.arraySize - 1);
                elem.FindPropertyRelative("Key").stringValue = key;
                elem.FindPropertyRelative("Value").vector3Value = value;
            }
        }

        private void ResetAnchors(SerializedProperty anchorKeywordsProp)
        {
            Dictionary<string, List<Vector2>> vals = new Dictionary<string, List<Vector2>>();

            for (int i = 0; i < anchorKeywordsProp.arraySize; i++)
            {
                var elem = anchorKeywordsProp.GetArrayElementAtIndex(i);
                var key = elem.FindPropertyRelative("Key").stringValue;
                var value = elem.FindPropertyRelative("Value").vector2Value;

                if (vals.ContainsKey(key))
                {
                    vals[key].Add(value);
                }
                else
                {
                    vals.Add(key, new List<Vector2> { value });
                }
            }

            foreach (var pair in AnchorKeywords)
            {
                var key = pair.Key;
                var value = pair.Value;

                // If is already present with same value
                if (vals.TryGetValue(key, out var currentValue))
                    if (currentValue.Any(val => val.Equals(value)))
                        continue;

                anchorKeywordsProp.arraySize++;
                var elem = anchorKeywordsProp.GetArrayElementAtIndex(anchorKeywordsProp.arraySize - 1);
                elem.FindPropertyRelative("Key").stringValue = key;
                elem.FindPropertyRelative("Value").vector2Value = value;
            }
        }

        private void ResetAnimationCurves(SerializedProperty animCurveKeywordsProp,
            IDictionary<string, AnimationCurve> curves)
        {
            Dictionary<string, List<AnimationCurve>> vals = new Dictionary<string, List<AnimationCurve>>();

            for (int i = 0; i < animCurveKeywordsProp.arraySize; i++)
            {
                var elem = animCurveKeywordsProp.GetArrayElementAtIndex(i);
                var key = elem.FindPropertyRelative("Key").stringValue;
                var value = elem.FindPropertyRelative("Value").animationCurveValue;

                if (vals.ContainsKey(key))
                {
                    vals[key].Add(value);
                }
                else
                {
                    vals.Add(key, new List<AnimationCurve> { value });
                }
            }

            foreach (var pair in curves)
            {
                var key = pair.Key;
                var value = pair.Value;

                // If is already present with same value
                if (vals.TryGetValue(key, out var currentValue))
                    if (currentValue.Any(val => val.Equals(value)))
                        continue;

                animCurveKeywordsProp.arraySize++;
                var elem = animCurveKeywordsProp.GetArrayElementAtIndex(animCurveKeywordsProp.arraySize - 1);
                elem.FindPropertyRelative("Key").stringValue = key;
                elem.FindPropertyRelative("Value").animationCurveValue = value;
            }
        }

        private void ResetOffsetTypes(SerializedProperty offsetKeywordsProp)
        {
            Dictionary<string, List<OffsetTypePowerEnum>> vals = new Dictionary<string, List<OffsetTypePowerEnum>>();

            for (int i = 0; i < offsetKeywordsProp.arraySize; i++)
            {
                var elem = offsetKeywordsProp.GetArrayElementAtIndex(i);
                var key = elem.FindPropertyRelative("Key").stringValue;
                var value = elem.FindPropertyRelative("Value");

                var enumValue = value.FindPropertyRelative("enumValue").enumValueIndex;
                var customValue = value.FindPropertyRelative("customValue").objectReferenceValue;
                var useCustomValue = value.FindPropertyRelative("useCustom").boolValue;
                var offsettype = new OffsetTypePowerEnum((ParameterTypes.WaveOffsetType)enumValue,
                    (ParameterTypes.TMPOffsetProvider)customValue, useCustomValue);

                if (vals.ContainsKey(key))
                {
                    vals[key].Add(offsettype);
                }
                else
                {
                    vals.Add(key, new List<OffsetTypePowerEnum> { offsettype });
                }
            }

            foreach (var pair in OffsetKeywords)
            {
                var key = pair.Key;
                var value = pair.Value;

                // If is already present with same value
                if (vals.TryGetValue(key, out var currentValue))
                    if (currentValue.Any(val => val.Equals(value)))
                        continue;

                offsetKeywordsProp.arraySize++;
                var elem = offsetKeywordsProp.GetArrayElementAtIndex(offsetKeywordsProp.arraySize - 1);
                elem.FindPropertyRelative("Key").stringValue = key;

                var offsetvalue = elem.FindPropertyRelative("Value");
                offsetvalue.FindPropertyRelative("useCustom").boolValue = value.UseCustom;
                offsetvalue.FindPropertyRelative("customValue").objectReferenceValue = value.Value;
                offsetvalue.FindPropertyRelative("enumValue").enumValueFlag = (int)value.EnumValue;
            }
        }
#endif


        public override void OnGUI(string searchContext)
        {
            if (!EnsureSerializedObjectExists()) return;

            EditorGUI.indentLevel = 1;

            serializedObject.UpdateIfRequiredOrScript();

            EditorGUIUtility.labelWidth += 50;
            EditorGUILayout.LabelField("Keywords", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(globalKeywordDatabaseProp);
            if (globalKeywordDatabaseProp.objectReferenceValue != null)
            {
                if (GUILayout.Button("Reset Global KeywordDatabase"))
                {
                    var assetObject = new SerializedObject(globalKeywordDatabaseProp.objectReferenceValue);
                    var dict = AnimationCurveUtility.NameConstructorMapping.ToDictionary(t => t.Key,
                        t => t.Value.Invoke());

#if UNITY_2022_2_OR_NEWER
                    Reset(assetObject.FindProperty("animationCurveKeywords").FindPropertyRelative("_serializedList"),
                        dict);
                    Reset(assetObject.FindProperty("colorKeywords").FindPropertyRelative("_serializedList"),
                        ColorKeywords);
                    Reset(assetObject.FindProperty("floatKeywords").FindPropertyRelative("_serializedList"),
                        FloatKeywords);
                    Reset(assetObject.FindProperty("offsetTypeKeywords").FindPropertyRelative("_serializedList"),
                        OffsetKeywords);
                    Reset(assetObject.FindProperty("anchorKeywords").FindPropertyRelative("_serializedList"),
                        AnchorKeywords);
                    Reset(assetObject.FindProperty("vector3Keywords").FindPropertyRelative("_serializedList"),
                        Vector3Keywords);
#else
                // TODO Test these in a version lower than 2022_2
                ResetColors(assetObject.FindProperty("colorKeywords").FindPropertyRelative("_serializedList"));
                ResetFloats(assetObject.FindProperty("floatKeywords").FindPropertyRelative("_serializedList"));
                ResetOffsetTypes(assetObject.FindProperty("offsetTypeKeywords").FindPropertyRelative("_serializedList"));
                ResetAnimationCurves(assetObject.FindProperty("animationCurveKeywords").FindPropertyRelative("_serializedList"), dict);
                ResetVector3s(assetObject.FindProperty("vector3Keywords").FindPropertyRelative("_serializedList"));
                ResetAnchors(assetObject.FindProperty("anchorKeywords").FindPropertyRelative("_serializedList"));
#endif

                    if (assetObject.hasModifiedProperties) assetObject.ApplyModifiedProperties();
                }
            }


            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Tags", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(animationTagPrefixProp);
            EditorGUILayout.PropertyField(showAnimationTagPrefixProp);
            EditorGUILayout.PropertyField(hideAnimationTagPrefixProp);
            EditorGUILayout.PropertyField(commandTagPrefixProp);
            EditorGUILayout.PropertyField(eventTagPrefixProp);
            EditorGUIUtility.labelWidth -= 50;

            bool changed = serializedObject.ApplyModifiedProperties();
            if (changed)
            {
                AssetDatabase.SaveAssetIfDirty(serializedObject.targetObject);
            }
        }

        public static readonly Dictionary<string, float> FloatKeywords = new Dictionary<string, float>()
        {
            { "e", (float)System.Math.E },
            { "pi", (float)Mathf.PI },
            { "epsilon", Mathf.Epsilon },
            { "phi", 1.61803f }
        };

        public static readonly Dictionary<string, int> IntKeywords = new Dictionary<string, int>()
        {
        };

        public static readonly Dictionary<string, bool> BoolKeywords = new Dictionary<string, bool>()
        {
        };

        // TODO Storing these here until implemented reset
        private Dictionary<string, Color> ColorKeywords = new Dictionary<string, Color>()
        {
            { "black", Color.black },
            { "blue", Color.blue },
            { "clear", Color.clear },
            { "green", Color.green },
            { "cyan", Color.cyan },
            { "gray", Color.gray },
            { "grey", Color.grey },
            { "magenta", Color.magenta },
            { "red", Color.red },
            { "white", Color.white },
            { "yellow", Color.yellow },

            { "maroon", new Color32(128, 0, 0, 255) },
            { "olive", new Color32(128, 128, 0, 255) },
            { "lime", new Color32(0, 255, 0, 255) },
            { "aqua", new Color32(0, 255, 255, 255) },
            { "teal", new Color32(0, 128, 128, 255) },
            { "navy", new Color32(0, 0, 128, 255) },
            { "fuchsia", new Color32(255, 0, 255, 255) },
            { "purple", new Color32(128, 0, 128, 255) },
            { "silver", new Color32(192, 192, 192, 255) },
            { "orange", new Color32(255, 165, 0, 255) },
            { "pink", new Color32(255, 192, 203, 255) },

            { "gold", new Color32(255, 215, 0, 255) },
            { "indigo", new Color32(75, 0, 130, 255) },
            { "violet", new Color32(238, 130, 238, 255) },
            { "brown", new Color32(165, 42, 42, 255) },
            { "beige", new Color32(245, 245, 220, 255) },
            { "ivory", new Color32(255, 255, 240, 255) },
            { "khaki", new Color32(240, 230, 140, 255) },
            { "lavender", new Color32(230, 230, 250, 255) },
            { "salmon", new Color32(250, 128, 114, 255) },
            { "turquoise", new Color32(64, 224, 208, 255) },

            { "coral", new Color32(255, 127, 80, 255) },
            { "peach", new Color32(255, 218, 185, 255) },
            { "mint", new Color32(189, 252, 201, 255) },
            { "skyblue", new Color32(135, 206, 235, 255) },
            { "plum", new Color32(221, 160, 221, 255) },
            { "chocolate", new Color32(210, 105, 30, 255) },
            { "tomato", new Color32(255, 99, 71, 255) },
            { "honeydew", new Color32(240, 255, 240, 255) },
            { "orchid", new Color32(218, 112, 214, 255) },
            { "papayawhip", new Color32(255, 239, 213, 255) },

            { "darkblue", new Color32(0, 0, 139, 255) },
            { "lightblue", new Color32(173, 216, 230, 255) },
            { "darkred", new Color32(139, 0, 0, 255) },
            { "lightred", new Color32(255, 102, 102, 255) },
            { "darkgreen", new Color32(0, 100, 0, 255) },
            { "lightgreen", new Color32(144, 238, 144, 255) },
            { "darkyellow", new Color32(204, 204, 0, 255) },
            { "lightyellow", new Color32(255, 255, 224, 255) },
            { "darkorange", new Color32(255, 140, 0, 255) },
            { "lightorange", new Color32(255, 165, 0, 255) },
            { "darkviolet", new Color32(148, 0, 211, 255) },
            { "lightviolet", new Color32(238, 130, 238, 255) },
            { "darkbrown", new Color32(101, 67, 33, 255) },
            { "lightbrown", new Color32(181, 101, 29, 255) },
            { "darkpurple", new Color32(128, 0, 128, 255) },
            { "lightpurple", new Color32(147, 112, 219, 255) },
            { "darkmagenta", new Color32(139, 0, 139, 255) },
            { "lightmagenta", new Color32(255, 0, 255, 255) },
        };


        public static readonly Dictionary<string, Vector2> AnchorKeywords = new Dictionary<string, Vector2>()
        {
            { "a:top", Vector2.up },
            { "a:bottom", Vector2.down },
            { "a:bttm", Vector2.down },
            { "a:right", Vector2.right },
            { "a:left", Vector2.left },

            { "a:topright", Vector2.up + Vector2.right },
            { "a:tr", Vector2.up + Vector2.right },

            { "a:bottomright", Vector2.down + Vector2.right },
            { "a:bttmright", Vector2.down + Vector2.right },
            { "a:br", Vector2.down + Vector2.right },

            { "a:topleft", Vector2.up + Vector2.left },
            { "a:tl", Vector2.up + Vector2.left },

            { "a:bottomleft", Vector2.down + Vector2.left },
            { "a:bttmleft", Vector2.down + Vector2.left },
            { "a:bl", Vector2.down + Vector2.left },

            { "a:center", Vector2.zero }
        };

        public static readonly Dictionary<string, Vector3> Vector3Keywords = new Dictionary<string, Vector3>()
        {
            { "up", Vector3.up },
            { "down", Vector3.down },
            { "right", Vector3.right },
            { "left", Vector3.left },
            { "forward", Vector3.forward },
            { "fwd", Vector3.forward },
            { "back", Vector3.back },

            { "inf", Vector3.positiveInfinity },
            { "ninf", Vector3.negativeInfinity },
            { "-inf", Vector3.negativeInfinity },
            { "+inf", Vector3.negativeInfinity },
            { "zero", Vector3.zero },

            { "up right", Vector3.up + Vector3.right },
            { "up right forward", Vector3.up + Vector3.right + Vector3.forward },
            { "up right fwd", Vector3.up + Vector3.right + Vector3.forward },
            { "up right back", Vector3.up + Vector3.right + Vector3.back },
            { "up left", Vector3.up + Vector3.left },
            { "up left forward", Vector3.up + Vector3.left + Vector3.forward },
            { "up left fwd", Vector3.up + Vector3.left + Vector3.forward },
            { "up left back", Vector3.up + Vector3.left + Vector3.back },
            { "down right", Vector3.down + Vector3.right },
            { "down right forward", Vector3.down + Vector3.right + Vector3.forward },
            { "down right fwd", Vector3.down + Vector3.right + Vector3.forward },
            { "down right back", Vector3.down + Vector3.right + Vector3.back },
            { "down left", Vector3.down + Vector3.left },
            { "down left forward", Vector3.down + Vector3.left + Vector3.forward },
            { "down left fwd", Vector3.down + Vector3.left + Vector3.forward },
            { "down left back", Vector3.down + Vector3.left + Vector3.back },
            { "up forward", Vector3.up + Vector3.forward },
            { "up forward right", Vector3.up + Vector3.forward + Vector3.right },
            { "up forward left", Vector3.up + Vector3.forward + Vector3.left },
            { "up fwd", Vector3.up + Vector3.forward },
            { "up fwd right", Vector3.up + Vector3.forward + Vector3.right },
            { "up fwd left", Vector3.up + Vector3.forward + Vector3.left },
            { "up back", Vector3.up + Vector3.back },
            { "up back right", Vector3.up + Vector3.back + Vector3.right },
            { "up back left", Vector3.up + Vector3.back + Vector3.left },
            { "down forward", Vector3.down + Vector3.forward },
            { "down forward right", Vector3.down + Vector3.forward + Vector3.right },
            { "down forward left", Vector3.down + Vector3.forward + Vector3.left },
            { "down fwd", Vector3.down + Vector3.forward },
            { "down fwd right", Vector3.down + Vector3.forward + Vector3.right },
            { "down fwd left", Vector3.down + Vector3.forward + Vector3.left },
            { "down back", Vector3.down + Vector3.back },
            { "down back right", Vector3.down + Vector3.back + Vector3.right },
            { "down back left", Vector3.down + Vector3.back + Vector3.left },
            { "right up", Vector3.right + Vector3.up },
            { "right up forward", Vector3.right + Vector3.up + Vector3.forward },
            { "right up fwd", Vector3.right + Vector3.up + Vector3.forward },
            { "right up back", Vector3.right + Vector3.up + Vector3.back },
            { "right down", Vector3.right + Vector3.down },
            { "right down forward", Vector3.right + Vector3.down + Vector3.forward },
            { "right down fwd", Vector3.right + Vector3.down + Vector3.forward },
            { "right down back", Vector3.right + Vector3.down + Vector3.back },
            { "left up", Vector3.left + Vector3.up },
            { "left up forward", Vector3.left + Vector3.up + Vector3.forward },
            { "left up fwd", Vector3.left + Vector3.up + Vector3.forward },
            { "left up back", Vector3.left + Vector3.up + Vector3.back },
            { "left down", Vector3.left + Vector3.down },
            { "left down forward", Vector3.left + Vector3.down + Vector3.forward },
            { "left down fwd", Vector3.left + Vector3.down + Vector3.forward },
            { "left down back", Vector3.left + Vector3.down + Vector3.back },
            { "right forward", Vector3.right + Vector3.forward },
            { "right forward up", Vector3.right + Vector3.forward + Vector3.up },
            { "right forward down", Vector3.right + Vector3.forward + Vector3.down },
            { "right fwd", Vector3.right + Vector3.forward },
            { "right fwd up", Vector3.right + Vector3.forward + Vector3.up },
            { "right fwd down", Vector3.right + Vector3.forward + Vector3.down },
            { "right back", Vector3.right + Vector3.back },
            { "right back up", Vector3.right + Vector3.back + Vector3.up },
            { "right back down", Vector3.right + Vector3.back + Vector3.down },
            { "left forward", Vector3.left + Vector3.forward },
            { "left forward up", Vector3.left + Vector3.forward + Vector3.up },
            { "left forward down", Vector3.left + Vector3.forward + Vector3.down },
            { "left fwd", Vector3.left + Vector3.forward },
            { "left fwd up", Vector3.left + Vector3.forward + Vector3.up },
            { "left fwd down", Vector3.left + Vector3.forward + Vector3.down },
            { "left back", Vector3.left + Vector3.back },
            { "left back up", Vector3.left + Vector3.back + Vector3.up },
            { "left back down", Vector3.left + Vector3.back + Vector3.down },
            { "forward right", Vector3.forward + Vector3.right },
            { "forward right up", Vector3.forward + Vector3.right + Vector3.up },
            { "forward right down", Vector3.forward + Vector3.right + Vector3.down },
            { "forward left", Vector3.forward + Vector3.left },
            { "forward left up", Vector3.forward + Vector3.left + Vector3.up },
            { "forward left down", Vector3.forward + Vector3.left + Vector3.down },
            { "fwd right", Vector3.forward + Vector3.right },
            { "fwd right up", Vector3.forward + Vector3.right + Vector3.up },
            { "fwd right down", Vector3.forward + Vector3.right + Vector3.down },
            { "fwd left", Vector3.forward + Vector3.left },
            { "fwd left up", Vector3.forward + Vector3.left + Vector3.up },
            { "fwd left down", Vector3.forward + Vector3.left + Vector3.down },
            { "back right", Vector3.back + Vector3.right },
            { "back right up", Vector3.back + Vector3.right + Vector3.up },
            { "back right down", Vector3.back + Vector3.right + Vector3.down },
            { "back left", Vector3.back + Vector3.left },
            { "back left up", Vector3.back + Vector3.left + Vector3.up },
            { "back left down", Vector3.back + Vector3.left + Vector3.down },
            { "forward up", Vector3.forward + Vector3.up },
            { "forward up right", Vector3.forward + Vector3.up + Vector3.right },
            { "forward up left", Vector3.forward + Vector3.up + Vector3.left },
            { "forward down", Vector3.forward + Vector3.down },
            { "forward down right", Vector3.forward + Vector3.down + Vector3.right },
            { "forward down left", Vector3.forward + Vector3.down + Vector3.left },
            { "fwd up", Vector3.forward + Vector3.up },
            { "fwd up right", Vector3.forward + Vector3.up + Vector3.right },
            { "fwd up left", Vector3.forward + Vector3.up + Vector3.left },
            { "fwd down", Vector3.forward + Vector3.down },
            { "fwd down right", Vector3.forward + Vector3.down + Vector3.right },
            { "fwd down left", Vector3.forward + Vector3.down + Vector3.left },
            { "back up", Vector3.back + Vector3.up },
            { "back up right", Vector3.back + Vector3.up + Vector3.right },
            { "back up left", Vector3.back + Vector3.up + Vector3.left },
            { "back down", Vector3.back + Vector3.down },
            { "back down right", Vector3.back + Vector3.down + Vector3.right },
            { "back down left", Vector3.back + Vector3.down + Vector3.left },

            { "upright", Vector3.up + Vector3.right },
            { "uprightforward", Vector3.up + Vector3.right + Vector3.forward },
            { "uprightfwd", Vector3.up + Vector3.right + Vector3.forward },
            { "uprightback", Vector3.up + Vector3.right + Vector3.back },
            { "upleft", Vector3.up + Vector3.left },
            { "upleftforward", Vector3.up + Vector3.left + Vector3.forward },
            { "upleftfwd", Vector3.up + Vector3.left + Vector3.forward },
            { "upleftback", Vector3.up + Vector3.left + Vector3.back },
            { "downright", Vector3.down + Vector3.right },
            { "downrightforward", Vector3.down + Vector3.right + Vector3.forward },
            { "downrightfwd", Vector3.down + Vector3.right + Vector3.forward },
            { "downrightback", Vector3.down + Vector3.right + Vector3.back },
            { "downleft", Vector3.down + Vector3.left },
            { "downleftforward", Vector3.down + Vector3.left + Vector3.forward },
            { "downleftfwd", Vector3.down + Vector3.left + Vector3.forward },
            { "downleftback", Vector3.down + Vector3.left + Vector3.back },
            { "upforward", Vector3.up + Vector3.forward },
            { "upforwardright", Vector3.up + Vector3.forward + Vector3.right },
            { "upforwardleft", Vector3.up + Vector3.forward + Vector3.left },
            { "upfwd", Vector3.up + Vector3.forward },
            { "upfwdright", Vector3.up + Vector3.forward + Vector3.right },
            { "upfwdleft", Vector3.up + Vector3.forward + Vector3.left },
            { "upback", Vector3.up + Vector3.back },
            { "upbackright", Vector3.up + Vector3.back + Vector3.right },
            { "upbackleft", Vector3.up + Vector3.back + Vector3.left },
            { "downforward", Vector3.down + Vector3.forward },
            { "downforwardright", Vector3.down + Vector3.forward + Vector3.right },
            { "downforwardleft", Vector3.down + Vector3.forward + Vector3.left },
            { "downfwd", Vector3.down + Vector3.forward },
            { "downfwdright", Vector3.down + Vector3.forward + Vector3.right },
            { "downfwdleft", Vector3.down + Vector3.forward + Vector3.left },
            { "downback", Vector3.down + Vector3.back },
            { "downbackright", Vector3.down + Vector3.back + Vector3.right },
            { "downbackleft", Vector3.down + Vector3.back + Vector3.left },
            { "rightup", Vector3.right + Vector3.up },
            { "rightupforward", Vector3.right + Vector3.up + Vector3.forward },
            { "rightupfwd", Vector3.right + Vector3.up + Vector3.forward },
            { "rightupback", Vector3.right + Vector3.up + Vector3.back },
            { "rightdown", Vector3.right + Vector3.down },
            { "rightdownforward", Vector3.right + Vector3.down + Vector3.forward },
            { "rightdownfwd", Vector3.right + Vector3.down + Vector3.forward },
            { "rightdownback", Vector3.right + Vector3.down + Vector3.back },
            { "leftup", Vector3.left + Vector3.up },
            { "leftupforward", Vector3.left + Vector3.up + Vector3.forward },
            { "leftupfwd", Vector3.left + Vector3.up + Vector3.forward },
            { "leftupback", Vector3.left + Vector3.up + Vector3.back },
            { "leftdown", Vector3.left + Vector3.down },
            { "leftdownforward", Vector3.left + Vector3.down + Vector3.forward },
            { "leftdownfwd", Vector3.left + Vector3.down + Vector3.forward },
            { "leftdownback", Vector3.left + Vector3.down + Vector3.back },
            { "rightforward", Vector3.right + Vector3.forward },
            { "rightforwardup", Vector3.right + Vector3.forward + Vector3.up },
            { "rightforwarddown", Vector3.right + Vector3.forward + Vector3.down },
            { "rightfwd", Vector3.right + Vector3.forward },
            { "rightfwdup", Vector3.right + Vector3.forward + Vector3.up },
            { "rightfwddown", Vector3.right + Vector3.forward + Vector3.down },
            { "rightback", Vector3.right + Vector3.back },
            { "rightbackup", Vector3.right + Vector3.back + Vector3.up },
            { "rightbackdown", Vector3.right + Vector3.back + Vector3.down },
            { "leftforward", Vector3.left + Vector3.forward },
            { "leftforwardup", Vector3.left + Vector3.forward + Vector3.up },
            { "leftforwarddown", Vector3.left + Vector3.forward + Vector3.down },
            { "leftfwd", Vector3.left + Vector3.forward },
            { "leftfwdup", Vector3.left + Vector3.forward + Vector3.up },
            { "leftfwddown", Vector3.left + Vector3.forward + Vector3.down },
            { "leftback", Vector3.left + Vector3.back },
            { "leftbackup", Vector3.left + Vector3.back + Vector3.up },
            { "leftbackdown", Vector3.left + Vector3.back + Vector3.down },
            { "forwardright", Vector3.forward + Vector3.right },
            { "forwardrightup", Vector3.forward + Vector3.right + Vector3.up },
            { "forwardrightdown", Vector3.forward + Vector3.right + Vector3.down },
            { "forwardleft", Vector3.forward + Vector3.left },
            { "forwardleftup", Vector3.forward + Vector3.left + Vector3.up },
            { "forwardleftdown", Vector3.forward + Vector3.left + Vector3.down },
            { "fwdright", Vector3.forward + Vector3.right },
            { "fwdrightup", Vector3.forward + Vector3.right + Vector3.up },
            { "fwdrightdown", Vector3.forward + Vector3.right + Vector3.down },
            { "fwdleft", Vector3.forward + Vector3.left },
            { "fwdleftup", Vector3.forward + Vector3.left + Vector3.up },
            { "fwdleftdown", Vector3.forward + Vector3.left + Vector3.down },
            { "backright", Vector3.back + Vector3.right },
            { "backrightup", Vector3.back + Vector3.right + Vector3.up },
            { "backrightdown", Vector3.back + Vector3.right + Vector3.down },
            { "backleft", Vector3.back + Vector3.left },
            { "backleftup", Vector3.back + Vector3.left + Vector3.up },
            { "backleftdown", Vector3.back + Vector3.left + Vector3.down },
            { "forwardup", Vector3.forward + Vector3.up },
            { "forwardupright", Vector3.forward + Vector3.up + Vector3.right },
            { "forwardupleft", Vector3.forward + Vector3.up + Vector3.left },
            { "forwarddown", Vector3.forward + Vector3.down },
            { "forwarddownright", Vector3.forward + Vector3.down + Vector3.right },
            { "forwarddownleft", Vector3.forward + Vector3.down + Vector3.left },
            { "fwdup", Vector3.forward + Vector3.up },
            { "fwdupright", Vector3.forward + Vector3.up + Vector3.right },
            { "fwdupleft", Vector3.forward + Vector3.up + Vector3.left },
            { "fwddown", Vector3.forward + Vector3.down },
            { "fwddownright", Vector3.forward + Vector3.down + Vector3.right },
            { "fwddownleft", Vector3.forward + Vector3.down + Vector3.left },
            { "backup", Vector3.back + Vector3.up },
            { "backupright", Vector3.back + Vector3.up + Vector3.right },
            { "backupleft", Vector3.back + Vector3.up + Vector3.left },
            { "backdown", Vector3.back + Vector3.down },
            { "backdownright", Vector3.back + Vector3.down + Vector3.right },
            { "backdownleft", Vector3.back + Vector3.down + Vector3.left },
        };

        public static readonly Dictionary<string, OffsetTypePowerEnum> OffsetKeywords =
            new Dictionary<string, OffsetTypePowerEnum>()
            {
                { "sidx", new OffsetTypePowerEnum(ParameterTypes.OffsetType.SegmentIndex) },
                { "sindex", new OffsetTypePowerEnum(ParameterTypes.OffsetType.SegmentIndex) },
                { "segmentindex", new OffsetTypePowerEnum(ParameterTypes.OffsetType.SegmentIndex) },

                { "idx", new OffsetTypePowerEnum(ParameterTypes.OffsetType.Index) },
                { "index", new OffsetTypePowerEnum(ParameterTypes.OffsetType.Index) },

                { "word", new OffsetTypePowerEnum(ParameterTypes.OffsetType.Word) },
                { "wordidx", new OffsetTypePowerEnum(ParameterTypes.OffsetType.Word) },
                { "wordindex", new OffsetTypePowerEnum(ParameterTypes.OffsetType.Word) },

                { "line", new OffsetTypePowerEnum(ParameterTypes.OffsetType.Line) },
                { "linenumber", new OffsetTypePowerEnum(ParameterTypes.OffsetType.Line) },

                { "base", new OffsetTypePowerEnum(ParameterTypes.OffsetType.Baseline) },
                { "baseline", new OffsetTypePowerEnum(ParameterTypes.OffsetType.Baseline) },

                { "x", new OffsetTypePowerEnum(ParameterTypes.OffsetType.XPos) },
                { "xpos", new OffsetTypePowerEnum(ParameterTypes.OffsetType.XPos) },

                { "y", new OffsetTypePowerEnum(ParameterTypes.OffsetType.YPos) },
                { "ypos", new OffsetTypePowerEnum(ParameterTypes.OffsetType.YPos) },
            };
    }
}