using System.Collections.Generic;
using TMPEffects;
using TMPEffects.TextProcessing;
using UnityEngine;
using TMPEffects.Components.Animator;
using static TMPEffects.TMPAnimations.AnimationUtility;
using TMPEffects.Components.CharacterData;

namespace TMPEffects.TMPAnimations.Animations
{
    [CreateAssetMenu(fileName = "new GrowAnimation", menuName = "TMPEffects/Animations/Grow")]
    public class GrowAnimation : TMPAnimation
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

        public override void Animate(CharData cData, IAnimationContext context)
        {
            Vector3 BL, TL, TR, BR;
            Context ctx = context.customData as Context;
            FixVector(ref currentAnchor);
            if (ctx.lastUpdated == -1) { ctx.lastUpdated = context.animatorContext.PassedTime; }

            if (cData.info.index == context.segmentData.firstAnimationIndex) ctx.passed += context.animatorContext.DeltaTime * currentSpeed;
            //if (cData.segmentIndex == 0) ctx.passed += context.animatorContext.deltaTime * currentSpeed;
            float ppT = ctx.passed;
            float lerpT = Mathf.PingPong(ppT, 1);
            float repeat = Mathf.Repeat(ppT, 2) - 1;
            float multiplier;

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

            if (ctx.waitingSince == -1)
            {
                multiplier = Mathf.Lerp(currentMinScale, currentMaxScale, lerpT);

            }
            else
            {
                multiplier = ctx.isMax ? currentMaxScale : currentMinScale;
            }

            ctx.lastRoc = repeat;

            if (currentAnchor == Vector2.zero)
            {
                FixVector(ref currentDirection);

                if (currentDirection == Vector2.zero)
                {
                    BL = cData.info.initialPosition + (cData.mesh.initial.vertex_BL.position - cData.info.initialPosition) * multiplier;
                    TL = cData.info.initialPosition + (cData.mesh.initial.vertex_TL.position - cData.info.initialPosition) * multiplier;
                    TR = cData.info.initialPosition + (cData.mesh.initial.vertex_TR.position - cData.info.initialPosition) * multiplier;
                    BR = cData.info.initialPosition + (cData.mesh.initial.vertex_BR.position - cData.info.initialPosition) * multiplier;
                }
                else if (currentDirection == Vector2.up || currentDirection == Vector2.down)
                {
                    BL = new Vector2(cData.mesh.initial.vertex_BL.position.x, cData.info.initialPosition.y) + Vector2.up * (cData.mesh.initial.vertex_BL.position.y - cData.info.initialPosition.y) * multiplier;
                    TL = new Vector2(cData.mesh.initial.vertex_TL.position.x, cData.info.initialPosition.y) + Vector2.up * (cData.mesh.initial.vertex_TL.position.y - cData.info.initialPosition.y) * multiplier;
                    TR = new Vector2(cData.mesh.initial.vertex_TR.position.x, cData.info.initialPosition.y) + Vector2.up * (cData.mesh.initial.vertex_TR.position.y - cData.info.initialPosition.y) * multiplier;
                    BR = new Vector2(cData.mesh.initial.vertex_BR.position.x, cData.info.initialPosition.y) + Vector2.up * (cData.mesh.initial.vertex_BR.position.y - cData.info.initialPosition.y) * multiplier;
                }
                else if (currentDirection == Vector2.right || currentDirection == Vector2.left)
                {
                    BL = new Vector2(cData.info.initialPosition.x, cData.mesh.initial.vertex_BL.position.y) + Vector2.right * (cData.mesh.initial.vertex_BL.position.x - cData.info.initialPosition.x) * multiplier;
                    TL = new Vector2(cData.info.initialPosition.x, cData.mesh.initial.vertex_TL.position.y) + Vector2.right * (cData.mesh.initial.vertex_TL.position.x - cData.info.initialPosition.x) * multiplier;
                    TR = new Vector2(cData.info.initialPosition.x, cData.mesh.initial.vertex_TR.position.y) + Vector2.right * (cData.mesh.initial.vertex_TR.position.x - cData.info.initialPosition.x) * multiplier;
                    BR = new Vector2(cData.info.initialPosition.x, cData.mesh.initial.vertex_BR.position.y) + Vector2.right * (cData.mesh.initial.vertex_BR.position.x - cData.info.initialPosition.x) * multiplier;
                }
                else if (currentDirection == new Vector2(1, 1) || currentDirection == new Vector2(-1, -1))
                {
                    TR = cData.mesh.initial.vertex_TR.position;
                    BL = cData.mesh.initial.vertex_BL.position;

                    TL = cData.info.initialPosition + (cData.mesh.initial.vertex_TL.position - cData.info.initialPosition) * multiplier;
                    BR = cData.info.initialPosition + (cData.mesh.initial.vertex_BR.position - cData.info.initialPosition) * multiplier;
                }
                else if (currentDirection == new Vector2(1, -1) || currentDirection == new Vector2(-1, 1))
                {
                    TL = cData.mesh.initial.vertex_TL.position;
                    BR = cData.mesh.initial.vertex_BR.position;

                    TR = cData.info.initialPosition + (cData.mesh.initial.vertex_TR.position - cData.info.initialPosition) * multiplier;
                    BL = cData.info.initialPosition + (cData.mesh.initial.vertex_BL.position - cData.info.initialPosition) * multiplier;
                }
                else throw new System.Exception();
            }
            else
            {
                if (currentAnchor == Vector2.right)
                {
                    TR = cData.mesh.initial.vertex_TR.position;
                    BR = cData.mesh.initial.vertex_BR.position;

                    TL = TR + (cData.mesh.initial.vertex_TL.position - TR) * multiplier;
                    BL = BR + (cData.mesh.initial.vertex_BL.position - BR) * multiplier;
                }
                else if (currentAnchor == Vector2.left)
                {
                    TL = cData.mesh.initial.vertex_TL.position;
                    BL = cData.mesh.initial.vertex_BL.position;

                    TR = TL + (cData.mesh.initial.vertex_TR.position - TL) * multiplier;
                    BR = BL + (cData.mesh.initial.vertex_BR.position - BL) * multiplier;
                }
                else if (currentAnchor == Vector2.up)
                {
                    TL = cData.mesh.initial.vertex_TL.position;
                    TR = cData.mesh.initial.vertex_TR.position;

                    BL = TL + (cData.mesh.initial.vertex_BL.position - TL) * multiplier;
                    BR = TR + (cData.mesh.initial.vertex_BR.position - TR) * multiplier;
                }
                else if (currentAnchor == Vector2.down)
                {
                    BL = cData.mesh.initial.vertex_BL.position;
                    BR = cData.mesh.initial.vertex_BR.position;

                    TL = BL + (cData.mesh.initial.vertex_TL.position - BL) * multiplier;
                    TR = BR + (cData.mesh.initial.vertex_TR.position - BR) * multiplier;
                }
                else if (currentAnchor == new Vector2(1, 1))
                {
                    TR = cData.mesh.initial.vertex_TR.position;

                    TL = TR + (cData.mesh.initial.vertex_TL.position - TR) * multiplier;
                    BL = TR + (cData.mesh.initial.vertex_BL.position - TR) * multiplier;
                    BR = TR + (cData.mesh.initial.vertex_BR.position - TR) * multiplier;
                }
                else if (currentAnchor == new Vector2(-1, 1))
                {
                    TL = cData.mesh.initial.vertex_TL.position;

                    TR = TL + (cData.mesh.initial.vertex_TR.position - TL) * multiplier;
                    BL = TL + (cData.mesh.initial.vertex_BL.position - TL) * multiplier;
                    BR = TL + (cData.mesh.initial.vertex_BR.position - TL) * multiplier;
                }
                else if (currentAnchor == new Vector2(1, -1))
                {
                    BR = cData.mesh.initial.vertex_BR.position;

                    TR = BR + (cData.mesh.initial.vertex_TR.position - BR) * multiplier;
                    BL = BR + (cData.mesh.initial.vertex_BL.position - BR) * multiplier;
                    TL = BR + (cData.mesh.initial.vertex_TL.position - BR) * multiplier;
                }
                else if (currentAnchor == new Vector2(-1, -1))
                {
                    BL = cData.mesh.initial.vertex_BL.position;

                    TR = BL + (cData.mesh.initial.vertex_TR.position - BL) * multiplier;
                    BR = BL + (cData.mesh.initial.vertex_BR.position - BL) * multiplier;
                    TL = BL + (cData.mesh.initial.vertex_TL.position - BL) * multiplier;
                }
                else throw new System.Exception();
            }

            SetVertexRaw(0, BL, cData, ref context);
            SetVertexRaw(1, TL, cData, ref context);
            SetVertexRaw(2, TR, cData, ref context);
            SetVertexRaw(3, BR, cData, ref context);
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
                            case "topleft": currentAnchor = new Vector2(-1, 1); break;

                            case "tr":
                            case "topright": currentAnchor = new Vector2(1, 1); break;

                            case "bl":
                            case "bottomleft": currentAnchor = new Vector2(-1, -1); break;

                            case "br":
                            case "bottomright": currentAnchor = new Vector2(1, -1); break;

                            case "c":
                            case "center": currentAnchor = Vector2.zero; break;

                            case "l":
                            case "left": currentAnchor = Vector2.left; break;

                            case "r":
                            case "right": currentAnchor = Vector2.right; break;

                            case "t":
                            case "top": currentAnchor = Vector2.up; break;

                            case "b":
                            case "bottom": currentAnchor = Vector2.down; break;

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

        public override object GetNewCustomData()
        {
            return new Context() { lastUpdated = -1f, lastRoc = 0, waitingSince = -1, passed = 0 };
        }

        private class Context
        {
            public float lastUpdated;
            public float lastRoc;
            public float waitingSince;
            public bool isMax;
            public float passed;
        }
    }
}
