using UnityEngine;

namespace TMPEffects.Databases
{
    /// <summary>
    /// Base-class to create your own implementation of <see cref="TMPKeywordDatabase"/>.<br/>
    /// Provides keywords used for parsing TMPEffects tags.
    /// </summary>
    public abstract partial class TMPKeywordDatabaseBase : ScriptableObject, ITMPKeywordDatabase
    {
        public abstract bool TryGetFloat(string str, out float result);
        public abstract bool TryGetInt(string str, out int result);
        public abstract bool TryGetBool(string str, out bool result);
        public abstract bool TryGetColor(string str, out Color result);
        public abstract bool TryGetVector3(string str, out Vector3 result);
        public abstract bool TryGetAnchor(string str, out Vector2 result);
        public abstract bool TryGetAnimCurve(string str, out AnimationCurve result);
        public abstract bool TryGetUnityObject(string str, out Object obj);
    }
}