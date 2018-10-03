namespace FileSystemSearch.Test.Helpers
{
    /// <summary>
    /// Represents an <see cref="EventTypes"/> enum.
    /// </summary>
    public enum EventTypes
    {
        /// <summary>
        /// Type event about start search.
        /// </summary>
        Start,

        /// <summary>
        /// Type event about finish search.
        /// </summary>
        Finish,

        /// <summary>
        /// Type event about founded file or directory.
        /// </summary>
        Founded,

        /// <summary>
        /// Type event about filtered file or directory.
        /// </summary>
        Filtered
    }
}
