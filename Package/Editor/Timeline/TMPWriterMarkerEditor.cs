using TMPEffects.Timeline.Markers;
using UnityEditor.Timeline;
using UnityEngine.Timeline;

[CustomTimelineEditor(typeof(TMPStartWriterMarker))]
public class TMPStartWriterMarkerEditor : MarkerEditor
{
    public override MarkerDrawOptions GetMarkerOptions(IMarker marker)
    {
        MarkerDrawOptions s = new MarkerDrawOptions();
        s.tooltip = "Start Writer";
        return s;
    }
}

[CustomTimelineEditor(typeof(TMPStopWriterMarker))]
public class TMPStopWriterMarkerEditor : MarkerEditor
{
    public override MarkerDrawOptions GetMarkerOptions(IMarker marker)
    {
        MarkerDrawOptions s = new MarkerDrawOptions();
        s.tooltip = "Stop Writer";
        return s;
    }
}

[CustomTimelineEditor(typeof(TMPRestartWriterMarker))]
public class TMPRestartWriterMarkerEditor : MarkerEditor
{
    public override MarkerDrawOptions GetMarkerOptions(IMarker marker)
    {
        MarkerDrawOptions s = new MarkerDrawOptions();
        s.tooltip = "Restart Writer";
        return s;
    }
}

[CustomTimelineEditor(typeof(TMPResetWriterMarker))]
public class TMPResetWriterMarkerEditor : MarkerEditor
{
    public override MarkerDrawOptions GetMarkerOptions(IMarker marker)
    {
        var mark = marker as TMPResetWriterMarker;
        MarkerDrawOptions s = new MarkerDrawOptions();
        s.tooltip = "Reset Writer (" + mark.TextIndex + ")";
        return s;
    }
}

[CustomTimelineEditor(typeof(TMPWriterWaitMarker))]
public class TMPWriterWaitMarkerEditor : MarkerEditor
{
    public override MarkerDrawOptions GetMarkerOptions(IMarker marker)
    {
        var mark = marker as TMPWriterWaitMarker;
        MarkerDrawOptions s = new MarkerDrawOptions();
        s.tooltip = "Wait (" + mark.WaitTime + ")";
        return s;
    }
}

[CustomTimelineEditor(typeof(TMPWriterResetWaitMarker))]
public class TMPWriterResetWaitMarkerEditor : MarkerEditor
{
    public override MarkerDrawOptions GetMarkerOptions(IMarker marker)
    {
        MarkerDrawOptions s = new MarkerDrawOptions();
        s.tooltip = "Reset Wait";
        return s;
    }
}

[CustomTimelineEditor(typeof(TMPSkipWriterMarker))]
public class TMPSkipWriterMarkerEditor : MarkerEditor
{
    public override MarkerDrawOptions GetMarkerOptions(IMarker marker)
    {
        MarkerDrawOptions s = new MarkerDrawOptions();
        s.tooltip = "Skip Writer";  
        return s;
    }
}

[CustomTimelineEditor(typeof(TMPWriterSetSkippableMarker))]
public class TMPSetSkippableMarkerEditor : MarkerEditor
{
    public override MarkerDrawOptions GetMarkerOptions(IMarker marker)
    {
        var mark = marker as TMPWriterSetSkippableMarker;
        MarkerDrawOptions s = new MarkerDrawOptions();
        s.tooltip = "Set Skippable ("+ mark.Skippable +")";  
        return s;
    }
}