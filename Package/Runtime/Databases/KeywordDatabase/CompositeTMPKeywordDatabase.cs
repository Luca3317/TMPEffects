using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPEffects.Databases;
using TMPEffects.Parameters;
using UnityEngine;

namespace TMPEffects.Databases
{
    /// <summary>
    /// A <see cref="ITMPKeywordDatabase"/> that allows you to combine multiple databases.<br/>
    /// The first added databases take precedence / are checked first.
    /// </summary>
    public sealed partial class CompositeTMPKeywordDatabase : ITMPKeywordDatabase
    {
        public IEnumerable<ITMPKeywordDatabase> Databases
            => databases.Where(db => db != null);

        private ITMPKeywordDatabase[] databases;

        public CompositeTMPKeywordDatabase(ITMPKeywordDatabase[] databases)
        {
            this.databases = databases;
        }

        public bool TryGetFloat(string str, out float result)
        {
            for (int i = 0; i < databases.Length; i++)
            {
                var db = databases[i];
                if (db != null)
                    if (db.TryGetFloat(str, out result))
                        return true;
            }

            result = default;
            return false;
        }

        public bool TryGetInt(string str, out int result)
        {
            for (int i = 0; i < databases.Length; i++)
            {
                var db = databases[i];
                if (db != null)
                    if (db.TryGetInt(str, out result))
                        return true;
            }

            result = default;
            return false;
        }

        public bool TryGetBool(string str, out bool result)
        {
            for (int i = 0; i < databases.Length; i++)
            {
                var db = databases[i];
                if (db != null)
                    if (db.TryGetBool(str, out result))
                        return true;
            }

            result = default;
            return false;
        }

        public bool TryGetColor(string str, out Color result)
        {
            for (int i = 0; i < databases.Length; i++)
            {
                var db = databases[i];
                if (db != null)
                    if (db.TryGetColor(str, out result))
                        return true;
            }

            result = default;
            return false;
        }

        public bool TryGetVector3(string str, out Vector3 result)
        {
            for (int i = 0; i < databases.Length; i++)
            {
                var db = databases[i];
                if (db != null)
                    if (db.TryGetVector3(str, out result))
                        return true;
            }

            result = default;
            return false;
        }

        public bool TryGetAnchor(string str, out Vector2 result)
        {
            for (int i = 0; i < databases.Length; i++)
            {
                var db = databases[i];
                if (db != null)
                    if (db.TryGetAnchor(str, out result))
                        return true;
            }

            result = default;
            return false;
        }

        public bool TryGetAnimCurve(string str, out AnimationCurve result)
        {
            for (int i = 0; i < databases.Length; i++)
            {
                var db = databases[i];
                if (db != null)
                    if (db.TryGetAnimCurve(str, out result))
                        return true;
            }

            result = default;
            return false;
        }

        public bool TryGetUnityObject(string str, out Object result)
        {
            for (int i = 0; i < databases.Length; i++)
            {
                var db = databases[i];
                if (db != null)
                    if (db.TryGetUnityObject(str, out result))
                        return true;
            }

            result = default;
            return false;
        }
    }
}