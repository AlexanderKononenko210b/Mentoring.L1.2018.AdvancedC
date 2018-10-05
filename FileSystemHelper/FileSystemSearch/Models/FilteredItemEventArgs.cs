using System;
using FileSystemSearch.Enums;

namespace FileSystemSearch.Models
{
    /// <summary>
    /// The information about directory or file which was filtered by predicate.
    /// </summary>
    public class FilteredItemEventArgs : EventArgs
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="FilteredItemEventArgs"/> class.
        /// </summary>
        /// <param name="path">The directory or file path.</param>
        /// <param name="type">The type.</param>
        public FilteredItemEventArgs(string path, SearchItems type)
        {
            FilteredPath = path;
            FilteredType = type;
        }

        /// <summary>
        /// Gets or sets filtered item.
        /// </summary>
        public string FilteredPath { get; }

        /// <summary>
        /// Gets or sets filtered item type.
        /// </summary>
        public SearchItems FilteredType { get; set; }

        /// <summary>
        /// Gets or sets cansel search.
        /// </summary>
        public bool CancelRequested { get; set; }

        /// <summary>
        /// Gets or sets exclude item from search.
        /// </summary>
        public bool ExcludeItem { get; set; }
    }
}
