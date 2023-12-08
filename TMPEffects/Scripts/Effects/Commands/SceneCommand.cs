using System.Collections.Generic;
using UnityEngine.Events;

namespace TMPEffects.Commands
{
    [System.Serializable]
    public struct SceneCommand
    {
        public CommandType CommandType;
        public bool executeInstantly;
        public UnityEvent<Dictionary<string, string>> command;
    }
}
