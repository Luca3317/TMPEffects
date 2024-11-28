using System.Collections;
using System.Collections.Generic;
using TMPEffects.Components;
using TMPEffects.Tags;
using TMPEffects.TMPCommands;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(TMPWriter))]
public class TMPWriterReceiver : MonoBehaviour, INotificationReceiver
{
    private TMPWriter writer;

    public void OnNotify(Playable origin, INotification notification, object context)
    {
        if (writer == null)
        {
            writer = GetComponent<TMPWriter>();
            if (writer == null) return;
        }

        switch (notification)
        {
            case TMPStartWriterMarker:
                writer.StartWriter();
                break;
            case TMPStopWriterMarker:
                writer.StopWriter();
                break;
            case TMPResetWriterMarker rwm:
                writer.ResetWriter(rwm.TextIndex);
                break;
            case TMPSkipWriterMarker swm:
                writer.SkipWriter(swm.SkipShowAnimation);
                break;
            case TMPRestartWriterMarker:
                writer.RestartWriter();
                break;


            case TMPWriterWaitMarker wwm:
                writer.Wait(wwm.WaitTime);
                break;
            case TMPWriterSetSkippableMarker wss:
                writer.SetSkippable(wss.Skippable);
                break;
            case TMPWriterResetWaitMarker:
                writer.ResetWaitPeriod();
                break;
        }
    }
}