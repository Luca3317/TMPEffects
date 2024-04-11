using System.Collections;
using System.Collections.Generic;
using TMPEffects.Components;
using TMPEffects.CharacterData;
using TMPEffects.TMPAnimations;
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
                d.offsets = new(context.SegmentData.length);
                for (int i = 0; i < context.SegmentData.length; i++)
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

                AnimationUtility.AddPositionDeltaRaw(d.offsets[segmentIndex], cData, context);
                return;
            }

            // If dragging something
            if (d.dragging != -1)
            {
                // If dragging another character, set and return
                if (d.dragging != cData.info.index)
                {
                    AnimationUtility.AddPositionDeltaRaw(d.offsets[segmentIndex], cData, context);
                    return; 
                }

                // If dragging this, update offset, set and return
                
                d.dynamicOffset = (Input.mousePosition - d.startPosition) / (canvas == null ? 1f : canvas.scaleFactor);
                AnimationUtility.AddPositionDeltaRaw(d.offsets[segmentIndex] + d.dynamicOffset, cData, context);
                return;
            }

            // Issue what if animation is updated only during e.g. fixedupdate
            if (Input.GetMouseButtonDown(0))
            {
                if (d.dragging != -1) Debug.LogError("how the fuck did this happen");

                float scaleFactor = (canvas == null ? 1f : canvas.scaleFactor);
                Camera input = canvas == null ? null : (canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : cam);
                int index = TMP_TextUtilities.FindIntersectingCharacter(text, Input.mousePosition - d.offsets[segmentIndex] * scaleFactor, input, true);

                if (index != cData.info.index)
                {
                    AnimationUtility.AddPositionDeltaRaw(d.offsets[segmentIndex], cData, context);
                    return;
                }

                d.dynamicOffset = Vector3.zero;
                d.dragging = cData.info.index;
                d.startPosition = Input.mousePosition;
                AnimationUtility.AddPositionDeltaRaw(d.offsets[segmentIndex], cData, context);

                return;
            }

            if (d.offsets[segmentIndex] != Vector3.zero)
                AnimationUtility.AddPositionDeltaRaw(d.offsets[segmentIndex], cData, context);
        }

        //public void OldAnimate(CharData cData, IAnimationContext context)
        //{
        //    Data d = context.customData as Data;

        //    // Initialize all offsets to 0
        //    if (d.offsets == null)
        //    {
        //        d.offsets = new(context.segmentData.length);
        //        for (int i = 0; i < context.segmentData.length; i++)
        //        {
        //            Debug.Log("initing " + (context.segmentData.startIndex + i));
        //            d.offsets.Add(context.segmentData.startIndex + i, Vector3.zero);
        //        }
        //    }

        //    // If mouse button not clicked, set and return
        //    if (!Input.GetMouseButton(0))
        //    {
        //        if (d.dragging != -1)
        //        {
        //            if (d.dragging == cData.info.index)
        //            {
        //                Debug.Log("Stopped dragging " + cData.info.index + " w/ dynamioffset " + d.dynamicOffset);
        //                d.offsets[segmentIndex] = d.offsets[segmentIndex] + d.dynamicOffset;
        //                d.dynamicOffset = Vector3.zero;

        //                Debug.Log("Set to " + cData.initialPosition + d.offsets[segmentIndex]);
        //            }

        //            d.dragging = -1;
        //        }

        //        AnimationUtility.AddPositionDeltaRaw(d.offsets[segmentIndex], cData, ref context);
        //        return;
        //    }

        //    // If dragging something
        //    if (d.dragging != -1)
        //    {
        //        // If dragging another character, set and return
        //        if (d.dragging != cData.info.index)
        //        {
        //            AnimationUtility.AddPositionDeltaRaw(d.offsets[segmentIndex], cData, ref context);
        //            return;
        //        }

        //        // If dragging this, update offset, set and return
        //        Canvas c = text.GetComponentInParent<Canvas>();
        //        d.dynamicOffset = (Input.mousePosition - d.startPosition) / c.scaleFactor;
        //        AnimationUtility.AddPositionDeltaRaw(d.offsets[segmentIndex] + d.dynamicOffset, cData, ref context);
        //        return;
        //    }

        //    // Issue what if animation is updated only during e.g. fixedupdate
        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        if (d.dragging != -1) Debug.LogError("how the fuck did this happen");

        //        Debug.Log("Click at " + Input.mousePosition + "; will check at " + (Input.mousePosition - d.offsets[segmentIndex]));

        //        int index = TMP_TextUtilities.FindIntersectingCharacter(text, Input.mousePosition - d.offsets[segmentIndex], null, true);

        //        if (index != cData.info.index)
        //        {
        //            AnimationUtility.AddPositionDeltaRaw(d.offsets[segmentIndex], cData, ref context);
        //            return;
        //        }

        //        d.dragging = cData.info.index;
        //        d.startPosition = Input.mousePosition;
        //        AnimationUtility.AddPositionDeltaRaw(d.offsets[segmentIndex], cData, ref context);

        //        Debug.Log("Start: Offset of " + index + " is " + d.offsets[segmentIndex]);

        //        return;
        //    }

        //    if (d.offsets[segmentIndex] != Vector3.zero)
        //        AnimationUtility.AddPositionDeltaRaw(d.offsets[segmentIndex], cData, ref context);
        //}



        public override void SetParameters(object customData, IDictionary<string, string> parameters)
        {
            return;
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
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

