using System;
using System.Collections.Generic;
using TMPEffects.Databases;
using TMPEffects.TMPCommands;
using UnityEngine;

namespace TMPEffects.TMPCommands
{
    /// <summary>
    /// Struct wrapping either <see cref="TMPGenericSceneCommand"/> or <see cref="TMPSceneCommand"/>.
    /// </summary>
    [Serializable]
    public struct TMPSceneCommandWrapper : ITMPCommand
    {
        /// <summary>
        /// The type of this wrapper.<br/>
        /// The <see cref="ITMPCommand"/> implementation delegates to <see cref="Generic"/> or <see cref="Custom"/>
        /// depending on the value of this.
        /// </summary>
        public TMPSceneCommandType Type
        {
            get => type;
            set => type = value;
        }

        /// <summary>
        /// The wrapped generic command.<br/>
        /// The <see cref="ITMPCommand"/> implementation of the wrapper delegates to this
        /// if <see cref="Type"/> is set to <see cref="TMPSceneCommandType.Custom"/>.
        /// </summary>
        public TMPGenericSceneCommand Generic
        {
            get => generic;
            set => generic = value;
        }

        /// <summary>
        /// The wrapped custom command.<br/>
        /// The <see cref="ITMPCommand"/> implementation of the wrapper delegates to this
        /// if <see cref="Type"/> is set to <see cref="TMPSceneCommandType.Generic"/>.
        /// </summary>
        public TMPSceneCommand Custom
        {
            get => custom;
            set => custom = value;
        }
        
        [SerializeField] private TMPSceneCommandType type;
        [SerializeField] private TMPGenericSceneCommand generic;
        [SerializeField] private TMPSceneCommand custom;

        private ITMPCommand BackingCommand => type == TMPSceneCommandType.Custom ? custom : generic;
        
        [Serializable]
        public enum TMPSceneCommandType
        {
            Custom,
            Generic
        }

        public TagType TagType => BackingCommand.TagType;

        public bool ExecuteInstantly => BackingCommand.ExecuteInstantly;

        public bool ExecuteOnSkip => BackingCommand.ExecuteOnSkip;

        public bool ExecuteRepeatable => BackingCommand.ExecuteRepeatable;

#if UNITY_EDITOR
        public bool ExecuteInPreview => BackingCommand.ExecuteInPreview;
#endif

        public bool ValidateParameters(IDictionary<string, string> parameters, ITMPKeywordDatabase keywordDatabase)
        {
            return BackingCommand.ValidateParameters(parameters, keywordDatabase);
        }

        public void ExecuteCommand(ICommandContext context)
        {
            BackingCommand.ExecuteCommand(context);
        }

        public object GetNewCustomData()
        {
            return BackingCommand.GetNewCustomData();
        }

        public void SetParameters(object customData, IDictionary<string, string> parameters,
            ITMPKeywordDatabase keywordDatabase)
        {
            BackingCommand.SetParameters(customData, parameters, keywordDatabase);
        }
    }
}