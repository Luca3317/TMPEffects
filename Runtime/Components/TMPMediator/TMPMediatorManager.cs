using System;
using System.Collections.Generic;
using TMPEffects.Components;
using TMPro;
using UnityEngine;

namespace TMPEffects.Components.Mediator
{
    /// <summary>
    /// Static manager class for <see cref="TMPMediator"/> instances.<br/>
    /// Allows components to easily share one <see cref="TMPMediator"/> instance, using their associated
    /// <see cref="TMP_Text"/> component as key.
    /// </summary>
    internal static class TMPMediatorManager
    {
        public static Dictionary<GameObject, ValueTuple<TMPMediator, List<object>>> mediators = new();

        /// <summary>
        /// Subscribe to the TMPMediatorManager.<br/>
        /// The passed in <see cref="TMP_Text"/> instance serves as the identifier of the <see cref="TMPMediator"/> instance,
        /// whereas the passed in object is added to the internal list of subscribers associated with this <see cref="TMP_Text"/> instance.<br/>
        /// If there is no <see cref="TMPMediator"/> instance associated with the <see cref="TMP_Text"/> instance, it will be created.<br/>
        /// The <see cref="TMPMediator"/> perists as long as the subscriber list of the <see cref="TMP_Text"/> instance is not empty.
        /// </summary>
        /// <param name="text">The identifier of the <see cref="TMPMediator"/> instance.</param>
        /// <param name="obj">The object to add to the subscriber list of the <see cref="TMPMediator"/> associated with <paramref name="text"/>.</param>
        public static void Subscribe(TMP_Text text, object obj)
        {
            List<object> list;
            if (!mediators.ContainsKey(text.gameObject))
            {
                TMPMediator mediator = new TMPMediator(text);
                list = new List<object>() { obj };
                mediators.Add(text.gameObject, new ValueTuple<TMPMediator, List<object>>(mediator, list));
            }
            else
            {
                list = mediators[text.gameObject].Item2;
                if (!list.Contains(obj))
                {
                    list.Add(obj);
                }
            }
        }

        /// <summary>
        /// Unsubscribe from the TMPMediatorManager.<br/>
        /// Will remove <paramref name="obj"/> from the subscriber list of the <see cref="TMPMediator"/> instance associated with the passed
        /// in <see cref="TMP_Text"/> instances.<br/>
        /// If <paramref name="obj"/> was the last subscriber to the <see cref="TMPMediator"/> instance, it will be removed from the manager.
        /// </summary>
        /// <param name="text">The identifier of the <see cref="TMPMediator"/> instance.</param>
        /// <param name="obj">The object to remove from the subscriber list of the <see cref="TMPMediator"/> associated with <paramref name="text"/>.</param>
        public static void Unsubscribe(TMP_Text text, object obj)
        {
            if (!mediators.TryGetValue(text.gameObject, out ValueTuple<TMPMediator, List<object>> tuple))
            {
                return;
            }

            tuple.Item2.Remove(obj);

            if (tuple.Item2.Count == 0)
            {
                mediators.Remove(text.gameObject);
                tuple.Item1.Dispose();
                //if (text.textPreprocessor == tuple.Item1.Processor)
                //    text.textPreprocessor = null;
            }
        }

        /// <summary>
        /// Get the <see cref="TMPMediator"/> instance associated with the passed in <see cref="TMP_Text"/> instance.
        /// </summary>
        /// <param name="text">The identifier of the <see cref="TMPMediator"/> instance.</param>
        /// <returns>The <see cref="TMPMediator"/> instance associated with the passed in <see cref="TMP_Text"/> instance.</returns>
        public static TMPMediator GetMediator(TMP_Text text) => mediators[text.gameObject].Item1;

        /// <summary>
        /// Attempt to get the <see cref="TMPMediator"/> instance associated with the passed in <see cref="TMP_Text"/> instance.
        /// </summary>
        /// <param name="text">The identifier of the <see cref="TMPMediator"/> instance.</param>
        /// <param name="mediator"></param>
        /// <returns>true if the <paramref name="go"/> has an associated <see cref="TMPMediator"/>; otherwise false.</returns>
        public static bool TryGetMediator(TMP_Text text, out TMPMediator mediator)
        {
            mediator = null;
            if (!mediators.TryGetValue(text.gameObject, out var tuple))
            {
                return false;
            }
            mediator = tuple.Item1;
            return true;
        }
    }
}