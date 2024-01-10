using System.Collections;
using System.Collections.Generic;
using TMPEffects.Tags;
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

        public static void AddRange<T>(this ICollection<T> dst, IEnumerable<T> src)
        {
            List<T> tags = dst as List<T>;
            if (tags == null)
            {
                foreach (var tag in src)
                {
                    dst.Add(tag);
                }
            }
            else tags.AddRange(src);
        }
    }
}