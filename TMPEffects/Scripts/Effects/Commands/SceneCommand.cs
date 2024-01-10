using System.Collections.Generic;
using TMPEffects.Components;
using UnityEngine.Events;

namespace TMPEffects.Commands
{
    [System.Serializable]
    public struct SceneCommand
    {
        public CommandType CommandType;
        public bool executeInstantly;
        public bool executeOnSkip;
        public UnityEvent<SceneCommandArgs> command;
    }

    [System.Serializable]
    public struct SceneCommandArgs
    {
        public TMPWriter writer;
        public Dictionary<string, string> parameters;

        public SceneCommandArgs(TMPWriter writer, Dictionary<string, string> parameters)
        {
            this.writer = writer;
            this.parameters = parameters;
        }
    }
}
