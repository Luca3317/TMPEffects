using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public abstract class TMPEffectsMarker : Marker, INotification, INotificationOptionProvider
{
    [SerializeField] protected bool retroactive = false;
    [SerializeField] protected bool triggerOnce = false;
    [SerializeField] protected bool triggerInEditMode = false;

    public abstract PropertyName id { get; }
    public abstract NotificationFlags flags { get; }
}
  