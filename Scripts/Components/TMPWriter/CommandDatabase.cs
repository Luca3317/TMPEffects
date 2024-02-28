using System.Collections.Generic;
using TMPEffects.Databases;
using TMPEffects.TMPCommands;

namespace TMPEffects.Components.Writer
{
    internal class CommandDatabase : ITMPEffectDatabase<ITMPCommand>
    {
        private ITMPEffectDatabase<ITMPCommand> database;
        private IDictionary<string, SceneCommand> sceneDatabase;

        public CommandDatabase(ITMPEffectDatabase<ITMPCommand> database, IDictionary<string, SceneCommand> sceneDatabase)
        {
            this.database = database;
            this.sceneDatabase = sceneDatabase;
        }

        public bool ContainsEffect(string name)
        {
            bool contains = database != null && database.ContainsEffect(name);
            if (contains) return true;
            return sceneDatabase != null && sceneDatabase.ContainsKey(name);
        }

        public ITMPCommand GetEffect(string name)
        {
            if (database != null && database.ContainsEffect(name)) return database.GetEffect(name);
            if (sceneDatabase != null && sceneDatabase.ContainsKey(name)) return sceneDatabase[name];
            throw new KeyNotFoundException();
        }
    }
}