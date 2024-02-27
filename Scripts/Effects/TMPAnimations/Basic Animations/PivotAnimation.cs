using System.Collections.Generic;
using TMPEffects.TextProcessing;
using UnityEngine;
using TMPEffects.Components.CharacterData;

namespace TMPEffects.TMPAnimations.Animations
{
    [CreateAssetMenu(fileName = "new PivotAnimation", menuName = "TMPEffects/Animations/Pivot")]
    public class PivotAnimation : TMPAnimation
    {
        [SerializeField] float speed;
        [SerializeField] float xOffset;
        [SerializeField] float yOffset;
        [SerializeField] float minAngle;
        [SerializeField] float maxAngle;
        [SerializeField] Vector3 rotationAxis;

        [System.NonSerialized] float currentSpeed;
        [System.NonSerialized] float currentXOffset;
        [System.NonSerialized] float currentYOffset;
        [System.NonSerialized] float currentMaxAngle;
        [System.NonSerialized] float currentMinAngle;
        [System.NonSerialized] Vector3 currentRotationAxis;

        public override void Animate(ref CharData cData, IAnimationContext context)
        {
            float angle = (context.animatorContext.PassedTime * currentSpeed) % 360;
            var rotate = Matrix4x4.Rotate(Quaternion.FromToRotation(Vector3.right, (cData.mesh.initial.GetPosition(3) - cData.mesh.initial.GetPosition(0)).normalized));
            cData.SetRotation(Quaternion.AngleAxis(angle, rotate.MultiplyPoint3x4(currentRotationAxis)));
            cData.SetPivot(cData.info.initialPosition + new Vector3(currentXOffset, currentYOffset, 0f));
        }

        public override void ResetParameters()
        {
            currentSpeed = speed;
            currentYOffset = yOffset;
            currentXOffset = xOffset;
            currentMaxAngle = maxAngle;
            currentMinAngle = minAngle;
            currentRotationAxis = rotationAxis;
        }

        public override void SetParameters(IDictionary<string, string> parameters)
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

                    case "axis":
                        ParsingUtility.StringToVector3(kvp.Value, out currentRotationAxis);
                        break;
                }
            }

        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
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
                        if (!ParsingUtility.StringToFloat(kvp.Value, out _)) return false;
                        break;

                    case "axis":
                        if (!ParsingUtility.StringToVector3(kvp.Value, out _)) return false;
                        break;

                }
            }

            return true;
        }
    }
}


