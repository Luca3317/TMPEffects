using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AYellowpaper.SerializedCollections.Editor.Data
{
    [System.Serializable]
    internal class ElementData
    {
        [SerializeField]
        private bool _isListToggleActive = false;

        public ElementSettings Settings { get; }
        public bool ShowAsList => Settings.HasListDrawerToggle && IsListToggleActive;
        public bool IsListToggleActive { get => _isListToggleActive; set => _isListToggleActive = value; }
        public DisplayType EffectiveDisplayType => ShowAsList ? DisplayType.List : Settings.DisplayType;

        public ElementData(ElementSettings elementSettings)
        {
            Settings = elementSettings;
        }
    }
}