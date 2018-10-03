using System;
using FileSystemSearch.Enums;

namespace FileSystemSearch.EventArguments
{
    /// <summary>
    /// The information about directory or file which was found.
    /// </summary>
    public class FoundedItemEventArgs : EventArgs
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="FoundedItemEventArgs"/> class.
        /// </summary>
        /// <param name="path">The directory or file path.</param>
        /// <param name="type">The type.</param>
        public FoundedItemEventArgs(string path, SearchItems type)
        {
            FoundedPath = path;
            FoundedType = type;
        }

        /// <summary>
        /// Gets or sets founded item path.
        /// </summary>
        public string FoundedPath { get; }

        /// <summary>
        /// Gets or sets founded item path.
        /// </summary>
        public SearchItems FoundedType { get; set; }

        /// <summary>
        /// Gets or sets cansel search.
        /// </summary>
        public bool CancelRequested { get; set; }
    }
}
