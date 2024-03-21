using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using TMPEffects.Databases;
using TMPEffects.ObjectChanged;
using TMPEffects.SerializedCollections;
using TMPEffects.TMPAnimations;
using TMPEffects.TMPSceneAnimations;

namespace TMPEffects.Components.Animator
{
    // Wraps a database and sceneanimations dictionary
    internal class AnimationDatabase<TDB, TScene> : ITMPEffectDatabase<ITMPAnimation>, INotifyObjectChanged, IDisposable where TDB : class, ITMPEffectDatabase<ITMPAnimation>, INotifyObjectChanged where TScene : TMPSceneAnimationBase
    {
        public TDB Database => database;
        public SerializedObservableDictionary<string, TScene> SceneAnimations => sceneAnimations;
        private TDB database;
        private SerializedObservableDictionary<string, TScene> sceneAnimations;

        public event ObjectChangedEventHandler ObjectChanged;

        private bool disposed = false;

        public AnimationDatabase(TDB database, SerializedObservableDictionary<string, TScene> sceneAnimations)
        {
            this.database = database;
            this.sceneAnimations = sceneAnimations;

            if (database != null) database.ObjectChanged += RaiseObjectChanged;
            if (sceneAnimations != null) sceneAnimations.PropertyChanged += RaiseObjectChanged;
        }

        private void RaiseObjectChanged(object sender)
        {
            ObjectChanged?.Invoke(this);
        }

        private void RaiseObjectChanged(object sender, PropertyChangedEventArgs args)
        {
            ObjectChanged?.Invoke(this);
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
            throw new KeyNotFoundException(name);
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            if (database != null) database.ObjectChanged -= RaiseObjectChanged;
            if (sceneAnimations != null) sceneAnimations.PropertyChanged -= RaiseObjectChanged;
            database = null;
            sceneAnimations = null;
        }
    }
}
