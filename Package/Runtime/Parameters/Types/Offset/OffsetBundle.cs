using System;
using System.Collections.Generic;
using TMPEffects.CharacterData;
using TMPEffects.Components;
using TMPEffects.Components.Animator;
using TMPEffects.Parameters.Attributes;
using TMPEffects.TMPAnimations;
using UnityEngine;

namespace TMPEffects.Parameters
{
    /// <summary>
    /// Calculates offsets for characters (to be used with e.g. <see cref="Wave"/>).
    /// </summary>
    [Serializable]
    [TMPParameterBundle("OffsetBundle")]
    public partial class OffsetBundle
    {
        /// <summary>
        /// The <see cref="ITMPOffsetProvider"/> used to calculate offsets.
        /// </summary>
        public ITMPOffsetProvider Provider => provider;

        /// <summary>
        /// The uniformity that should be applied to the offset.
        /// </summary>
        public float Uniformity => uniformity;

        /// <summary>
        /// Whether to ignore animator scaling (for relevant offset types).
        /// </summary>
        public bool IgnoreAnimatorScaling => ignoreAnimatorScaling;

        /// <summary>
        /// Whether to zero base offset, i.e. whether to shift offsets so that the minimum offset is zero.
        /// </summary>
        public bool ZeroBasedOffset => zeroBasedOffset;
        
        /// <summary>
        /// Whether to cache offsets.
        /// </summary>
        public bool Cache
        {
            get => impl.Cache;
            set => impl.Cache = value;
        }

        private ITMPOffsetProvider provider
        {
            get => _provider ?? offsetProvider;
            set => _provider = value;
        }

        [TMPParameterBundleField("offset", "off")]
        private ITMPOffsetProvider _provider;

        // HideInInspector so it doesnt show "Add from TMPOffsetProvider" in context menu of timeline
        [SerializeField, HideInInspector] private OffsetTypePowerEnum offsetProvider = new OffsetTypePowerEnum();

        [TMPParameterBundleField("uniformity", "uni")] [SerializeField]
        private float uniformity = 1;

        [SerializeField] [TMPParameterBundleField("ignoreanimatorscaling", "ignorescaling", "ignorescl", "ignscl")]
        private bool ignoreAnimatorScaling = false;

        [SerializeField] [TMPParameterBundleField("zerooffset", "zerooff", "zoff", "zoffset", "ignlen")]
        private bool zeroBasedOffset = false;

        /// <summary>
        /// Clear the cached offsets (offsets will only be cached if <see cref="Cache"/> is true).
        /// </summary>
        public void ClearCache()
        {
            impl.ClearCache();
        }

        private OffsetBundleImpl impl = new OffsetBundleImpl();

        /// <summary>
        /// Get the offset for the given <see cref="CharData"/> in the context of the given <see cref="SegmentData"/>.<br/>
        /// Be aware that if <see cref="Cache"/> is true, this will cache offsets internally. You may clear the cache using <see cref="ClearCache"/>.
        /// </summary>
        /// <param name="cData">The <see cref="CharData"/> to get the offset for.</param>
        /// <param name="animatorData">Data about the animating <see cref="TMPAnimator"/>.</param>
        /// <param name="segmentData">Data about the contextual segment.</param>
        /// <returns>The offset for the given <see cref="CharData"/>.</returns>
        public float GetOffset(CharData cData, IAnimatorDataProvider animatorData, ITMPSegmentData segmentData = null)
            => impl.GetOffset(cData, animatorData, segmentData);

        /// <summary>
        /// Get the offset for the given <see cref="CharData"/> in the context of the given <see cref="SegmentData"/>.<br/>
        /// Be aware that if <see cref="Cache"/> is true, this will cache offsets internally. You may clear the cache using <see cref="ClearCache"/>.
        /// </summary>
        /// <param name="cData">The <see cref="CharData"/> to get the offset for.</param>
        /// <param name="context">The <see cref="IAnimationContext"/> of the animating <see cref="TMPAnimator"/>.</param>
        /// <returns>The offset for the given <see cref="CharData"/>.</returns>
        public float GetOffset(CharData cData, IAnimationContext context)
            => impl.GetOffset(cData, context);

        private static void Create_Hook(ref OffsetBundle newInstance, OffsetBundle originalInstance,
            OffsetBundleParameters parameters)
        {
            newInstance.impl = new OffsetBundleImpl();
            newInstance.offsetProvider = originalInstance.offsetProvider;

            // Create Hook always called after the parameters have been applied to newinstance
            newInstance.impl.IgnoreAnimatorScaling = newInstance.ignoreAnimatorScaling;
            newInstance.impl.Provider = newInstance.provider;
            newInstance.impl.ZeroBasedOffset = newInstance.zeroBasedOffset;
            newInstance.impl.Uniformity = newInstance.uniformity;
            
            // Cache by default
            newInstance.impl.Cache = true;
        }
    }

    internal class OffsetBundleImpl
    {
        public bool Cache
        {
            get => cache;
            set => cache = value;
        }

        public ITMPOffsetProvider Provider
        {
            get => provider;
            set
            {
                provider = value;
                ClearCache();
            }
        }

        public float Uniformity
        {
            get => uniformity;
            set
            {
                uniformity = value;
                ClearCache();
            }
        }

        public bool IgnoreAnimatorScaling
        {
            get => ignoreAnimatorScaling;
            set
            {
                ignoreAnimatorScaling = value;
                ClearCache();
            }
        }

        public bool ZeroBasedOffset
        {
            get => zeroBasedOffset;
            set
            {
                zeroBasedOffset = value;
                ClearCache();
            }
        }


        private bool cache = false;
        private ITMPOffsetProvider provider;
        private float uniformity;
        private bool ignoreAnimatorScaling;
        private bool zeroBasedOffset;

        private OffsetCache offsetCache;

        public void ClearCache()
        {
            offsetCache.ClearCache();
        }

        public OffsetBundleImpl()
        {
            offsetCache = new OffsetCache();
            offsetCache.offset = new Dictionary<CharData, float>();
        }

        public float GetOffset(CharData cData, IAnimatorDataProvider animatorData, ITMPSegmentData segmentData = null)
        {
            float offset;
            if (Cache && offsetCache.GetOffset(cData, out offset))
                return offset;

            if (segmentData == null)
            {
                segmentData = TMPAnimationUtility.GetMockedSegment(
                    animatorData.Animator.TextComponent.GetParsedText().Length,
                    animatorData.Animator.CharData);
            }

            offset = Provider.GetOffset(cData, segmentData, animatorData, IgnoreAnimatorScaling);
            
            if (ZeroBasedOffset)
            {
                float min, max;
                if (Cache)
                {
                    if (!offsetCache.GetMinMaxOffset(out min, out max))
                    {
                        Provider.GetMinMaxOffset(out min, out max, segmentData, animatorData);
                        offsetCache.CacheMinMax(min, max);
                    }
                }
                else
                    Provider.GetMinMaxOffset(out min, out max, segmentData, animatorData);

                float zeroedOffset = offset - min;
                float zeroedMax = max - min;
                if (Uniformity >= 0)
                {
                    offset = zeroedOffset;
                }
                else
                {
                    offset = zeroedMax - zeroedOffset;
                }
            }

            offset *= Uniformity;
            if (Cache)
                offsetCache.CacheOffset(cData, offset);
            
            return offset;
        }

        public float GetOffset(CharData cData, IAnimationContext context) =>
            GetOffset(cData, context.AnimatorContext, context.SegmentData);
    }

    internal struct OffsetCache
    {
        public float? maxOffset;
        public float? minOffset;
        public Dictionary<CharData, float> offset;

        public void CacheOffset(CharData cData, float cOffset)
        {
            offset[cData] = cOffset;
        }

        public void CacheMinMax(float min, float max)
        {
            minOffset = min;
            maxOffset = max;
        }

        public bool GetOffset(CharData cData, out float cOffset)
        {
            return offset.TryGetValue(cData, out cOffset);
        }

        public bool GetMinMaxOffset(out float min, out float max)
        {
            if (maxOffset.HasValue && minOffset.HasValue)
            {
                min = minOffset.Value;
                max = maxOffset.Value;
                return true;
            }

            min = 0f;
            max = 0f;
            return false;
        }
        

        public void ClearCache()
        {
            maxOffset = null;
            minOffset = null;
            offset.Clear();
        }
    }
}