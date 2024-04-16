namespace TMPEffects.TMPCommands
{
    /// <summary>
    /// Defines types of commands.
    /// </summary>
    /// <remarks>
    /// <list type="table">
    /// <item><see cref="TagType.Index"/>: This type of command is executed when the <see cref="TMPWriter"/> shows the character at the corresponding index. It does not need to be closed. Example: This is <!delay=0.01> my text</item>
    /// <item><see cref="TagType.Block"/>: This type of command is executed when the <see cref="TMPWriter"/> shows the character at the first corresponding index. It needs to be closed, and will operate on the enclosed text. Example: This <!show>is my</!show> text</item>
    /// <item><see cref="TagType.Either"/>: Both applications are valid.</item>
    /// </list>
    /// </remarks>
    public enum TagType
    {
        Index = 0,
        Block = 1,
        Either = 2
    }
}