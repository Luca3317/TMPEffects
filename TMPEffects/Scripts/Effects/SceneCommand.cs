using System.Collections.Generic;
using TMPEffects.Components;
using TMPEffects.Tags;
using UnityEngine;
using UnityEngine.Events;

namespace TMPEffects.Commands
{
    [System.Serializable]
    public struct SceneCommand : ITMPCommand
    {
        public CommandType CommandType => commandType;
        public bool ExecuteInstantly => executeInstantly;
        public bool ExecuteOnSkip => executeOnSkip;
        //public UnityEvent<SceneCommandArgs> Command => command;

        [SerializeField] private CommandType commandType;
        [SerializeField] private bool executeInstantly;
        [SerializeField] private bool executeOnSkip;
        [SerializeField] private UnityEvent<TMPCommandArgs> command;

        public void ExecuteCommand(TMPCommandArgs args) => command?.Invoke(args);
    }
}
