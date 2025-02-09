namespace MonoGame.Invisible
{
    /// <summary>
    /// Enum for selecting the desired transparency mode.
    /// </summary>
    public enum TransparencyMode
    {
        /// <summary>
        /// Fast, prefered and recommended way of making your MonoGame window transparent!
        /// </summary>
        /// <remarks>
        /// <em>Tip:</em> Use <b>new Color(1, 1, 1)</b> to support shadows and dark edges the best way possible
        /// in this mode. Another option would be <b>Magenta</b>, but it can cause some artifacts in certain situations.
        /// </remarks>
        ColorKey,

        /// <summary>
        /// Slow and <b>NOT</b> the prefered way of making your MonoGame window transparent!
        /// Use it for tinkering or if the ColorKey version does not satisfy you.
        /// </summary>
        PerPixelAlpha
    }
}
