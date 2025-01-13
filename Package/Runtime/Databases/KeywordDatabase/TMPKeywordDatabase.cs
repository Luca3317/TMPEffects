using TMPEffects.ObjectChanged;
using TMPEffects.Parameters;
using TMPEffects.SerializedCollections;
using UnityEngine;

namespace TMPEffects.Databases
{
    /// <summary>
    /// Provides keywords used for parsing TMPEffects tags.
    /// </summary>
    [CreateAssetMenu(fileName = "new KeywordDatabase", menuName = "TMPEffects/Database/Keywords")]
    public sealed partial class TMPKeywordDatabase : TMPKeywordDatabaseBase, ITMPKeywordDatabase, INotifyObjectChanged
    {
        public event ObjectChangedEventHandler ObjectChanged;

        [SerializedDictionary(keyName: "Keyword", valueName: "Float")] [SerializeField]
        internal SerializedDictionary<string, float> floatKeywords;

        [SerializedDictionary(keyName: "Keyword", valueName: "Int")] [SerializeField]
        internal SerializedDictionary<string, int> intKeywords;

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

        public override bool TryGetFloat(string str, out float result)
        {
            return floatKeywords.TryGetValue(str, out result);
        }

        public override bool TryGetInt(string str, out int result)
        {
            return intKeywords.TryGetValue(str, out result);
        }

        public override bool TryGetBool(string str, out bool result)
        {
            return boolKeywords.TryGetValue(str, out result);
        }

        public override bool TryGetColor(string str, out Color result)
        {
            return colorKeywords.TryGetValue(str, out result);
        }

        public override bool TryGetVector3(string str, out Vector3 result)
        {
            return vector3Keywords.TryGetValue(str, out result);
        }

        public override bool TryGetAnchor(string str, out Vector2 result)
        {
            return anchorKeywords.TryGetValue(str, out result);
        }

        public override bool TryGetAnimCurve(string str, out AnimationCurve result)
        {
            return animationCurveKeywords.TryGetValue(str, out result);
        }

        public override bool TryGetUnityObject(string str, out UnityEngine.Object result)
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