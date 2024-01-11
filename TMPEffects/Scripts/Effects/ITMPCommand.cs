using System.Collections;
using System.Collections.Generic;
using TMPEffects.Components;
using TMPEffects.Tags;
using UnityEngine;

namespace TMPEffects
{
    public interface ITMPCommand
    {
        public CommandType CommandType { get; }
        public bool ExecuteInstantly { get; }
        public bool ExecuteOnSkip { get; }

        public void ExecuteCommand(TMPCommandArgs args);
    }
}
