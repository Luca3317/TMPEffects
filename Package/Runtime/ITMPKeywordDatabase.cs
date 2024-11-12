using System.Collections.ObjectModel;
using TMPEffects.Parameters;
using UnityEngine;

namespace TMPEffects.Databases
{
    // TODO Potentially auto-generate this (+ implementations)
    public partial interface ITMPKeywordDatabase
    {
        // public ReadOnlyDictionary<string, float> FloatKeywords { get; }
        // public ReadOnlyDictionary<string, int> IntKeywords { get; }
        // public ReadOnlyDictionary<string, bool> BoolKeywords { get; }
        // public ReadOnlyDictionary<string, Color> ColorKeywords { get; }
        // public ReadOnlyDictionary<string, Vector3> Vector3Keywords { get; }
        // public ReadOnlyDictionary<string, Vector2> AnchorKeywords { get; }
        // public ReadOnlyDictionary<string, AnimationCurve> AnimationCurveKeywords { get; }
        // public ReadOnlyDictionary<string, OffsetTypeRedo> OffsetTypeKeywords { get; }

        public bool TryGetFloat(string str, out float result);
        public bool TryGetInt(string str, out int result);
        public bool TryGetBool(string str, out bool result);
        public bool TryGetColor(string str, out Color result);
        public bool TryGetVector3(string str, out Vector3 result);
        public bool TryGetAnchor(string str, out Vector2 result);
        public bool TryGetAnimCurve(string str, out AnimationCurve result);
        public bool TryGetUnityObject(string str, out UnityEngine.Object obj);
    }
}