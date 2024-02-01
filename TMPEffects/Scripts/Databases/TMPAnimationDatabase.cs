using System.Collections.Generic;
using UnityEngine;
using TMPEffects.Animations;

namespace TMPEffects.Databases
{
    /// <summary>
    /// Stores <see cref="TMPAnimation"/>, <see cref="TMPShowAnimation"/> and <see cref="TMPHideAnimation"/> animations.
    /// </summary>
    [CreateAssetMenu(fileName = "new TMPAnimationDatabase", menuName = "TMPEffects/Database/Animation Database", order = 0)]
    public class TMPAnimationDatabase : TMPEffectDatabase<ITMPAnimation>
    {
        public TMPBasicAnimationDatabase basicAnimationDatabase;
        public TMPShowAnimationDatabase showAnimationDatabase;
        public TMPHideAnimationDatabase hideAnimationDatabase;

        public bool Contains(string name, TMPAnimationType type)
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

        public override bool ContainsEffect(string name)
        {
            if (basicAnimationDatabase != null && basicAnimationDatabase.ContainsEffect(name)) return true;
            if (showAnimationDatabase != null && showAnimationDatabase.ContainsEffect(name)) return true;
            if (hideAnimationDatabase != null && hideAnimationDatabase.ContainsEffect(name)) return true;
            return false;
        }

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

        public override ITMPAnimation GetEffect(string name)
        {
            if (basicAnimationDatabase != null && basicAnimationDatabase.ContainsEffect(name)) return basicAnimationDatabase.GetEffect(name);
            if (showAnimationDatabase != null && showAnimationDatabase.ContainsEffect(name)) return showAnimationDatabase.GetEffect(name);
            if (hideAnimationDatabase != null && hideAnimationDatabase.ContainsEffect(name)) return hideAnimationDatabase.GetEffect(name);
            throw new KeyNotFoundException();
        }
    }
}