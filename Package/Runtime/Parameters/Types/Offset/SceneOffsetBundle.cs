using System;
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
    [TMPParameterBundle("SceneOffsetBundle")]
    public partial class SceneOffsetBundle
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

        private OffsetBundleImpl impl;

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

        private static void Create_Hook(ref SceneOffsetBundle newInstance, SceneOffsetBundle originalInstance,
            SceneOffsetBundleParameters parameters)
        {
            newInstance.impl = new OffsetBundleImpl();

            // Create Hook always called after the parameters have been applied to newinstance
            newInstance.impl.IgnoreAnimatorScaling = newInstance.ignoreAnimatorScaling;
            newInstance.impl.Provider = newInstance.provider;
            newInstance.impl.ZeroBasedOffset = newInstance.zeroBasedOffset;
            newInstance.impl.Uniformity = newInstance.uniformity;
            // Cache by default
            newInstance.impl.Cache = true;
        }
    }
}