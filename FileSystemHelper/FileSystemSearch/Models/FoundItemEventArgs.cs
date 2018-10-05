using System;
using FileSystemSearch.Enums;

namespace FileSystemSearch.Models
{
    /// <summary>
    /// The information about directory or file which was found.
    /// </summary>
    public class FoundItemEventArgs : EventArgs
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="FoundItemEventArgs"/> class.
        /// </summary>
        /// <param name="path">The directory or file path.</param>
        /// <param name="type">The type.</param>
        public FoundItemEventArgs(string path, SearchItems type)
        {
            FoundPath = path;
            FoundType = type;
        }

        /// <summary>
        /// Gets or sets founded item path.
        /// </summary>
        public string FoundPath { get; }

        /// <summary>
        /// Gets or sets founded item path.
        /// </summary>
        public SearchItems FoundType { get; set; }

        /// <summary>
        /// Gets or sets cansel search.
        /// </summary>
        public bool CancelRequested { get; set; }
    }
}
