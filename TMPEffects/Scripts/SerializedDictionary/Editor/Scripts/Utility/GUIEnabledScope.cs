using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AYellowpaper.SerializedCollections.Editor
{
    public struct GUIEnabledScope : IDisposable
    {
        public readonly bool PreviouslyEnabled;

        public GUIEnabledScope(bool enabled)
        {
            PreviouslyEnabled = GUI.enabled;
            GUI.enabled = enabled;
        }

        public void Dispose()
        {
            GUI.enabled = PreviouslyEnabled;
        }
    }
}