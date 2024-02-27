using TMPEffects.SerializedCollections;
using UnityEngine;
using TMPEffects.TMPAnimations;

namespace TMPEffects.Databases.AnimationDatabase
{
    /// <summary>
    /// Stores <see cref="TMPShowAnimation"/> animations.
    /// </summary>
    [CreateAssetMenu(fileName = "new TMPShowAnimationDatabase", menuName = "TMPEffects/Database/Show Animation Database", order = 12)]
    public class TMPShowAnimationDatabase : TMPAnimationDatabaseBase<TMPShowAnimation>
    {
        [SerializedDictionary(keyName: "Tag Name", valueName: "Show Animation")]
        [SerializeField] SerializedDictionary<string, TMPShowAnimation> showAnimations;

        /// <summary>
        /// Check whether this database contains an animation associated with the given name.
        /// </summary>
        /// <param name="name">The identifier of the animation.</param>
        /// <returns>true if this database contains an animation associated with the given name; false otherwise.</returns>
        public override bool ContainsEffect(string name)
        {
            return showAnimations.ContainsKey(name);
        }

        /// <summary>
        /// Get the animation associated with the given name.
        /// </summary>
        /// <param name="name">The identifier of the animation.</param>
        /// <returns>The animation associated with the given name.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
        public override TMPShowAnimation GetEffect(string name)
        {
            return showAnimations[name];
        }
    }
}