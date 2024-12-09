namespace TMPEffects.Tags
{
    /// <summary>
    /// Interface for supplying prefix.
    /// </summary>
    internal interface ITMPPrefixSupplier
    {
        /// <summary>
        /// The supplied prefix.
        /// </summary>
        public char Prefix { get; }
    }
}
