using System.Collections;
using System.Collections.Generic;
using TMPEffects.Tags;
using UnityEngine;

namespace TMPEffects.Extensions
{
    public static class Extensions
    {
        /// <summary>
        /// Gets a reference to a component of type <see cref="T"/> on the specified GameObject.<br/>
        /// Will first add the component if not already present.
        /// </summary>
        /// <typeparam name="T">The type of Component to search for.</typeparam>
        /// <param name="gameObject"></param>
        /// <returns>A reference to a component of the type <see cref="T"/>.</returns>
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            T comp = gameObject.GetComponent<T>();
            if (comp == null) comp = gameObject.AddComponent<T>();
            return comp;
        }
    }
}