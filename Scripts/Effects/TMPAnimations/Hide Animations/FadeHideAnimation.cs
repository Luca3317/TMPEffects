using System.Collections.Generic;
using TMPEffects.TextProcessing;
using UnityEngine;
using TMPEffects.Components.Animator;
using TMPEffects.Components.CharacterData;

namespace TMPEffects.TMPAnimations.HideAnimations
{
    [CreateAssetMenu(fileName = "new FadeHideAnimation", menuName = "TMPEffects/Hide Animations/Fade")]
    public class FadeHideAnimation : TMPHideAnimation
    {
        [SerializeField] Vector2 anchor;
        [SerializeField] float speed;
        [SerializeField] float minScale;
        [SerializeField] float maxScale;

        [System.NonSerialized] float currentMinScale;
        [System.NonSerialized] float currentMaxScale;
        [System.NonSerialized] Vector2 currentAnchor;
        [System.NonSerialized] float currentSpeed;

        public override void Animate(CharData cData, IAnimationContext context)
        {
            //if (currentAnchor == Vector2.zero) currentAnchor = Vector2.right;
            float BL, TL, TR, BR;
            Context ctx = context as Context;
            FixVector(ref currentAnchor);

            float angle = (context.animatorContext.PassedTime - context.animatorContext.StateTime(cData)) * currentSpeed * 2 + Mathf.Deg2Rad * 90;
            float t = Mathf.Sin(angle) / 2 + 0.5f;
            if (ctx.lastRoc == 0) ctx.lastRoc = Mathf.Cos(angle);

            if (Mathf.Sign(ctx.lastRoc) != Mathf.Sign(Mathf.Cos(angle)))
            {
                cData.SetVisibilityState(VisibilityState.Hidden);
            }

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


            Color32 blcolor = cData.mesh.GetColor(0);
            blcolor.a = (byte)(BL * blcolor.a);
            cData.mesh.SetColor(0, blcolor);

            Color32 tlcolor = cData.mesh.GetColor(1);
            tlcolor.a = (byte)(TL * tlcolor.a);
            cData.mesh.SetColor(1, tlcolor);

            Color32 trcolor = cData.mesh.GetColor(2);
            trcolor.a = (byte)(TR * trcolor.a);
            cData.mesh.SetColor(2, trcolor);

            Color32 brcolor = cData.mesh.GetColor(3);
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
            currentSpeed = speed;
            currentMaxScale = maxScale;
            currentMinScale = minScale;
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
                        if (!ParsingUtility.StringToFloat(kvp.Value, out _)) return false;
                        break;
                }
            }

            return true;
        }

        public override IAnimationContext GetNewContext()
        {
            return new Context() { lastRoc = 0 };
        }

        private class Context : IAnimationContext
        {
            public ReadOnlyAnimatorContext animatorContext { get; set; }
            public SegmentData segmentData { get; set; }

            public float lastRoc;
        }
    }
}