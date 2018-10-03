using System;
using System.IO;
using FileSystemSearch.Enums;
using FileSystemSearch.Interfaces;

namespace FileSystemSearch.Services
{
    /// <summary>
    /// Represents a model of the <see cref="Validator"/> class.
    /// </summary>
    public class Validator : IValidator
    {
        private readonly Predicate<string> _predicate;

        /// <summary>
        /// Initialize a new instance of the <see cref="Validator"/> class.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        public Validator(Predicate<string> predicate)
        {
            _predicate = predicate;
        }

        /// <inheritdoc/>
        public SearchItems Exists(string path)
        {
            if (Directory.Exists(path))
            {
                return SearchItems.Directory;
            }

            if (File.Exists(path))
            {
                return SearchItems.File;
            }

            return SearchItems.Unknown;
        }

        /// <inheritdoc/>
        public bool IsFiltered(string path)
        {
            return _predicate(path);
        }
    }
}
