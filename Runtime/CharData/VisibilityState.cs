using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMPEffects.CharacterData
{
    /// <summary>
    /// A character's visibility state.<br/>
    /// A character can be either fully shown or hidden, or in the process of being shown or hidden.
    /// </summary>
    public enum VisibilityState : int
    {
        /// <summary>
        /// The character is fully shown.
        /// </summary>
        Shown = 10,
        /// <summary>
        /// The character is fully hidden.
        /// </summary>
        Hidden = -10,
        /// <summary>
        /// The character is in the process of being shown.
        /// </summary>
        Showing = 5,
        /// <summary>
        /// The character is in the process of being hidden.
        /// </summary>
        Hiding = -5
    }
}
