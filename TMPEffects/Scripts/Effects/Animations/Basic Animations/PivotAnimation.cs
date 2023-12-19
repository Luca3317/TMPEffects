using System.Collections.Generic;
using TMPEffects;
using TMPEffects.TextProcessing;
using UnityEngine;

[CreateAssetMenu(fileName = "new PivotAnimation", menuName = "TMPEffects/Animations/Pivot")]
public class PivotAnimation : TMPAnimation
{
    [SerializeField] float speed;
    [SerializeField] float xOffset;
    [SerializeField] float yOffset;
    [SerializeField] float minAngle;
    [SerializeField] float maxAngle;

    [System.NonSerialized] float currentSpeed;
    [System.NonSerialized] float currentXOffset;
    [System.NonSerialized] float currentYOffset;
    [System.NonSerialized] float currentMaxAngle;
    [System.NonSerialized] float currentMinAngle;
    [System.NonSerialized] string mode;

    public override void Animate(ref CharData cData, ref IAnimationContext context)
    {
        float angle = (context.animatorContext.passedTime * currentSpeed) % 360;
        switch (mode)
        {
            case "z":
                cData.SetRotation(Quaternion.AngleAxis(angle, Vector3.forward));
                cData.SetPivot(cData.info.initialPosition + new Vector3(currentXOffset, currentYOffset, 0f));
                //cData.SetRotation(Quaternion.AngleAxis(angle, Vector3.forward), cData.info.initialPosition + new Vector3(currentXOffset, currentYOffset, 0f));
                break;
            case "-z":
                cData.SetRotation(Quaternion.AngleAxis(-angle, Vector3.forward));
                cData.SetPivot(cData.info.initialPosition + new Vector3(currentXOffset, currentYOffset, 0f));
                //cData.SetRotation(Quaternion.AngleAxis(-angle, Vector3.forward), cData.info.initialPosition + new Vector3(currentXOffset, currentYOffset, 0f));
                break;
            case "x":
                cData.SetRotation(Quaternion.AngleAxis(angle, Vector3.right));
                cData.SetPivot(cData.info.initialPosition + new Vector3(currentXOffset, currentYOffset, 0f));
                //cData.SetRotation(Quaternion.AngleAxis(angle, Vector3.right), cData.info.initialPosition + new Vector3(currentXOffset, currentYOffset, 0f));
                break;
            case "y":
                cData.SetRotation(Quaternion.AngleAxis(angle, Vector3.up));
                cData.SetPivot(cData.info.initialPosition + new Vector3(currentXOffset, currentYOffset, 0f));
                //cData.SetRotation(Quaternion.AngleAxis(angle, Vector3.up), cData.info.initialPosition + new Vector3(currentXOffset, currentYOffset, 0f));
                break;
        }
    }

    public override void ResetParameters()
    {
        currentSpeed = speed;
        currentYOffset = yOffset;
        currentXOffset = xOffset;
        currentMaxAngle = maxAngle;
        currentMinAngle = minAngle;
    }

    public override void SetParameters(Dictionary<string, string> parameters)
    {
        if (parameters == null) return;

        foreach (var kvp in parameters)
        {
            switch (kvp.Key)
            {
                case "s":
                case "sp":
                case "speed": ParsingUtility.StringToFloat(kvp.Value, out currentSpeed); break;

                case "x":
                case "xoffset": ParsingUtility.StringToFloat(kvp.Value, out currentXOffset); break;

                case "y":
                case "yoffset": ParsingUtility.StringToFloat(kvp.Value, out currentYOffset); break;

                case "min":
                case "minangle": ParsingUtility.StringToFloat(kvp.Value, out currentMinAngle); break;

                case "max":
                case "maxangle": ParsingUtility.StringToFloat(kvp.Value, out currentMaxAngle); break;

                case "o":
                case "offset":
                    ParsingUtility.StringToFloat(kvp.Value, out currentXOffset);
                    ParsingUtility.StringToFloat(kvp.Value, out currentYOffset);
                    break;

                case "mode":
                    mode = kvp.Value;
                    break;
            }
        }

    }

    public override bool ValidateParameters(Dictionary<string, string> parameters)
    {
        if (parameters == null) return true;

        foreach (var kvp in parameters)
        {
            switch (kvp.Key)
            {
                case "s":
                case "sp":
                case "speed":
                case "x":
                case "xoffset":
                case "y":
                case "yoffset":
                case "min":
                case "minangle":
                case "max":
                case "maxangle":
                case "o":
                case "offset":
                    ParsingUtility.StringToFloat(kvp.Value, out _);
                    break;
            }
        }

        return true;
    }
}
