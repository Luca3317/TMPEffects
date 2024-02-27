using UnityEngine;
using TMPEffects.Components.CharacterData;

namespace TMPEffects.TMPAnimations
{
    // Big-ish TODO:
    // Make some public static utility to apply scale (for vertex, position, etc)
    // Then use that in both the animator to scale and this 


    /// <summary>
    /// Utility methods for animations.
    /// </summary>
    public static class AnimationUtility
    {
        /// <summary>
        /// Begins a waiting process.
        /// </summary>
        /// <remarks>
        /// You need to manually store the new value for <paramref name="waitingSince"/>
        /// (for which you may use your custom IAnimationContext).
        /// </remarks>
        /// <param name="ctx">The animation context.</param>
        /// <param name="waitingSince">Will be set to the current time.</param>
        public static void BeginWaiting(in IAnimationContext ctx, out float waitingSince)
        {
            waitingSince = ctx.animatorContext.passedTime;
        }

        /// <summary>
        /// Checks if the waiting process is done.<br/>
        /// This will also return false if not waiting (i.e. waitingSince is -1).
        /// </summary>
        /// <remarks>
        /// In addition to the return value, the value for <paramref name="waitingSince"/> also indicates whether
        /// waiting is done (it will be set to -1).
        /// </remarks>
        /// <param name="waitTime">The amount of time to wait, in seconds.</param>
        /// <param name="ctx">The animation context.</param>
        /// <param name="waitingSince">Since when you have been waiting.</param>
        /// <returns></returns>
        public static bool TryFinishWaiting(float waitTime, in IAnimationContext ctx, ref float waitingSince)
        {
            if (waitingSince < 0) return false;
            if ((ctx.animatorContext.passedTime - waitingSince) >= waitTime)
            {
                waitingSince = -1;
                return true;
            }
            return false;
        }



        #region Raw Positions & Deltas
        public static Vector3 GetRawVertex(int index, Vector3 position, ref CharData cData, ref IAnimationContext ctx)
        {
            return GetRawPosition(position, cData.mesh.initial[index].position, cData.info.referenceScale, ref ctx);
        }

        public static Vector3 GetRawPosition(Vector3 position, ref CharData cData, ref IAnimationContext ctx)
        {
            return GetRawPosition(position, cData.info.initialPosition, cData.info.referenceScale, ref ctx);
        }
        public static Vector3 GetRawPosition(Vector3 position, Vector3 referencePosition, float scale, ref IAnimationContext ctx)
        {
            if (!ctx.animatorContext.scaleAnimations) return position;
            return (position - referencePosition) / scale + referencePosition;
        }

        public static Vector3 GetRawDelta(Vector3 delta, ref CharData cData, ref IAnimationContext ctx)
        {
            return GetRawDelta(delta, cData.info.referenceScale, ref ctx);
        }
        public static Vector3 GetRawDelta(Vector3 delta, float scale, ref IAnimationContext ctx)
        {
            if (!ctx.animatorContext.scaleAnimations) return delta;
            return delta / scale;
        }

        /// <summary>
        /// Set the raw position of the vertex at the given index. This position will ignore the animator's scaling.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="position"></param>
        /// <param name="cData"></param>
        /// <param name="ctx"></param>
        public static void SetVertexRaw(int index, Vector3 position, ref CharData cData, ref IAnimationContext ctx)
        {
            if (ctx.animatorContext.scaleAnimations)
            {
                Vector3 ogPos = cData.mesh.initial.GetPosition(index);
                cData.SetVertex(index, (position - ogPos) / cData.info.referenceScale + ogPos);
            }
            else
            {
                cData.SetVertex(index, position);
            }
        }
        /// <summary>
        /// Set the raw position of the character. This position will ignore the animator's scaling.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="position"></param>
        /// <param name="cData"></param>
        /// <param name="ctx"></param>
        public static void SetPositionRaw(Vector3 position, ref CharData cData, ref IAnimationContext ctx)
        {
            if (ctx.animatorContext.scaleAnimations)
            {
                Vector3 ogPos = cData.info.initialPosition;
                cData.SetPosition((position - ogPos) / cData.info.referenceScale + ogPos);
            }
            else
            {
                cData.SetPosition(position);
            }
        }
        /// <summary>
        /// Set the raw pivot of the character. This position will ignore the animator's scaling.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="position"></param>
        /// <param name="cData"></param>
        /// <param name="ctx"></param>
        public static void SetPivotRaw(Vector3 pivot, ref CharData cData, ref IAnimationContext ctx)
        {
            if (ctx.animatorContext.scaleAnimations)
            {
                Vector3 ogPos = cData.info.initialPosition;
                cData.SetPivot((pivot - ogPos) / cData.info.referenceScale + ogPos);
            }
            else
            {
                cData.SetPivot(pivot);
            }
        }
        /// <summary>
        /// Add a raw delta to the vertex at the given index. This delta will ignore the animator's scaling.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="delta"></param>
        /// <param name="cData"></param>
        /// <param name="ctx"></param>
        public static void AddVertexDeltaRaw(int index, Vector3 delta, ref CharData cData, ref IAnimationContext ctx)
        {
            if (ctx.animatorContext.scaleAnimations)
            {
                cData.AddVertexDelta(index, delta / cData.info.referenceScale);
            }
            else
            {
                cData.AddVertexDelta(index, delta);
            }
        }
        /// <summary>
        /// Add a raw delta to the position of the character. This delta will ignore the animator's scaling.
        /// </summary>
        /// <param name="delta"></param>
        /// <param name="cData"></param>
        /// <param name="ctx"></param>
        public static void AddPositionDeltaRaw(Vector3 delta, ref CharData cData, ref IAnimationContext ctx)
        {
            if (ctx.animatorContext.scaleAnimations)
            {
                cData.AddPositionDelta(delta / cData.info.referenceScale);
            }
            else
            {
                cData.AddPositionDelta(delta);
            }
        }
        /// <summary>
        /// Add a raw delta to the pivot of the character. This delta will ignore the animator's scaling.
        /// </summary>
        /// <param name="delta"></param>
        /// <param name="cData"></param>
        /// <param name="ctx"></param>
        public static void AddPivotDeltaRaw(Vector3 delta, ref CharData cData, ref IAnimationContext ctx)
        {
            if (ctx.animatorContext.scaleAnimations)
            {
                cData.AddPivotDelta(delta / cData.info.referenceScale);
            }
            else
            {
                cData.AddPivotDelta(delta);
            }
        }
        #endregion
    }
}

