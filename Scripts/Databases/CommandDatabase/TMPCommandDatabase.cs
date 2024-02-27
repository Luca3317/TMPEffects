using TMPEffects.SerializedCollections;
using UnityEngine;
using TMPEffects.TMPCommands;

namespace TMPEffects.Databases.CommandDatabase
{
    /// <summary>
    /// Stores <see cref="TMPCommand"/>.
    /// </summary>
    [CreateAssetMenu(fileName = "new TMPCommandDatabase", menuName = "TMPEffects/Database/Command Database", order = 30)]
    public class TMPCommandDatabase : TMPEffectDatabase<TMPCommand>
    {
        [SerializedDictionary(keyName: "Tag Name", valueName: "Command")]
        [SerializeField] SerializedDictionary<string, TMPCommand> commands;

        /// <summary>
        /// Check whether this database contains a command associated with the given name.
        /// </summary>
        /// <param name="name">The identifier of the command.</param>
        /// <returns>true if this database contains a command associated with the given name; false otherwise.</returns>
        public override bool ContainsEffect(string name)
        {
            return commands.ContainsKey(name);
        }

        /// <summary>
        /// Get the command associated with the given name.
        /// </summary>
        /// <param name="name">The identifier of the command.</param>
        /// <returns>The command associated with the given name.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
        public override TMPCommand GetEffect(string name)
        {
            return commands[name];
        }
    }
}
