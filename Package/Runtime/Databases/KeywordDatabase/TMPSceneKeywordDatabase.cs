using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TMPEffects.Databases;
using TMPEffects.ObjectChanged;
using TMPEffects.Parameters;
using TMPEffects.SerializedCollections;
using TMPEffects.TMPAnimations;
using UnityEngine;

namespace TMPEffects.Databases
{
    public partial class TMPSceneKeywordDatabase : MonoBehaviour, ITMPKeywordDatabase, INotifyObjectChanged
    {
        public event ObjectChangedEventHandler ObjectChanged;
        
        [SerializedDictionary(keyName: "Keyword", valueName: "Float")] [SerializeField]
        SerializedDictionary<string, float> floatKeywords;

        [SerializedDictionary(keyName: "Keyword", valueName: "Int")] [SerializeField]
        SerializedDictionary<string, int> intKeywords;

        // Really pointless but here for completions sake
        [SerializedDictionary(keyName: "Keyword", valueName: "Bool")] [SerializeField]
        SerializedDictionary<string, bool> boolKeywords;

        [SerializedDictionary(keyName: "Keyword", valueName: "Color")] [SerializeField]
        SerializedDictionary<string, Color> colorKeywords;

        [SerializedDictionary(keyName: "Keyword", valueName: "Vector3")] [SerializeField]
        SerializedDictionary<string, Vector3> vector3Keywords;

        [SerializedDictionary(keyName: "Keyword", valueName: "Anchor")] [SerializeField]
        SerializedDictionary<string, Vector2> anchorKeywords;

        [SerializedDictionary(keyName: "Keyword", valueName: "Curve")] [SerializeField]
        SerializedDictionary<string, AnimationCurve> animationCurveKeywords;

        [SerializedDictionary(keyName: "Keyword", valueName: "OffsetType")] [SerializeField]
        SerializedDictionary<string, SceneOffsetTypePowerEnum> offsetTypeKeywords;

        [SerializedDictionary(keyName: "Keyword", valueName: "Unity Object")] [SerializeField]
        SerializedDictionary<string, UnityEngine.Object> unityObjectKeywords;

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