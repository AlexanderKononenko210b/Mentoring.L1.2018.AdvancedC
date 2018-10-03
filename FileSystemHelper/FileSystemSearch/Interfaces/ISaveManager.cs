using System.Collections;

namespace FileSystemSearch.Interfaces
{
    /// <summary>
    /// Represents an <see cref="ISaveManager"/> interface.
    /// </summary>
    public interface ISaveManager
    {
        /// <summary>
        /// Save item in storage.
        /// </summary>
        /// <param name="path">The path directory or file.</param>
        /// <param name="collection">The collection for save founded item.</param>
        void SaveItem(string path, IList collection);
    }
}
