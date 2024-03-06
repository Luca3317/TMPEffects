using System.Collections.Generic;
using System.Diagnostics;
using TMPEffects.Databases;
using TMPEffects.TMPAnimations;

namespace TMPEffects.Components.Animator
{
    // Wraps a database and sceneanimations dictionary
    internal class AnimationDatabase<T> : ITMPEffectDatabase<ITMPAnimation> where T : TMPSceneAnimationBase
    {
        public ITMPEffectDatabase<ITMPAnimation> Database => database;
        public IDictionary<string, T> SceneAnimations => sceneAnimations;
        private readonly ITMPEffectDatabase<ITMPAnimation> database;
        private readonly IDictionary<string, T> sceneAnimations;
        public AnimationDatabase(ITMPEffectDatabase<ITMPAnimation> database, IDictionary<string, T> sceneAnimations)
        {
            this.database = database;
            this.sceneAnimations = sceneAnimations;
        }
        public bool ContainsEffect(string name)
        {
            bool contains = database != null && database.ContainsEffect(name);
            if (contains) return true;
            return sceneAnimations != null && sceneAnimations.ContainsKey(name) && sceneAnimations[name] != null;
        }
        public ITMPAnimation GetEffect(string name)
        {
            if (database != null && database.ContainsEffect(name)) return database.GetEffect(name);
            if (sceneAnimations != null && sceneAnimations.ContainsKey(name) && sceneAnimations[name] != null) return sceneAnimations[name];
            UnityEngine.Debug.LogWarning("Gonna throw an error with db null: " + (database == null) + " and scene animaiotns null: " + (sceneAnimations == null));
            throw new KeyNotFoundException(name);
        }
    }
}
