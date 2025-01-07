using System.Collections.Generic;
using TMPEffects.Databases;
using TMPEffects.ObjectChanged;
using TMPEffects.TMPCommands;
using UnityEngine;

namespace TMPEffects.TMPCommands
{
    /// <summary>
    /// Base class to derive from to create scene commands.
    /// </summary>
    public abstract class TMPSceneCommand : MonoBehaviour, ITMPCommand, INotifyObjectChanged
    {
        public abstract bool ValidateParameters(IDictionary<string, string> parameters,
            ITMPKeywordDatabase keywordDatabase);

        public abstract TagType TagType { get; }
        public abstract bool ExecuteInstantly { get; }
        public abstract bool ExecuteOnSkip { get; }
        public abstract bool ExecuteRepeatable { get; }
#if UNITY_EDITOR
        public virtual bool ExecuteInPreview => false;
#endif
        public abstract void ExecuteCommand(ICommandContext context);
        public abstract object GetNewCustomData();

        public abstract void SetParameters(object customData, IDictionary<string, string> parameters,
            ITMPKeywordDatabase keywordDatabase);

        public event ObjectChangedEventHandler ObjectChanged;

        protected virtual void OnValidate()
        {
            RaiseObjectChanged();
        }

        protected virtual void OnDestroy()
        {
            RaiseObjectChanged();
        }

        protected void RaiseObjectChanged()
        {
            ObjectChanged?.Invoke(this);
        }
    }
}