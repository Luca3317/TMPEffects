using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AYellowpaper.SerializedCollections
{
    public static class SerializedCollectionsUtility
    {
        public static bool IsValidKey(object obj)
        {
            // we catch this error if we are not on the main thread and simply return false as we assume the object is null
            try
            {
                return !(obj is Object unityObject && unityObject == null);
            }
            catch
            {
                return false;
            }
        }
    }
}