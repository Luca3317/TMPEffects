using System.Collections.Generic;
using TMPEffects.TextProcessing;
using UnityEngine;
using TMPEffects.Components.Animator;
using TMPEffects.Components.CharacterData;

namespace TMPEffects.TMPAnimations.ShowAnimations
{
    [CreateAssetMenu(fileName = "new GrowShowAnimation", menuName = "TMPEffects/Show Animations/Grow")]
    public class GrowShowAnimation : TMPShowAnimation
    {
        [SerializeField] Vector2 anchor;
        [SerializeField] Vector2 direction;
        [SerializeField] float speed;
        [SerializeField] float minScale;
        [SerializeField] float maxScale;

        [System.NonSerialized] float currentMinScale;
        [System.NonSerialized] float currentMaxScale;
        [System.NonSerialized] Vector2 currentAnchor;
        [System.NonSerialized] Vector2 currentDirection; // ignored if anchor not center
        [System.NonSerialized] float currentSpeed;

        public override void Animate(ref CharData cData, IAnimationContext context)
        {


            Vector3 BL, TL, TR, BR;
            Context ctx = context as Context;
            FixVector(ref currentAnchor);


            float angle = (context.animatorContext.PassedTime - cData.visibleTime) * currentSpeed * 2 + Mathf.Deg2Rad * 270;
            float t = Mathf.Sin(angle) / 2 + 0.5f;
            if (ctx.lastRoc == 0) ctx.lastRoc = Mathf.Cos(angle);

            float multiplier = Mathf.Lerp(currentMinScale, currentMaxScale, t);
            if (Mathf.Sign(ctx.lastRoc) != Mathf.Sign(Mathf.Cos(angle)))
            {
                cData.SetVisibilityState(VisibilityState.Shown, context.animatorContext.PassedTime);
            }

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

            AnimationUtility.SetVertexRaw(0, BL, ref cData, ref context);
            AnimationUtility.SetVertexRaw(1, TL, ref cData, ref context);
            AnimationUtility.SetVertexRaw(2, TR, ref cData, ref context);
            AnimationUtility.SetVertexRaw(3, BR, ref cData, ref context);
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