using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "new SpeedCommand", menuName = "TMPEffects/Commands/Speed")]
public class SpeedCommand : TMPCommand
{
    public override CommandType CommandType => CommandType.Index;
    public override bool ExecuteInstantly => false;

    public override void ExecuteCommand(TMPCommandTag args, TMPWriter writer)
    {
        writer.SetSpeed(float.Parse(args.parameters[""], CultureInfo.InvariantCulture));
    }

    public override bool ValidateParameters(Dictionary<string, string> parameters)
    {
        if (parameters == null) return false;
        if (!parameters.ContainsKey(""))
            return false;

        return float.TryParse(parameters[""], NumberStyles.Float, CultureInfo.InvariantCulture, out _);
    }

#if UNITY_EDITOR
    public override void SceneGUI(Vector3 position, TMPCommandTag tag)
    {
        if (tag.parameters == null || !tag.parameters.ContainsKey(""))
        {
        }
        else
        {
            Handles.Label(position, new GUIContent("Speed set to " + tag.parameters[""]));
            Handles.SphereHandleCap(0, position, Quaternion.identity, 15, EventType.Repaint);
        }
    }
#endif
}
