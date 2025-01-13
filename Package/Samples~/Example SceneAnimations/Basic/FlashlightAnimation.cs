using System.Collections.Generic;
using TMPEffects.AutoParameters.Attributes;
using TMPEffects.CharacterData;
using TMPEffects.Extensions;
using TMPEffects.TMPAnimations;
using TMPEffects.TMPAnimations.Animations;
using TMPro;
using UnityEngine;
using static TMPEffects.Parameters.TMPParameterUtility;

namespace TMPEffects.TMPSceneAnimations.Animations
{
    [AutoParameters]
    public partial class FlashlightAnimation : TMPSceneAnimation
    {
        [SerializeField] Camera cam;
        [SerializeField] TMP_Text text;

        [AutoParameter("hiddenopacity"), SerializeField]
        float hiddenOpacity = 0;

        [AutoParameter("shownopacity"), SerializeField]
        float shownOpacity = 255;

        [AutoParameter("falloffcurve"), SerializeField]
        AnimationCurve fallOffCurve = AnimationCurveUtility.Linear();

        [AutoParameter("radius"), SerializeField]
        float radius = 50;

        private Canvas canvas;

        private void OnEnable()
        {
            if (text == null)
            {
                text = GetComponentInParent<TMP_Text>();
                if (text == null)
                {
                    Debug.LogError(
                        "Could not find TMP_Text component on gameobject or its parent; Please either put this component on a gameobject with a TMP_Text component in its ancestors or manually assign a component.");
                }
            }

            if (text is TextMeshProUGUI)
            {
                canvas = text.GetComponentInParent<Canvas>();
                if (canvas == null)
                {
                    Debug.LogError("Could not find Canvas component on gameobject of TMP_Text or its parents");
                }
            }

            if (cam == null)
            {
                cam = Camera.main;
                if (cam == null)
                {
                    Debug.LogError("Could not find Camera component");
                }
            }
        }

        // Your animation logic goes here
        private partial void Animate(CharData cData, AutoParametersData data, IAnimationContext context)
        {
            // Dont want to show this one in preview
            if (!Application.isPlaying)
            {
                return;
            }

            Camera input = canvas == null ? null : (canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : cam);

            if (!TMP_TextUtilities.ScreenPointToWorldPointInRectangle(text.transform, Input.mousePosition, input,
                    out Vector3 res))
            {
                Debug.LogError("Failed to calculate ScreenPointToWorldPointInRectangle");
            }

            context.AnimatorContext.Modifiers.CalculateVertexPositions(cData, context.AnimatorContext);
            context.AnimatorContext.Modifiers.CalculateVertexColors(cData, context.AnimatorContext);
            var rad = TMPAnimationUtility.ScaleVector(new Vector3(data.radius, 0, 0), cData, context).x;
            for (int i = 0; i < 4; i++)
            {
                Vector3 vertex;

                vertex = text.transform.TransformPoint(context.AnimatorContext.Modifiers.VertexPosition(i));

                float magnitude = (res - vertex).magnitude;

                Color32 color = context.AnimatorContext.Modifiers.VertexColor(i);

                if (magnitude < rad)
                {
                    float t = magnitude / rad;
                    float t2 = 1 - data.fallOffCurve.Evaluate(t);
                    float opacity = Mathf.LerpUnclamped(data.hiddenOpacity, data.shownOpacity, t2);

                    color.a = (byte)opacity;
                    cData.mesh.SetAlpha(i, (byte)opacity);
                    // cData.mesh.SetColor(i, color);
                }
                else
                {
                    color.a = (byte)0;
                    // cData.mesh.SetColor(i, color);
                    cData.mesh.SetAlpha(i, 0);
                }
            }
        }
    }
}