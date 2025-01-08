using System.Collections;
using System.Collections.Generic;
using TMPEffects.Timeline.Markers;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;

[CustomTimelineEditor(typeof(TMPStartAnimatingMarker))]
public class TMPStartAnimatingMarkerEditor : MarkerEditor
{
    public override MarkerDrawOptions GetMarkerOptions(IMarker marker)
    {
        MarkerDrawOptions s = new MarkerDrawOptions();
        s.tooltip = "Start Animating";
        return s;
    }
}

[CustomTimelineEditor(typeof(TMPStopAnimatingMarker))]
public class TMPStopAnimatingMarkerEditor : MarkerEditor
{
    public override MarkerDrawOptions GetMarkerOptions(IMarker marker)
    {
        MarkerDrawOptions s = new MarkerDrawOptions();
        s.tooltip = "Stop Animating";
        return s;
    }
}

[CustomTimelineEditor(typeof(TMPUpdateAnimationsMarker))]
public class TMPUpdateAnimationsMarkerEditor : MarkerEditor
{
    public override MarkerDrawOptions GetMarkerOptions(IMarker marker)
    {
        var mark = marker as TMPUpdateAnimationsMarker;
        MarkerDrawOptions s = new MarkerDrawOptions();
        s.tooltip = "Update Animations (" + mark.DeltaTime + ")";
        return s;
    }
}

[CustomTimelineEditor(typeof(TMPResetAnimationsMarker))]
public class TMPResetAnimationsMarkerEditor : MarkerEditor
{
    public override MarkerDrawOptions GetMarkerOptions(IMarker marker)
    {
        MarkerDrawOptions s = new MarkerDrawOptions();
        s.tooltip = "Reset Animations";
        return s;
    }
}

[CustomTimelineEditor(typeof(TMPResetTimeMarker))]
public class TMPResetTimeMarkerEditor : MarkerEditor
{
    public override MarkerDrawOptions GetMarkerOptions(IMarker marker)
    {
        var mark = marker as TMPResetTimeMarker;
        MarkerDrawOptions s = new MarkerDrawOptions();
        s.tooltip = "Reset Time (" + mark.Time + ")";
        return s;
    }
}

[CustomTimelineEditor(typeof(TMPSetUpdateFromMarker))]
public class TMPSetUpdateFromMarkerEditor : MarkerEditor
{
    public override MarkerDrawOptions GetMarkerOptions(IMarker marker)
    {
        var mark = marker as TMPSetUpdateFromMarker;
        MarkerDrawOptions s = new MarkerDrawOptions();
        s.tooltip = "Set UpdateFrom (" + mark.UpdateFrom + ")";
        return s;
    }
}