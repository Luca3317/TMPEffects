using System.Collections.ObjectModel;
using TMPEffects.Parameters;
using UnityEngine;

namespace TMPEffects.Databases
{
    /// <summary>
    /// Provides keywords used for parsing TMPEffects tags.
    /// </summary>
    public partial interface ITMPKeywordDatabase
    {
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