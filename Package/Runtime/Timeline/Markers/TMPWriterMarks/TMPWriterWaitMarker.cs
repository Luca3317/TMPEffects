using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Timeline;

[CustomStyle("TMPWriterWaitMarkerStyle")]
[TrackBindingType(typeof(TMPWriterTrack))]
[DisplayName("TMPEffects Marker/TMPWriter/Wait")]
public class TMPWriterWaitMarker : TMPEffectsMarker
{
    public override PropertyName id => new PropertyName();

    public override NotificationFlags flags =>
        (retroactive ? NotificationFlags.Retroactive : default) |
        (triggerOnce ? NotificationFlags.TriggerOnce : default) |
        (triggerInEditMode ? NotificationFlags.TriggerInEditMode : default);

    [Space] [Tooltip("The amount of time the TMPWriter should wait before continuing to write.")]
    [SerializeField] private float waitTime = 0.5f;
    public float WaitTime => waitTime;

    private void OnValidate()
    {
        if (waitTime < 0) waitTime = 0;
    }

    // TODO Maybe supply some default class for implementing conditional waits
    // eg just some ConditionalClass { public bool Evaluate() { } }
    // [SerializeField] private WaitType  waitType = WaitType.Period;
    // [SerializeField] private UnityEvent<bool> waitAction;
    // public enum WaitType
    // {
    //     Condition,
    //     Period
    // }
}
