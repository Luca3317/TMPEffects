using System.Collections.Generic;
using System.ComponentModel;
using TMPEffects.TMPCommands;
using UnityEngine;
using UnityEngine.Events;

namespace TMPEffects.TMPCommands
{
    [System.Serializable]
    public struct SceneCommand : ITMPCommand, INotifyPropertyChanged, ISerializationCallbackReceiver
    {
        public TagType TagType => commandType;
        public bool ExecuteInstantly => executeInstantly;
        public bool ExecuteOnSkip => executeOnSkip;
        public bool ExecuteRepeatable => executeRepeatable;

#if UNITY_EDITOR
        public bool ExecuteInPreview => false;
#endif
        [Tooltip("Whether tags of this command operate on a range (and therefore need to be closed) or on an index.")]
        [SerializeField] private TagType commandType;
        [Tooltip("Whether the command is executed instantly when the writer begins writing the text, regardless of its position within it.")]
        [SerializeField] private bool executeInstantly;
        [Tooltip("Whether the command should be executed when the tag's position is skipped by the writer. Check for essential commands, e.g. triggering a quest.")]
        [SerializeField] private bool executeOnSkip;
        [Tooltip("Whether the command should be allowed to be executed multiple times. Relevant for when the writer is reset while writing.")]
        [SerializeField] private bool executeRepeatable;
        [Tooltip("The methods to trigger.")]
        [SerializeField] private UnityEvent<TMPCommandArgs> command;

        public event PropertyChangedEventHandler PropertyChanged;

        public void ExecuteCommand(TMPCommandArgs args) => command?.Invoke(args);

        public bool ValidateParameters(IDictionary<string, string> parameters)
        {
            return true;
        }

        public void OnBeforeSerialize()
        {
            if (prevExecuteInstantly != executeInstantly ||
                prevCommandType != commandType ||
                prevExecuteRepeatable != executeRepeatable ||
                prevExecuteOnSkip != executeOnSkip ||
                args.Count != command.GetPersistentEventCount())
            {
                    Debug.Log("RAISING");
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
                return;
            }

            for (int i = 0; i < command.GetPersistentEventCount(); i++)
            {
                var element = args[i];

                if (element.Item1 != command.GetPersistentTarget(i) || element.Item2 != command.GetPersistentListenerState(i))
                {
                    Debug.Log("RAISING");
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
                    return;
                }
            }
        }

        public void OnAfterDeserialize()
        {
            prevExecuteInstantly = executeInstantly;
            prevExecuteOnSkip = executeOnSkip;
            prevExecuteRepeatable = executeRepeatable;
            prevCommandType = commandType;

            args = new List<(Object, UnityEventCallState)>();
            for (int i = 0; i < command.GetPersistentEventCount(); i++)
            {
                args.Add((command.GetPersistentTarget(i), command.GetPersistentListenerState(i)));
            }

            //prevCommand = new UnityEvent<TMPCommandArgs>();
            //for (int i = 0; i < command.GetPersistentEventCount(); i++)
            //{
            //    prevCommand.SetPersistentListenerState(i, command.GetPersistentListenerState(i));
            //}
        }

#if UNITY_EDITOR
        private TagType prevCommandType;
        private bool prevExecuteInstantly;
        private bool prevExecuteOnSkip;
        private bool prevExecuteRepeatable;
        //private UnityEvent<TMPCommandArgs> prevCommand;
        private List<(UnityEngine.Object, UnityEventCallState)> args;
#endif
    }
}