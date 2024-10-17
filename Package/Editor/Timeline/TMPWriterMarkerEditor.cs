using UnityEditor.Timeline;
using UnityEngine.Timeline;

[CustomTimelineEditor(typeof(TMPStartWriterMarker))]
public class TMPWriterMarkerEditor : MarkerEditor
{
    public override MarkerDrawOptions GetMarkerOptions(IMarker marker)
    {
        MarkerDrawOptions s = new MarkerDrawOptions();
        s.tooltip = "Start Writer";
        return s;
    }
}         
