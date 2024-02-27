using TMPEffects.TextProcessing;

namespace TMPEffects.Tags
{
    /// <summary>
    /// Tag validation interface.
    /// </summary>
    public interface ITMPTagValidator
    {
        /// <summary>
        /// Validate a given <see cref="ParsingUtility.TagInfo"/>.
        /// </summary>
        /// <param name="tagInfo">Information about the tag.</param>
        /// <param name="data">Assuming the tag is validated, this will be set to the created <see cref="EffectTag"/>; otherwise it will be null.</param>
        /// <returns>true if the tag is successfully validated; false otherwise.</returns>
        public bool ValidateTag(ParsingUtility.TagInfo tagInfo, out EffectTag data);
        /// <summary>
        /// Validate a given <see cref="ParsingUtility.TagInfo"/>.
        /// </summary>
        /// <param name="tagInfo">Information about the tag.</param>
        /// <returns>true if the tag is successfully validated; false otherwise.</returns>
        public bool ValidateTag(ParsingUtility.TagInfo tagInfo);
        /// <summary>
        /// Validate a given <see cref="EffectTag"/>.
        /// </summary>
        /// <param name="tag">Information about the tag.</param>
        /// <returns>true if the tag is successfully validated; false otherwise.</returns>
        public bool ValidateTag(EffectTag tag);
    }
}
