using FileSystemSearch.Enums;

namespace FileSystemSearch.Interfaces
{
    /// <summary>
    /// Represents a model of the <see cref="IValidator"/> interface.
    /// </summary>
    public interface IValidator
    {
        /// <summary>
        /// Validate root path.
        /// </summary>
        /// <param name="path">The directory or file path.</param>
        /// <returns>The <see cref="SearchItems"/>.</returns>
        SearchItems Exists(string path);

        /// <summary>
        /// Check directory or file by filter.
        /// </summary>
        /// <param name="path">The directory or file path.</param>
        /// <returns>True if directory or file path is filtered.</returns>
        bool IsFiltered(string path);
    }
}
