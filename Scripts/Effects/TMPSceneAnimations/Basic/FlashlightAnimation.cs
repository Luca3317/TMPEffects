using System.Collections;
using System.Collections.Generic;
using TMPEffects.Components;
using TMPEffects.Components.CharacterData;
using TMPEffects.TMPAnimations;
using TMPro;
using UnityEngine;
using static TMPEffects.ParameterUtility;

namespace TMPEffects.TMPSceneAnimations.Animations
{
    public class FlashlightAnimation : TMPSceneAnimation
    {
        [SerializeField] Camera cam;
        [SerializeField] TMP_Text text;

        [SerializeField] float hiddenOpacity;
        [SerializeField] float shownOpacity;
        [SerializeField] AnimationCurve fallOffCurve;
        [SerializeField] float radius;

        private Canvas canvas;

        private void OnEnable()
        {
            if (text == null)
            {
                text = GetComponentInParent<TMP_Text>();
                if (text == null)
                {
                    Debug.LogError("Could not find TMP_Text component on gameobject or its parent; Please either put this component on a gameobject with a TMP_Text component in its ancestors or manually assign a component.");
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

        public override void Animate(CharData cData, IAnimationContext context)
        {
            if (!Application.isPlaying) return;

            Data d = context.customData as Data;

            Camera input = canvas == null ? null : (canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : cam);

            bool succ = TMP_TextUtilities.ScreenPointToWorldPointInRectangle(text.transform, Input.mousePosition, input, out Vector3 res);

            if (!succ) Debug.LogWarning("Fuckup");

            for (int i = 0; i < 4; i++)
            {
                Vector3 vertex = text.transform.TransformPoint(cData.mesh.initial.GetVertex(i));
                float magnitude = (res - vertex).magnitude;

                Color32 color = cData.mesh.initial.GetColor(i);

                if (magnitude < d.radius)
                {
                    float t = magnitude / d.radius;
                    float t2 = d.fallOffCurve.Evaluate(t + 1);
                    float opacity = Mathf.LerpUnclamped(d.hiddenOpacity, d.shownOpacity, t2);

                    color.a = (byte)opacity;
                    cData.mesh.SetColor(i, color);
                }
                else
                {
                    color.a = (byte)0;
                    cData.mesh.SetColor(i, color);
                }
            }
        }

        public override void SetParameters(object customData, IDictionary<string, string> parameters)
        {
            if (parameters == null) return;

            Data d = customData as Data;
            if (TryGetFloatParameter(out float f, parameters, "hiddenOpacity", "hOpacity", "hOp", "hidden")) d.hiddenOpacity = f;
            if (TryGetFloatParameter(out f, parameters, "shownOpacity", "sOpacity", "sOp", "shown")) d.shownOpacity = f;
            if (TryGetFloatParameter(out f, parameters, "radius", "rad", "r")) d.radius = f;
            if (TryGetAnimCurveParameter(out var crv, parameters, "fallOffCurve", "foCurve", "foCrv")) d.fallOffCurve = crv;
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return true;

            if (HasNonFloatParameter(parameters, "hiddenOpacity", "hOpacity", "hOp", "hidden")) return false;
            if (HasNonFloatParameter(parameters, "shownOpacity", "sOpacity", "sOp", "shown")) return false;
            if (HasNonFloatParameter(parameters, "radius", "rad", "r")) return false;
            if (HasNonAnimCurveParameter(parameters, "fallOffCurve", "foCurve", "foCrv")) return false;
            return true;
        }

        public override object GetNewCustomData()
        {
            return new Data()
            {
                hiddenOpacity = this.hiddenOpacity,
                shownOpacity = this.shownOpacity,
                fallOffCurve = this.fallOffCurve,
                radius = this.radius
            };
        }

        private class Data
        {
            public float hiddenOpacity;
            public float shownOpacity;
            public AnimationCurve fallOffCurve;
            public float radius;
        }
    }
}
