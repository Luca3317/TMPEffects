using System.Collections.Generic;
using TMPEffects;
using TMPEffects.TextProcessing;
using UnityEngine;
using TMPEffects.Components.Animator;
using static TMPEffects.TMPAnimations.AnimationUtility;
using TMPEffects.Components.CharacterData;

namespace TMPEffects.TMPAnimations.Animations
{
    [CreateAssetMenu(fileName = "new FadeAnimation", menuName = "TMPEffects/Animations/Fade")]
    public class FadeAnimation : TMPAnimation
    {
        [SerializeField] Vector2 anchor;
        [SerializeField] Vector2 direction;
        [SerializeField] float speed;
        [SerializeField] float minScale;
        [SerializeField] float maxScale;
        [SerializeField, Tooltip("How long to wait after reaching maximum scale")] float afterMaxWaitDuration;
        [SerializeField, Tooltip("How long to wait after reaching minimum scale")] float afterMinWaitDuration;

        [System.NonSerialized] float currentMinScale;
        [System.NonSerialized] float currentMaxScale;
        [System.NonSerialized] Vector2 currentAnchor;
        [System.NonSerialized] Vector2 currentDirection; // ignored if anchor not center
        [System.NonSerialized] float currentSpeed;
        [System.NonSerialized] float currentAfterMaxWaitDuration;
        [System.NonSerialized] float currentAfterMinWaitDuration;

        public override void Animate(ref CharData cData, IAnimationContext context)
        {
            float BL, TL, TR, BR;
            Context ctx = context as Context;
            FixVector(ref currentAnchor);
            if (ctx.lastUpdated == -1) { ctx.lastUpdated = ctx.animatorContext.passedTime; }

            if (cData.info.index == ctx.segmentData.firstAnimationIndex)
            {
                ctx.passed += context.animatorContext.deltaTime * currentSpeed;
            }
            float ppT = ctx.passed;
            float lerpT = Mathf.PingPong(ppT, 1);
            float repeat = Mathf.Repeat(ppT, 2) - 1;

            if (ctx.waitingSince == -1 && Mathf.Sign(repeat) != Mathf.Sign(ctx.lastRoc))
            {
                ctx.isMax = ctx.lastRoc < 0;
                BeginWaiting(in context, out ctx.waitingSince);
            }

            if (TryFinishWaiting(ctx.isMax ? currentAfterMaxWaitDuration : currentAfterMinWaitDuration, in context, ref ctx.waitingSince))
            {
                if (ctx.isMax) ctx.passed = 1;
                else ctx.passed = 0;

                ppT = ctx.passed;
                lerpT = Mathf.PingPong(ppT, 1);
                repeat = Mathf.Repeat(ppT, 2) - 1;
            }


            float t;

            if (ctx.waitingSince == -1)
            {
                t = Mathf.PingPong(ppT, 1);
            }
            else
            {
                t = ctx.isMax ? 1 : 0;
            }

            ctx.lastRoc = repeat;

            float multiplier = 10;
            if (currentAnchor == Vector2.zero)
            {
                TR = Mathf.Lerp(0, 1, t);
                BR = Mathf.Lerp(0, 1, t);
                TL = Mathf.Lerp(0, 1, t);
                BL = Mathf.Lerp(0, 1, t);
            }
            else if (currentAnchor == Vector2.right)
            {
                TR = Mathf.Lerp(0, 1, t * multiplier);
                BR = Mathf.Lerp(0, 1, t * multiplier);

                TL = Mathf.Lerp(0, 1, t);
                BL = Mathf.Lerp(0, 1, t);
            }
            else if (currentAnchor == Vector2.left)
            {
                TL = Mathf.Lerp(0, 1, t * multiplier);
                BL = Mathf.Lerp(0, 1, t * multiplier);

                TR = Mathf.Lerp(0, 1, t);
                BR = Mathf.Lerp(0, 1, t);
            }
            else if (currentAnchor == Vector2.up)
            {
                TL = Mathf.Lerp(0, 1, t * multiplier);
                TR = Mathf.Lerp(0, 1, t * multiplier);

                BR = Mathf.Lerp(0, 1, t);
                BL = Mathf.Lerp(0, 1, t);
            }
            else if (currentAnchor == Vector2.down)
            {
                BL = Mathf.Lerp(0, 1, t * multiplier);
                BR = Mathf.Lerp(0, 1, t * multiplier);

                TL = Mathf.Lerp(0, 1, t);
                TR = Mathf.Lerp(0, 1, t);
            }
            else if (currentAnchor == new Vector2(1, 1))
            {
                TR = Mathf.Lerp(0, 1, t * multiplier);

                TL = Mathf.Lerp(0, 1, t);
                BL = Mathf.Lerp(0, 1, t);
                BR = Mathf.Lerp(0, 1, t);
            }
            else if (currentAnchor == new Vector2(-1, 1))
            {
                TL = Mathf.Lerp(0, 1, t * multiplier);

                TR = Mathf.Lerp(0, 1, t);
                BL = Mathf.Lerp(0, 1, t);
                BR = Mathf.Lerp(0, 1, t);
            }
            else if (currentAnchor == new Vector2(1, -1))
            {
                BR = Mathf.Lerp(0, 1, t * multiplier);

                TL = Mathf.Lerp(0, 1, t);
                BL = Mathf.Lerp(0, 1, t);
                TR = Mathf.Lerp(0, 1, t);
            }
            else if (currentAnchor == new Vector2(-1, -1))
            {
                BL = Mathf.Lerp(0, 1, t * multiplier);

                TL = Mathf.Lerp(0, 1, t);
                TR = Mathf.Lerp(0, 1, t);
                BR = Mathf.Lerp(0, 1, t);
            }
            else throw new System.Exception();


            Color32 blcolor = cData.mesh.initial.GetColor(0);
            blcolor.a = (byte)(BL * blcolor.a);
            cData.mesh.SetColor(0, blcolor);

            Color32 tlcolor = cData.mesh.initial.GetColor(1);
            tlcolor.a = (byte)(TL * tlcolor.a);
            cData.mesh.SetColor(1, tlcolor);

            Color32 trcolor = cData.mesh.initial.GetColor(2);
            trcolor.a = (byte)(TR * trcolor.a);
            cData.mesh.SetColor(2, trcolor);

            Color32 brcolor = cData.mesh.initial.GetColor(3);
            brcolor.a = (byte)(BR * brcolor.a);
            cData.mesh.SetColor(3, brcolor);
        }

        void FixVector(ref Vector2 v)
        {
            if (v.x != 0)
            {
                if (v.x > 0) v.x = 1;
                else v.x = -1;
            }
            if (v.y != 0)
            {
                if (v.y > 0) v.y = 1;
                else v.y = -1;
            }
        }

        public override void ResetParameters()
        {
            currentAnchor = anchor;
            currentDirection = direction;
            currentSpeed = speed;
            currentMaxScale = maxScale;
            currentMinScale = minScale;
            currentAfterMaxWaitDuration = afterMaxWaitDuration;
            currentAfterMinWaitDuration = afterMinWaitDuration;
        }

        public override void SetParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return;

            foreach (KeyValuePair<string, string> kvp in parameters)
            {
                switch (kvp.Key)
                {
                    case "s":
                    case "sp":
                    case "speed": ParsingUtility.StringToFloat(kvp.Value, out currentSpeed); break;

                    case "dir":
                    case "direction":
                        switch (kvp.Value)
                        {
                            case "d":
                            case "diagonal": currentDirection = new Vector2(1, -1); break;
                            case "horizontal":
                            case "h":
                            case "right": currentDirection = new Vector2(1, 0); break;
                            case "left": currentDirection = new Vector2(-1, 0); break;
                            case "vertical":
                            case "v":
                            case "down": currentDirection = new Vector2(0, -1); break;
                            case "up": currentDirection = new Vector2(0, 1); break;
                            case "e":
                            case "exp":
                            case "expand": currentDirection = new Vector2(0, 0); break;


                            default: ParsingUtility.StringToVector2(kvp.Value, out currentDirection); break;
                        }
                        break;

                    case "a":
                    case "ac":
                    case "anchor":
                        switch (kvp.Value)
                        {
                            case "tl":
                            case "topleft": anchor = new Vector2(-1, 1); break;

                            case "tr":
                            case "topright": anchor = new Vector2(1, 1); break;

                            case "bl":
                            case "bottomleft": anchor = new Vector2(-1, -1); break;

                            case "br":
                            case "bottomright": anchor = new Vector2(1, -1); break;

                            case "c":
                            case "center": anchor = Vector2.zero; break;

                            case "l":
                            case "left": anchor = Vector2.left; break;

                            case "r":
                            case "right": anchor = Vector2.right; break;

                            case "t":
                            case "top": anchor = Vector2.up; break;

                            case "b":
                            case "bottom": anchor = Vector2.down; break;

                            default:
                                ParsingUtility.StringToVector2(kvp.Value, out currentAnchor);
                                break;
                        }
                        break;

                    case "mins":
                    case "minscale":
                        ParsingUtility.StringToFloat(kvp.Value, out currentMinScale); break;

                    case "maxs":
                    case "maxscale":
                        ParsingUtility.StringToFloat(kvp.Value, out currentMaxScale); break;

                    case "minw":
                    case "minwait":
                        ParsingUtility.StringToFloat(kvp.Value, out currentAfterMinWaitDuration); break;

                    case "maxw":
                    case "maxwait":
                        ParsingUtility.StringToFloat(kvp.Value, out currentAfterMaxWaitDuration); break;
                }
            }
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return true;

            foreach (KeyValuePair<string, string> kvp in parameters)
            {
                switch (kvp.Key)
                {
                    case "dir":
                    case "direction":
                        switch (kvp.Value)
                        {
                            case "diagonal":
                            case "horizontal":
                            case "right":
                            case "left":
                            case "vertical":
                            case "down":
                            case "up":
                            case "e":
                            case "exp":
                            case "expand":
                                break;

                            default:
                                if (!ParsingUtility.StringToVector2(kvp.Value, out _)) return false;
                                break;
                        }
                        break;


                    case "a":
                    case "ac":
                    case "anchor":
                        switch (kvp.Value)
                        {
                            case "tl":
                            case "topleft":
                            case "tr":
                            case "topright":
                            case "bl":
                            case "bottomleft":
                            case "br":
                            case "bottomright":
                            case "c":
                            case "center":
                            case "l":
                            case "left":
                            case "r":
                            case "right":
                            case "t":
                            case "top":
                            case "b":
                            case "bottom": break;

                            default:
                                if (!ParsingUtility.StringToVector2(kvp.Value, out _)) return false;
                                break;
                        }
                        break;

                    case "maxs":
                    case "maxscale":
                    case "mins":
                    case "minscale":
                    case "s":
                    case "sp":
                    case "speed":
                    case "minw":
                    case "minwait":
                    case "maxw":
                    case "maxwait":
                        if (!ParsingUtility.StringToFloat(kvp.Value, out _)) return false;
                        break;
                }
            }

            return true;
        }

        public override IAnimationContext GetNewContext()
        {
            return new Context() { lastUpdated = -1f, lastRoc = 0, waitingSince = -1, passed = 0 };
        }

        private class Context : IAnimationContext
        {
            public AnimatorContext animatorContext { get; set; }
            public SegmentData segmentData { get; set; }

            public float lastUpdated;
            public float lastRoc;
            public float waitingSince;
            public bool isMax;
            public float passed;
        }
    }
}

