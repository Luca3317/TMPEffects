using TMPEffects.SerializedCollections;
using UnityEngine;
using TMPEffects.TMPAnimations;

namespace TMPEffects.Databases.AnimationDatabase
{
    /// <summary>
    /// Stores <see cref="TMPAnimation"/>.
    /// </summary>
    [CreateAssetMenu(fileName = "new TMPBasicAnimationDatabase", menuName = "TMPEffects/Database/Basic Animation Database", order = 11)]
    public class TMPBasicAnimationDatabase : TMPAnimationDatabaseBase<TMPAnimation>
    {
        [SerializedDictionary(keyName: "Tag Name", valueName: "Animation")]
        [SerializeField] SerializedDictionary<string, TMPAnimation> animations;

        /// <summary>
        /// Check whether this database contains an animation associated with the given name.
        /// </summary>
        /// <param name="name">The identifier of the animation.</param>
        /// <returns>true if this database contains an animation associated with the given name; false otherwise.</returns>
        public override bool ContainsEffect(string name)
        {
            return animations.ContainsKey(name);
        }

        /// <summary>
        /// Get the animation associated with the given name.
        /// </summary>
        /// <param name="name">The identifier of the animation.</param>
        /// <returns>The animation associated with the given name.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
        /// <exception cref="System.InvalidOperationException"></exception>
        public override TMPAnimation GetEffect(string name)
        {
            var anim = animations[name];
            if (anim == null)
            {
                throw new System.InvalidOperationException($"The animation {name} is unassigned on database {this.name}");
            }
            return anim;
        }
    }
}
