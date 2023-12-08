using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMPEffects.Extensions
{
    public static class Extensions
    {
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            T comp = gameObject.GetComponent<T>();
            if (comp == null) comp = gameObject.AddComponent<T>();
            return comp;
        }
    }
}