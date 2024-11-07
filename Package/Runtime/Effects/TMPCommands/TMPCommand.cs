using System.Collections.Generic;
using TMPEffects.Components.Writer;
using TMPEffects.ObjectChanged;
using UnityEngine;

namespace TMPEffects.TMPCommands
{
    /// <summary>
    /// Base class for commands.
    /// </summary>
    public abstract class TMPCommand : ScriptableObject, ITMPCommand, INotifyObjectChanged
    {
        ///<inheritdoc/>
        public abstract TagType TagType { get; }

        ///<inheritdoc/>
        public abstract bool ExecuteInstantly { get; }

        ///<inheritdoc/>
        public abstract bool ExecuteOnSkip { get; }

        ///<inheritdoc/>
        public virtual bool ExecuteRepeatable => false;

#if UNITY_EDITOR
        ///<inheritdoc/>
        public virtual bool ExecuteInPreview => false;
#endif

        ///<inheritdoc/>
        public abstract void ExecuteCommand(IDictionary<string, string> parameters, ICommandContext context);
        
        ///<inheritdoc/>
        public abstract bool ValidateParameters(IDictionary<string, string> parameters, IWriterContext context);

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