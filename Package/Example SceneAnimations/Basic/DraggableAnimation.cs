using System.Collections.Generic;
using TMPEffects.CharacterData;
using TMPEffects.Databases;
using TMPEffects.TMPAnimations;
using TMPEffects.TMPAnimations.Animations;
using TMPro;
using UnityEngine;

namespace TMPEffects.TMPSceneAnimations
{
    public class Draggable : TMPSceneAnimation
    {
        [SerializeField] Camera cam;
        [SerializeField] TMP_Text text;

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

            Data d = context.CustomData as Data;
            int segmentIndex = context.SegmentData.SegmentIndexOf(cData);

            // Initialize all offsets to 0
            if (d.offsets == null)
            {
                d.offsets = new(context.SegmentData.Length);
                for (int i = 0; i < context.SegmentData.Length; i++)
                {
                    d.offsets.Add(i, Vector3.zero);
                }
            }

            // If mouse button not clicked, set and return
            if (!Input.GetMouseButton(0))
            {
                if (d.dragging != -1)
                {
                    if (d.dragging == cData.info.index)
                    {
                        d.offsets[segmentIndex] = d.offsets[segmentIndex] + d.dynamicOffset;
                        d.dynamicOffset = Vector3.zero;
                        d.dragging = -1;
                    }
                }

                cData.PositionDelta += 
                    TMPAnimationUtility.IgnoreScaling(d.offsets[segmentIndex], cData, context);
                return;
            }

            // If dragging something
            if (d.dragging != -1)
            {
                // If dragging another character, set and return
                if (d.dragging != cData.info.index)
                {
                    cData.PositionDelta += 
                        TMPAnimationUtility.IgnoreScaling(d.offsets[segmentIndex], cData, context);
                    // AnimationUtility.AddPositionDeltaRaw(d.offsets[segmentIndex], cData, context);
                    return; 
                }

                // If dragging this, update offset, set and return
                
                d.dynamicOffset = (Input.mousePosition - d.startPosition) / (canvas == null ? 1f : canvas.scaleFactor);
                cData.PositionDelta += 
                    TMPAnimationUtility.IgnoreScaling(d.offsets[segmentIndex], cData, context);
                // AnimationUtility.AddPositionDeltaRaw(d.offsets[segmentIndex] + d.dynamicOffset, cData, context);
                return;
            }

            // Issue what if animation is updated only during e.g. fixedupdate
            if (Input.GetMouseButtonDown(0))
            {
                if (d.dragging != -1) Debug.LogError("how the fuck did this happen");

                float scaleFactor = (canvas == null ? 1f : canvas.scaleFactor);
                Camera input = canvas == null ? null : (canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : cam);
                int index = TMP_TextUtilities.FindIntersectingCharacter(text, Input.mousePosition - (d.offsets[segmentIndex] * scaleFactor), input, true);

                if (index != cData.info.index)
                {
                    cData.PositionDelta += 
                        TMPAnimationUtility.IgnoreScaling(d.offsets[segmentIndex], cData, context);
                    // AnimationUtility.AddPositionDeltaRaw(d.offsets[segmentIndex], cData, context);
                    return;
                }

                d.dynamicOffset = Vector3.zero;
                d.dragging = cData.info.index;
                d.startPosition = Input.mousePosition;
                cData.PositionDelta += 
                    TMPAnimationUtility.IgnoreScaling(d.offsets[segmentIndex], cData, context);
                // AnimationUtility.AddPositionDeltaRaw(d.offsets[segmentIndex], cData, context);

                return;
            }

            if (d.offsets[segmentIndex] != Vector3.zero)
                cData.PositionDelta += 
                    TMPAnimationUtility.IgnoreScaling(d.offsets[segmentIndex], cData, context);
                // AnimationUtility.AddPositionDeltaRaw(d.offsets[segmentIndex], cData, context);
        }

        public override void SetParameters(object customData, IDictionary<string, string> parameters,
            ITMPKeywordDatabase keywordDatabase)
        {
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters, ITMPKeywordDatabase keywordDatabase)
        {
            return true;
        }

        public override object GetNewCustomData()
        {
            return new Data();
        }

        private class Data
        {
            public Vector3 dynamicOffset;
            public Dictionary<int, Vector3> offsets = null;
            public int dragging = -1;
            public Vector3 startPosition;
        }
    }
}

