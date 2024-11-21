using System;
using TMPEffects.ObjectChanged;
using TMPEffects.Parameters;
using TMPEffects.SerializedCollections;
using TMPEffects.TMPAnimations;
using UnityEngine;

namespace TMPEffects.Databases
{
    [CreateAssetMenu(fileName = "new KeywordDatabase", menuName = "TMPEffects/Database/Keywords")]
    public partial class TMPKeywordDatabase : ScriptableObject, ITMPKeywordDatabase, INotifyObjectChanged
    {
        public static TMPKeywordDatabase Global
        {
            get
            {
                if (globalInstance == null)
                {
                    globalInstance = Resources.Load<TMPKeywordDatabase>("TMPEffects.GlobalKeywordDatabase");
                }

                return globalInstance;
            }
        }

        private static TMPKeywordDatabase globalInstance = null;


        public event ObjectChangedEventHandler ObjectChanged;

        [SerializedDictionary(keyName: "Keyword", valueName: "Float")] [SerializeField]
        internal SerializedDictionary<string, float> floatKeywords;

        [SerializedDictionary(keyName: "Keyword", valueName: "Int")] [SerializeField]
        internal SerializedDictionary<string, int> intKeywords;

        // Really pointless but here for completions sake
        [SerializedDictionary(keyName: "Keyword", valueName: "Bool")] [SerializeField]
        internal SerializedDictionary<string, bool> boolKeywords;

        [SerializedDictionary(keyName: "Keyword", valueName: "Color")] [SerializeField]
        internal SerializedDictionary<string, Color> colorKeywords;

        [SerializedDictionary(keyName: "Keyword", valueName: "Vector3")] [SerializeField]
        internal SerializedDictionary<string, Vector3> vector3Keywords;

        [SerializedDictionary(keyName: "Keyword", valueName: "Anchor")] [SerializeField]
        internal SerializedDictionary<string, Vector2> anchorKeywords;

        [SerializedDictionary(keyName: "Keyword", valueName: "Curve")] [SerializeField]
        internal SerializedDictionary<string, AnimationCurve> animationCurveKeywords;

        [SerializedDictionary(keyName: "Keyword", valueName: "OffsetType")] [SerializeField]
        internal SerializedDictionary<string, OffsetTypePowerEnum> offsetTypeKeywords;

        [SerializedDictionary(keyName: "Keyword", valueName: "Unity Object")] [SerializeField]
        internal SerializedDictionary<string, UnityEngine.Object> unityObjectKeywords;

        public bool TryGetFloat(string str, out float result)
        {
            return floatKeywords.TryGetValue(str, out result);
        }

        public bool TryGetInt(string str, out int result)
        {
            return intKeywords.TryGetValue(str, out result);
        }

        public bool TryGetBool(string str, out bool result)
        {
            return boolKeywords.TryGetValue(str, out result);
        }

        public bool TryGetColor(string str, out Color result)
        {
            return colorKeywords.TryGetValue(str, out result);
        }

        public bool TryGetVector3(string str, out Vector3 result)
        {
            return vector3Keywords.TryGetValue(str, out result);
        }

        public bool TryGetAnchor(string str, out Vector2 result)
        {
            return anchorKeywords.TryGetValue(str, out result);
        }

        public bool TryGetAnimCurve(string str, out AnimationCurve result)
        {
            return animationCurveKeywords.TryGetValue(str, out result);
        }

        public bool TryGetOffsetType(string str, out ITMPOffsetProvider result)
        {
            bool success = offsetTypeKeywords.TryGetValue(str, out var offset);
            result = offset;
            return success;
        }

        public bool TryGetUnityObject(string str, out UnityEngine.Object result)
        {
            return unityObjectKeywords.TryGetValue(str, out result);
        }

        private void OnValidate()
        {
            RaiseDatabaseChanged();
        }

        private void OnDestroy()
        {
            RaiseDatabaseChanged();
        }

        private void RaiseDatabaseChanged()
        {
            ObjectChanged?.Invoke(this);
        }
    }
}