using System.Collections.Generic;
using TMPEffects.CharacterData;
using TMPEffects.Components.Animator;
using TMPEffects.Parameters;
using TMPEffects.ParameterUtilityGenerator.Attributes;
using TMPEffects.TMPAnimations;

namespace TMPEffects.Parameters
{
    [TMPParameterType("OffsetProvider", typeof(OffsetTypePowerEnum), typeof(SceneOffsetTypePowerEnum), true)]
    public partial interface ITMPOffsetProvider
    {
        // TODO Later on might want to generalize even more; eg maybe tmpwriter needs offsets for some commands too?
        /// <summary>
        /// Get a timing offset for the given character.
        /// </summary>
        /// <param name="cData">The character to get an offset for.</param>
        /// <param name="segmentData">The relevant segment data.</param>
        /// <param name="animatorData">The relevant animator data.</param>
        /// <param name="ignoreAnimatorScaling">Whether to ignore the animator's scaling.</param>
        /// <returns>A timing offset (to be used, for example, with <see cref="AnimationUtility.Wave"/>).</returns>
        public float GetOffset(CharData cData, ITMPSegmentData segmentData, IAnimatorDataProvider animatorData,
            bool ignoreAnimatorScaling = false);

        /// <summary>
        /// Get the minimum / maximum offset for the given segment.
        /// </summary>
        /// <param name="min">The output parameter that will contain the minimum offset.</param>
        /// <param name="max">The output parameter that will contain the maximum offset.</param>
        /// <param name="segmentData">The relevant segment data.</param>
        /// <param name="animatorData">The relevant animator data.</param>
        /// <param name="ignoreAnimatorScaling">Whether to ignore the animator's scaling.</param>
        public void GetMinMaxOffset(out float min, out float max, ITMPSegmentData segmentData, IAnimatorDataProvider animatorData,
            bool ignoreAnimatorScaling = false);

        public static partial bool StringToOffsetProvider(string str, out ITMPOffsetProvider result,
            TMPEffects.Databases.ITMPKeywordDatabase keywords)
        {
            result = null;

            // if (ParameterParsing.GlobalKeywordDatabase.TryGetOffsetProvider(str, out result)) return true;
            // if (keywords != null && keywords.TryGetOffsetProvider(str, out result)) return true;

            switch (str)
            {
                case "sidx":
                case "sindex":
                case "segmentindex":
                    result = new OffsetTypePowerEnum(ParameterTypes.OffsetType.SegmentIndex);
                    return true;

                case "idx":
                case "index":
                    result = new OffsetTypePowerEnum(ParameterTypes.OffsetType.Index);
                    return true;

                case "word":
                case "wordidx":
                case "wordindex":
                    result = new OffsetTypePowerEnum(ParameterTypes.OffsetType.Word);
                    return true;

                case "line":
                case "lineno":
                case "linenumber":
                    result = new OffsetTypePowerEnum(ParameterTypes.OffsetType.Line);
                    return true;

                case "base":
                case "baseline":
                    result = new OffsetTypePowerEnum(ParameterTypes.OffsetType.Baseline);
                    return true;

                case "x":
                case "xpos":
                    result = new OffsetTypePowerEnum(ParameterTypes.OffsetType.XPos);
                    return true;

                case "y":
                case "ypos":
                    result = new OffsetTypePowerEnum(ParameterTypes.OffsetType.YPos);
                    return true;

                case "wordly":
                case "worldypos":
                    result = new OffsetTypePowerEnum(ParameterTypes.OffsetType.WorldYPos);
                    return true;

                case "wordlx":
                case "worldxpos":
                    result = new OffsetTypePowerEnum(ParameterTypes.OffsetType.WorldXPos);
                    return true;

                case "wordlz":
                case "worldzpos":
                    result = new OffsetTypePowerEnum(ParameterTypes.OffsetType.WorldZPos);
                    return true;
            }

            return false;
        }
    }
}