using TMPEffects.TextProcessing;

namespace TMPEffects.Tags
{
    /// <summary>
    /// Tag validation interface.
    /// </summary>
    public interface ITMPTagValidator
    {
        /// <summary>
        /// Validate a given <see cref="ParsingUtility.TagInfo"/>. MUST be of type <see cref="TextProcessing.ParsingUtility.TagType.Open"/>.
        /// </summary>
        /// <param name="tagInfo">Information about the tag.</param>
        /// <param name="data">Assuming the tag is validated, this will be set to the created <see cref="TMPEffectTag"/>; otherwise it will be null.</param>
        /// <param name="endIndex">Assuming the tag is validated, this will be set to the endIndex of the tag. TODO this arguably exceeds ITMPTagValidators responsibilities</param>
        /// <returns>true if the tag is successfully validated; false otherwise.</returns>
        /// <exception cref="System.ArgumentException"></exception>
        public bool ValidateOpenTag(ParsingUtility.TagInfo tagInfo, out TMPEffectTag data, out int endIndex);
        /// <summary>
        /// Validate a given <see cref="ParsingUtility.TagInfo"/>.
        /// </summary>
        /// <param name="tagInfo">Information about the tag.</param>
        /// <returns>true if the tag is successfully validated; false otherwise.</returns>
        public bool ValidateTag(ParsingUtility.TagInfo tagInfo);
        /// <summary>
        /// Validate a given <see cref="TMPEffectTag"/>.
        /// </summary>
        /// <param name="tag">Information about the tag.</param>
        /// <returns>true if the tag is successfully validated; false otherwise.</returns>
        public bool ValidateTag(TMPEffectTag tag);
    }
}
