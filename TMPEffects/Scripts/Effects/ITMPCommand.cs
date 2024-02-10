using System.Collections;
using System.Collections.Generic;
using TMPEffects.Components;
using TMPEffects.Tags;
using UnityEngine;

namespace TMPEffects
{
    public interface ITMPCommand
    {
        public TagType TagType { get; }

        public bool ExecuteInstantly { get; }
        public bool ExecuteOnSkip { get; }
        public bool ExecuteRepeatable { get; }

#if UNITY_EDITOR
        public bool ExecuteInPreview { get; }
#endif

        public void ExecuteCommand(TMPCommandArgs args);
    }
}
