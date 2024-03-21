using System.Collections.Generic;
using TMPEffects.Components;
using TMPEffects.CharacterData;
using TMPEffects.TMPAnimations;
using TMPro;
using UnityEngine;

namespace TMPEffects.TMPSceneAnimations.ShowAnimations
{
    public class ScratchOffShowAnimation : TMPSceneShowAnimation
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
            Data d = context.customData as Data;

            if (d.state == null)
            {
                d.state = new Dictionary<int, Vector4>();
                for (int i = context.segmentData.firstAnimationIndex; i <= context.segmentData.lastAnimationIndex; i++)
                {
                    d.state[i] = Vector4.one * hiddenOpacity;
                }
            }

            int segmentIndex = context.segmentData.SegmentIndexOf(cData);
            Vector4 alphas = d.state[segmentIndex];

            UpdateAlphas(ref alphas, cData, context);

            cData.mesh.SetAlpha(0, alphas.x);
            cData.mesh.SetAlpha(1, alphas.y);
            cData.mesh.SetAlpha(2, alphas.z);
            cData.mesh.SetAlpha(3, alphas.w);

            d.state[segmentIndex] = alphas;

            if (alphas.x >= 255 && alphas.y >= 255f && alphas.z >= 255f && alphas.w >= 255f)
            {
                context.FinishAnimation(cData);
            }
        }

        private void UpdateAlphas(ref Vector4 alphas, CharData cData, IAnimationContext context)
        {
            if (!Input.GetMouseButton(0) || (Input.GetAxis("Mouse X") == 0 && Input.GetAxis("Mouse Y") == 0)) return;


            Data d = context.customData as Data;

            context.state.CalculateVertexPositions();

            Camera input = canvas == null ? null : (canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : cam);

            if (!TMP_TextUtilities.ScreenPointToWorldPointInRectangle(text.transform, Input.mousePosition, input, out Vector3 res))
            {
                Debug.LogError("Failed to calculate ScreenPointToWorldPointInRectangle");
            }

            for (int i = 0; i < 4; i++)
            {
                Vector3 vertex;

                switch (i)
                {
                    case 0: vertex = text.transform.TransformPoint(context.state.BL_Result); break;
                    case 1: vertex = text.transform.TransformPoint(context.state.TL_Result); break;
                    case 2: vertex = text.transform.TransformPoint(context.state.TR_Result); break;
                    case 3: vertex = text.transform.TransformPoint(context.state.BR_Result); break;
                    default: throw new System.Exception();
                }

                float magnitude = (res - vertex).magnitude;

                if (magnitude < d.radius)
                {
                    float t = magnitude / d.radius;
                    float t2 = 1 - d.fallOffCurve.Evaluate(t);
                    //Debug.LogWarning("Actuially doing it!; Adding " + t2 + " * " + context.animatorContext.DeltaTime + " * 2000 => " + (t2 * context.animatorContext.DeltaTime * 2000));

                    if (t2 < 0) Debug.LogWarning("NEGATIVE");
                    if (context.animatorContext.DeltaTime < 0) Debug.LogWarning("NEGATIVE");
                    alphas[i] = Mathf.Clamp(alphas[i] + t2 * context.animatorContext.DeltaTime * 400, 0f, 255f);
                }
            }
        }


        public override object GetNewCustomData()
        {
            return new Data()
            {
                state = null,
                radius = this.radius,
                fallOffCurve = this.fallOffCurve,
                hiddenOpacity = this.hiddenOpacity,
                shownOpacity = this.shownOpacity,
            };
        }

        public override void SetParameters(object customData, IDictionary<string, string> parameters)
        {

        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            return true;
        }

        private class Data
        {
            public Dictionary<int, Vector4> state;
            public float radius;
            public AnimationCurve fallOffCurve;
            public float hiddenOpacity;
            public float shownOpacity;
        }
    }
}
