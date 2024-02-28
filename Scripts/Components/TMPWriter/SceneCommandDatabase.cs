using System.Collections;
using System.Collections.Generic;
using TMPEffects.Databases;
using TMPEffects.TMPCommands;
using UnityEngine;

namespace TMPEffects.Components.Writer
{
    internal class SceneCommandDatabase : ITMPEffectDatabase<ITMPCommand>
    {
        private IDictionary<string, SceneCommand> dict;

        public SceneCommandDatabase(IDictionary<string, SceneCommand> dict)
        {
            this.dict = dict;
        }

        public bool ContainsEffect(string name)
        {
            return dict.ContainsKey(name);
        }

        public ITMPCommand GetEffect(string name)
        {
            return dict[name];
        }
    }
}
