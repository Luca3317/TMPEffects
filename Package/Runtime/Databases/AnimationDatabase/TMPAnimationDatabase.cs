using System.Collections.Generic;
using UnityEngine;
using TMPEffects.TMPAnimations;
using TMPEffects.Components.Animator;
using System.ComponentModel;

namespace TMPEffects.Databases.AnimationDatabase
{
    /// <summary>
    /// Stores <see cref="TMPAnimation"/>, <see cref="TMPShowAnimation"/> and <see cref="TMPHideAnimation"/> animations.
    /// </summary>
    [CreateAssetMenu(fileName = "new TMPAnimationDatabase", menuName = "TMPEffects/Database/Animation Database", order = 0)]
    public class TMPAnimationDatabase : TMPEffectDatabase<ITMPAnimation>
    {
        /// <summary>
        /// The backing <see cref="TMPBasicAnimationDatabase"/> used.
        /// </summary>
        public TMPBasicAnimationDatabase BasicAnimationDatabase => basicAnimationDatabase;
        /// <summary>
        /// The backing <see cref="TMPShowAnimationDatabase"/> used.
        /// </summary>
        public TMPShowAnimationDatabase ShowAnimationDatabase => showAnimationDatabase;
        /// <summary>
        /// The backing <see cref="TMPHideAnimationDatabase"/> used.
        /// </summary>
        public TMPHideAnimationDatabase HideAnimationDatabase => hideAnimationDatabase;

        [SerializeField] private TMPBasicAnimationDatabase basicAnimationDatabase;
        [SerializeField] private TMPShowAnimationDatabase showAnimationDatabase;
        [SerializeField] private TMPHideAnimationDatabase hideAnimationDatabase;

        /// <summary>
        /// Check whether this database contains an animation associated with the given name.
        /// </summary>
        /// <param name="name">The identifier of the animation.</param>
        /// <param name="type">The type of animation.</param>
        /// <returns>true if this database contains an animation associated with the given name; false otherwise.</returns>
        public bool ContainsEffect(string name, TMPAnimationType type)
        {
            ITMPEffectDatabase<ITMPAnimation> db;
            switch (type)
            {
                case TMPAnimationType.Basic: db = basicAnimationDatabase; break;
                case TMPAnimationType.Show: db = showAnimationDatabase; break;
                case TMPAnimationType.Hide: db = hideAnimationDatabase; break;
                default: throw new System.ArgumentException(nameof(type));
            }

            if (db == null) return false;
            return db.ContainsEffect(name);
        }

        /// <summary>
        /// Check whether this database contains an animation associated with the given name.
        /// </summary>
        /// <param name="name">The identifier of the animation.</param>
        /// <returns>true if this database contains an animation associated with the given name; false otherwise.</returns>
        public override bool ContainsEffect(string name)
        {
            if (basicAnimationDatabase != null && basicAnimationDatabase.ContainsEffect(name)) return true;
            if (showAnimationDatabase != null && showAnimationDatabase.ContainsEffect(name)) return true;
            if (hideAnimationDatabase != null && hideAnimationDatabase.ContainsEffect(name)) return true;
            return false;
        }

        /// <summary>
        /// Get the animation associated with the given name.
        /// </summary>
        /// <param name="name">The identifier of the animation.</param>
        /// <param name="type">The type of animation.</param>
        /// <returns>The animation associated with the given name.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
        /// <exception cref="System.InvalidOperationException"></exception>
        public ITMPAnimation GetEffect(string name, TMPAnimationType type)
        {
            ITMPEffectDatabase<ITMPAnimation> db;
            switch (type)
            {
                case TMPAnimationType.Basic: db = basicAnimationDatabase; break;
                case TMPAnimationType.Show: db = showAnimationDatabase; break;
                case TMPAnimationType.Hide: db = hideAnimationDatabase; break;
                default: throw new System.ArgumentException(nameof(type));
            }

            if (db == null) return null;
            return db.GetEffect(name);
        }

        /// <summary>
        /// Get the animation associated with the given name.
        /// </summary>
        /// <param name="name">The identifier of the animation.</param>
        /// <returns>The animation associated with the given name.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
        /// <exception cref="System.InvalidOperationException"></exception>
        public override ITMPAnimation GetEffect(string name)
        {
            if (basicAnimationDatabase != null && basicAnimationDatabase.ContainsEffect(name)) return basicAnimationDatabase.GetEffect(name);
            if (showAnimationDatabase != null && showAnimationDatabase.ContainsEffect(name)) return showAnimationDatabase.GetEffect(name);
            if (hideAnimationDatabase != null && hideAnimationDatabase.ContainsEffect(name)) return hideAnimationDatabase.GetEffect(name);
            throw new KeyNotFoundException();
        }




        [SerializeField, HideInInspector] private TMPBasicAnimationDatabase prevBasicAnimationDatabase;
        [SerializeField, HideInInspector] private TMPShowAnimationDatabase prevShowAnimationDatabase;
        [SerializeField, HideInInspector] private TMPHideAnimationDatabase prevHideAnimationDatabase;

        protected override void OnValidate()
        {
            if (prevBasicAnimationDatabase != basicAnimationDatabase)
            {
                if (prevBasicAnimationDatabase != null)
                    prevBasicAnimationDatabase.ObjectChanged -= OnChanged;
                //prevBasicAnimationDatabase.StopListenForChanges(OnChanged);

                if (basicAnimationDatabase != null)
                    basicAnimationDatabase.ObjectChanged += OnChanged;
                //basicAnimationDatabase.ListenForChanges(OnChanged);

                prevBasicAnimationDatabase = basicAnimationDatabase;
            }

            if (prevShowAnimationDatabase != showAnimationDatabase)
            {
                if (prevShowAnimationDatabase != null)
                    prevShowAnimationDatabase.ObjectChanged -= OnChanged;
                //prevShowAnimationDatabase.StopListenForChanges(OnChanged);

                if (showAnimationDatabase != null)
                    showAnimationDatabase.ObjectChanged += OnChanged;
                //showAnimationDatabase.ListenForChanges(OnChanged);

                prevShowAnimationDatabase = showAnimationDatabase;
            }

            if (prevHideAnimationDatabase != hideAnimationDatabase)
            {
                if (prevHideAnimationDatabase != null)
                    prevHideAnimationDatabase.ObjectChanged -= OnChanged;
                //prevHideAnimationDatabase.StopListenForChanges(OnChanged);

                if (hideAnimationDatabase != null)
                    hideAnimationDatabase.ObjectChanged += OnChanged;
                //hideAnimationDatabase.ListenForChanges(OnChanged);

                prevHideAnimationDatabase = hideAnimationDatabase;
            }

            RaiseDatabaseChanged();
        }

        private void OnChanged(object sender)
        {
            RaiseDatabaseChanged();
        }
    }
}