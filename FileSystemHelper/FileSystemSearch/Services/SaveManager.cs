using System.Collections;
using FileSystemSearch.Interfaces;

namespace FileSystemSearch.Services
{
    /// <summary>
    /// Represents a model of the <see cref="SaveManager"/> class.
    /// </summary>
    public class SaveManager : ISaveManager
    {
        /// <inheritdoc/>
        public void SaveItem(string path, IList collection)
        {
            collection.Add(path);
        }
    }
}
