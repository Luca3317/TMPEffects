using System;
using System.Collections.Generic;
using System.ComponentModel;
using TMPEffects.Databases;
using TMPEffects.ObjectChanged;
using TMPEffects.SerializedCollections;
using TMPEffects.TMPAnimations;
using TMPEffects.TMPAnimations.Animations;

namespace TMPEffects.Components.Animator
{
    internal class KeywordDatabaseWrapper : INotifyObjectChanged, IDisposable
    {
        public event ObjectChangedEventHandler ObjectChanged;
        public ITMPKeywordDatabase Database => compDatabase;
        
        private ITMPKeywordDatabase[] databases;
        private bool disposed = false;
        private CompositeTMPKeywordDatabase compDatabase;

        public KeywordDatabaseWrapper(params ITMPKeywordDatabase[] databases)
        {
            this.databases = databases;

            for (int i = 0; i < this.databases.Length; i++)
            {
                var db = this.databases[i];
                if (db == null) continue;
                if (db is INotifyObjectChanged notifyObjectChanged)
                    notifyObjectChanged.ObjectChanged += RaiseObjectChanged;
            }

            compDatabase = new CompositeTMPKeywordDatabase(databases);
        }

        private void RaiseObjectChanged(object sender)
        {
            ObjectChanged?.Invoke(this);
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            for (int i = 0; i < this.databases.Length; i++)
            {
                var db = this.databases[i];
                if (db == null) continue;
                if (db is INotifyObjectChanged notifyObjectChanged)
                    notifyObjectChanged.ObjectChanged -= RaiseObjectChanged;
            }
            databases = null;
            ObjectChanged = null;
        }
    }
}


// Wraps a database and sceneanimations dictionary
internal class AnimationDatabase<TDB, TScene> : ITMPEffectDatabase<ITMPAnimation>, INotifyObjectChanged, IDisposable
    where TDB : class, ITMPEffectDatabase<ITMPAnimation>, INotifyObjectChanged where TScene : TMPSceneAnimationBase
{
    public TDB Database => database;
    public SerializedObservableDictionary<string, TScene> SceneAnimations => sceneAnimations;
    private TDB database;
    private SerializedObservableDictionary<string, TScene> sceneAnimations;

    public event ObjectChangedEventHandler ObjectChanged;

    private Dictionary<string, ITMPAnimation> customAnimations;
    private bool disposed = false;

    public AnimationDatabase(TDB database, SerializedObservableDictionary<string, TScene> sceneAnimations)
    {
        this.database = database;
        this.sceneAnimations = sceneAnimations;
        customAnimations = new Dictionary<string, ITMPAnimation>();

        if (database != null) database.ObjectChanged += RaiseObjectChanged;
        if (sceneAnimations != null) sceneAnimations.ObjectChanged += RaiseObjectChanged;
    }

    public void AddAnimation(string key, ITMPAnimation animation)
    {
        if (customAnimations.ContainsKey(key)) throw new System.InvalidOperationException();
        customAnimations[key] = animation;
        RaiseObjectChanged(this);
    }

    public void RemoveAnimation(string key)
    {
        customAnimations.Remove(key);
        RaiseObjectChanged(this);
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
        contains = sceneAnimations != null && sceneAnimations.ContainsKey(name) && sceneAnimations[name] != null;
        if (contains) return true;
        return customAnimations.ContainsKey(name);
    }

    public ITMPAnimation GetEffect(string name)
    {
        if (database != null && database.ContainsEffect(name)) return database.GetEffect(name);
        if (sceneAnimations != null && sceneAnimations.ContainsKey(name) && sceneAnimations[name] != null)
            return sceneAnimations[name];
        if (customAnimations.ContainsKey(name)) return customAnimations[name];
        throw new KeyNotFoundException(name);
    }

    public void Dispose()
    {
        if (disposed) return;
        disposed = true;
        if (database != null) database.ObjectChanged -= RaiseObjectChanged;
        if (sceneAnimations != null) sceneAnimations.ObjectChanged -= RaiseObjectChanged;
        database = null;
        sceneAnimations = null;
        customAnimations = null;
        ObjectChanged = null;
    }
}
