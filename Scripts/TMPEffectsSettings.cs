using System.Collections;
using System.Collections.Generic;
using TMPEffects;
using TMPEffects.Databases.AnimationDatabase;
using TMPEffects.Databases.CommandDatabase;
using UnityEditor;
using UnityEngine;

namespace TMPEffects
{
    [FilePath("Assets/Unity-TMPEffects", FilePathAttribute.Location.ProjectFolder)]
    public class TMPEffectsSettings : ScriptableSingleton<TMPEffectsSettings>
    {
        [SerializeField] public TMPAnimationDatabase defaultAnimationDatabase;
        [SerializeField] public TMPCommandDatabase defaultCommandDatabase;
    
        public void Log()
        {
            Debug.Log("ayo defualtanim is " + defaultAnimationDatabase?.name);
        }
    }
}