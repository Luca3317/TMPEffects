using TMPEffects.SerializedCollections;
using UnityEngine;
using TMPEffects.TMPAnimations;

namespace TMPEffects.Databases.AnimationDatabase
{
    /// <summary>
    /// Stores <see cref="TMPHideAnimation"/>.
    /// </summary>
    [CreateAssetMenu(fileName = "new TMPHideAnimationDatabase", menuName = "TMPEffects/Database/Hide Animation Database", order = 13)]
    public class TMPHideAnimationDatabase : TMPAnimationDatabaseBase<TMPHideAnimation>
    {
        [SerializedDictionary(keyName: "Tag Name", valueName: "Hide Animation")]
        [SerializeField] SerializedDictionary<string, TMPHideAnimation> hideAnimations;

        /// <summary>
        /// Check whether this database contains an animation associated with the given name.
        /// </summary>
        /// <param name="name">The identifier of the animation.</param>
        /// <returns>true if this database contains an animation associated with the given name; false otherwise.</returns>
        public override bool ContainsEffect(string name)
        {
            return hideAnimations.ContainsKey(name);
        }

        /// <summary>
        /// Get the animation associated with the given name.
        /// </summary>
        /// <param name="name">The identifier of the animation.</param>
        /// <returns>The animation associated with the given name.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
        /// <exception cref="System.InvalidOperationException"></exception>
        public override TMPHideAnimation GetEffect(string name)
        {
            var anim = hideAnimations[name];
            if (anim == null)
            {
                throw new System.InvalidOperationException($"The animation {name} is unassigned on database {this.name}");
            }
            return anim;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            foreach (var animation in hideAnimations.Values)
            {
                if (animation == null) continue;

                animation.ObjectChanged -= OnAnimationChanged;
                animation.ObjectChanged += OnAnimationChanged;
            }

            RaiseDatabaseChanged();
        }

        private void OnAnimationChanged(object sender)
        {
            RaiseDatabaseChanged();
        }
#endif
    }
}