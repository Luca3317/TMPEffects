namespace TMPEffects.Tags
{
    /// <summary>
    /// Interface for supplying prefix.
    /// </summary>
    public interface ITMPPrefixSupplier
    {
        /// <summary>
        /// The supplied prefix.
        /// </summary>
        public char Prefix { get; }
    }
}
