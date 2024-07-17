using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using TMPEffects.Databases;
using TMPEffects.Databases.CommandDatabase;
using TMPEffects.ObjectChanged;
using TMPEffects.SerializedCollections;
using TMPEffects.TMPAnimations;
using TMPEffects.TMPCommands;

namespace TMPEffects.Components.Writer
{
    // Wraps a database and sceneanimations dictionary
    internal class CommandDatabase : ITMPEffectDatabase<ITMPCommand>, INotifyObjectChanged, IDisposable
    {
        public TMPCommandDatabase Database => database;
        public IDictionary<string, TMPSceneCommand> SceneCommands => sceneCommands;
        private TMPCommandDatabase database;
        private IDictionary<string, TMPSceneCommand> sceneCommands;

        public event ObjectChangedEventHandler ObjectChanged;

        private bool disposed = false;

        public CommandDatabase(TMPCommandDatabase database, IDictionary<string, TMPSceneCommand> sceneCommands)
        {
            this.database = database;
            this.sceneCommands = sceneCommands;

            if (database != null) database.ObjectChanged += RaiseObjectChanged;
            //if (sceneCommands != null) sceneCommands.PropertyChanged += RaiseObjectChanged;
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
            return sceneCommands != null && sceneCommands.ContainsKey(name);
        }

        public ITMPCommand GetEffect(string name)
        {
            if (database != null && database.ContainsEffect(name)) return database.GetEffect(name);
            if (sceneCommands != null && sceneCommands.ContainsKey(name)) return sceneCommands[name];
            throw new KeyNotFoundException(name);
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            if (database != null) database.ObjectChanged -= RaiseObjectChanged;
            database = null;
            sceneCommands = null;
        }
    }
}
