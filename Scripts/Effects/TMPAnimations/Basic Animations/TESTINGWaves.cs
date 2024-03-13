using System.Collections;
using System.Collections.Generic;
using TMPEffects.Components.CharacterData;
using TMPEffects.Extensions;
using UnityEngine;
using static TMPEffects.TMPAnimations.AnimationUtility;

namespace TMPEffects.TMPAnimations.Animations
{
    [CreateAssetMenu(fileName = "lol", menuName = "TEST")]
    public class TESTINGWaves : TMPAnimation
    {
        //[SerializeField] Wave wave;
        [SerializeField] ParameterUtility.WaveType waveType;
        //[SerializeField] float interval;
        //[SerializeField] float wavelength;
        //[SerializeField] float amplitude;
        //[SerializeField] float velocity;
        [SerializeField] AnimationCurve upCurve = AnimationCurveUtility.EaseOutElastic();
        [SerializeField] AnimationCurve downCurve = AnimationCurveUtility.EaseOutElastic();

        [SerializeField] float upPeriod;
        [SerializeField] float downPeriod;
        [SerializeField] float velocity;
        [SerializeField] float amplitude;
        [SerializeField] float pulseInterval;
        [SerializeField] bool useIndex = false;
        [SerializeField] bool realtimeinterval = false;
        [SerializeField] float crestWait = 1f;
        [SerializeField] float throughWait = 1f;


        Wave w = null;
        public override void Animate(CharData cData, IAnimationContext context)
        {
            if (w == null)
            {
                w = new Wave(upCurve, downCurve, upPeriod, downPeriod, velocity, amplitude, crestWait, throughWait);
            }


            // Xpos is roughly approximated to look similar to index based T;
            // Couldnt figure out the exact logic behind referenceScale yet
            // so will stay rough approximation for now
            float xPos = cData.info.initialPosition.x;
            xPos /= (cData.info.referenceScale / 36f);
            xPos /= 2000f;

            //if (context.segmentData.SegmentIndexOf(cData) == 0)
            //{
            //    int val;
            //    if (useIndex)
            //        val = w.PassedExtrema(context.animatorContext.PassedTime, context.animatorContext.DeltaTime, context.segmentData.SegmentIndexOf(cData), realtimeinterval);
            //    else
            //        val = w.PassedExtrema(context.animatorContext.PassedTime, context.animatorContext.DeltaTime, xPos, realtimeinterval);

            //    if (val == -1)
            //    {
            //        Debug.Log("MINIMA");
            //    }
            //    if (val == 1)
            //    {
            //        Debug.Log("MAXIMA");
            //    }
            //}

            float eval;
            if (useIndex)
                eval = w.Evaluate(context.animatorContext.PassedTime, context.segmentData.SegmentIndexOf(cData), realtimeinterval);
            else 
                eval = w.Evaluate(context.animatorContext.PassedTime, xPos, realtimeinterval);

            cData.SetPosition(cData.info.initialPosition + eval * Vector3.up * 25);
        }

        public override object GetNewCustomData()
        {
            return null;
        } 

        public override void SetParameters(object customData, IDictionary<string, string> parameters)
        {
            w = null;
            return;
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            return true;
        }
    }

}
