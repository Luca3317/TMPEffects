using System.Collections.Generic;
using TMPEffects;
using TMPEffects.TextProcessing;
using UnityEngine;
using static EffectUtility;

// TOOD Make segmentlength independent
[CreateAssetMenu(fileName = "new JumpAnimation", menuName = "TMPEffects/Animations/Jump")]
public class JumpAnimation : TMPAnimation
{
    [SerializeField] float speed = 1;
    [SerializeField] float amplitude = 1;
    [SerializeField] bool uniform;
    [SerializeField] float frequency = 1;
    [SerializeField] float waitDuration = 1;
    [SerializeField] float radius = 1;

    [System.NonSerialized] float currentSpeed;
    [System.NonSerialized] float currentAmplitude;
    [System.NonSerialized] bool currentUniform;
    [System.NonSerialized] float currentFrequency;
    [System.NonSerialized] float currentWaitDuration;
    [System.NonSerialized] float currentRadius;

    public override void Animate(ref CharData cData, IAnimationContext context)
    {
        JumpContext ctx = (JumpContext)context;

        // Initialize playingSince
        if (ctx.playingSince == -1)
        {
            ctx.playingSince = GetTime(ctx);
        }

        // If waiting, and done
        if (ctx.waitingSince != -1 && (context.animatorContext.useScaledTime ? Time.time : Time.unscaledTime) - ctx.waitingSince >= currentWaitDuration)
        {
            ctx.waitingSince = -1;
            ctx.playingSince = GetTime(ctx);
        }

        // If not waiting
        if (ctx.waitingSince == -1)
        {
            ctx.index = Mathf.Lerp(-currentRadius, context.segmentData.length - 1 + currentRadius, (((GetTime(ctx) - ctx.playingSince) * currentSpeed) /*% cData.segmentLength*/) / context.segmentData.length);
            //ctx.index = Mathf.Lerp(-1, cData.segmentLength, ((context.AnimatorContext.passedTime * currentSpeed) % cData.segmentLength) / cData.segmentLength);
            int newIndexInt = (int)ctx.index;

            if (newIndexInt == context.segmentData.length - 1 + currentRadius)
            {
                // begin wait period
                ctx.waitingSince = GetTime(ctx);
                return;
            }

            if (Mathf.Abs(cData.segmentIndex - ctx.index) <= currentRadius/*1*/)
            {
                float diff = (float)cData.segmentIndex - ctx.index;
                float yOffset;

                if (diff > 0)
                {
                    yOffset = Mathf.SmoothStep(currentAmplitude, 0, diff / currentRadius);
                }
                else
                {
                    yOffset = Mathf.SmoothStep(currentAmplitude, 0, Mathf.Abs(diff) / currentRadius);
                }

                cData.SetPosition(cData.info.initialPosition + Vector3.up * yOffset);
            }
        }
    }

    public override void ResetParameters()
    {
        currentFrequency = frequency;
        currentAmplitude = amplitude;
        currentUniform = uniform;
        currentSpeed = speed;
        currentRadius = radius;
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

                case "f":
                case "fq":
                case "frequency": ParsingUtility.StringToFloat(kvp.Value, out currentFrequency); break;

                case "a":
                case "amp":
                case "amplitude": ParsingUtility.StringToFloat(kvp.Value, out currentAmplitude); break;

                case "u":
                case "uniform": ParsingUtility.StringToBool(kvp.Value, out currentUniform); break;

                case "w":
                case "wait": ParsingUtility.StringToFloat(kvp.Value, out currentWaitDuration); break;

                case "r":
                case "radius": ParsingUtility.StringToFloat(kvp.Value, out currentRadius); break;
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
                    if (!ParsingUtility.StringToFloat(kvp.Value, out _)) return false;
                    break;

                case "f":
                case "fq":
                case "frequency":
                    if (!ParsingUtility.StringToFloat(kvp.Value, out _)) return false;
                    break;

                case "a":
                case "amp":
                case "amplitude":
                    if (!ParsingUtility.StringToFloat(kvp.Value, out _)) return false;
                    break;

                case "u":
                case "uniform":
                    if (!ParsingUtility.StringToBool(kvp.Value, out _)) return false;
                    break;


                case "w":
                case "wait":
                    if (!ParsingUtility.StringToFloat(kvp.Value, out _)) return false;
                    break;

                case "r":
                case "radius":
                    if (!ParsingUtility.StringToFloat(kvp.Value, out _)) return false;
                    break;
            }
        }

        return true;
    }

    public override IAnimationContext GetNewContext()
    {
        return new JumpContext() { waitingSince = -1f, playingSince = -1f };
    }

    private class JumpContext : IAnimationContext
    {
        private AnimatorContext context;
        public AnimatorContext animatorContext { get => context; set => context = value; }

        public SegmentData segmentData { get; set; }

        public float waitingSince;
        public float playingSince;

        public float index;
    }
}
