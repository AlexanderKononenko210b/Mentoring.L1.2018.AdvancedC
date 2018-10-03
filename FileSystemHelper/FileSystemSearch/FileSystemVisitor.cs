using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FileSystemSearch.Enums;
using FileSystemSearch.EventArguments;
using FileSystemSearch.Interfaces;

namespace FileSystemSearch
{
    /// <summary>
    /// File system visitor for searching files and directories.
    /// </summary>
    public sealed class FileSystemVisitor : IEnumerable<string>
    {
        private readonly IValidator _validator;
        private readonly ISaveManager _saveManager;

        private bool _cancelOperation;
        private bool _excludeItem;
        private List<string> _savedItems = new List<string>();

        /// <summary>
        /// Initialize a new instance of the <see cref="FileSystemVisitor"/>
        /// </summary>
        /// <param name="validator">The <see cref="IValidator"/>.</param>
        /// <param name="saveManager">The <see cref="ISaveManager"/>.</param>
        public FileSystemVisitor(IValidator validator, ISaveManager saveManager)
        {
            _validator = validator;
            _saveManager = saveManager;
        }

        /// <summary>
        /// Gets count elements in collection.
        /// </summary>
        public int Count => _savedItems.Count;

        #region Indexer

        /// <summary>
        /// Indexer for access to the collection elements.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The collection element.</returns>
        public string this[int index]
        {
            get
            {
                if (index < 0 || index > this.Count - 1)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(index)}");
                }

                return _savedItems[index];
            }
        }

        #endregion

        #region Iterator

        /// <summary>
        /// Get Enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<string> GetEnumerator()
        {
            return GetItems().GetEnumerator();
        }

        /// <summary>
        /// Get Enumerator.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Get collection elements.
        /// </summary>
        /// <returns>The <see cref="IEnumerable"/></returns>
        private IEnumerable<string> GetItems()
        {
            foreach (var path in _savedItems)
                yield return path;
        }

        #endregion

        #region Events

        /// <summary>
        /// The start search event.
        /// </summary>
        public event EventHandler<EventArgs> Start;

        /// <summary>
        /// The finish search event.
        /// </summary>
        public event EventHandler<EventArgs> Finish;

        /// <summary>
        /// The directory or file founded event.
        /// </summary>
        public event EventHandler<FoundedItemEventArgs> FoundedItem;

        /// <summary>
        /// The directory or file filtered event.
        /// </summary>
        public event EventHandler<FilteredItemEventArgs> FilteredItem;

        /// <summary>
        /// Invoke start search event.
        /// </summary>
        private void OnStart()
        {
            this.Start?.Invoke(this, EventArgs.Empty);

            Console.WriteLine("Start");
        }

        /// <summary>
        /// Invoke finish search event.
        /// </summary>
        private void OnFinish()
        {
            this.Finish?.Invoke(this, EventArgs.Empty);

            Console.WriteLine($"Finish. Saved {_savedItems.Count} files and directories.");
        }

        /// <summary>
        /// Invoke founded search event.
        /// </summary>
        private void OnFoundedItem(FoundedItemEventArgs args)
        {
            Console.WriteLine($"Founded {args.FoundedPath}");

            this.FoundedItem?.Invoke(this, args);

            if (args.CancelRequested)
            {
                _cancelOperation = true;

                Console.WriteLine("Cansel operation...");
            }
        }

        /// <summary>
        /// Invoke filtered search event.
        /// </summary>
        private void OnFilteredItem(FilteredItemEventArgs args)
        {
            Console.WriteLine($"Filtered {args.FilteredPath}");

            this.FilteredItem?.Invoke(this, args);

            if (args.CancelRequested)
            {
                _cancelOperation = true;

                Console.WriteLine("Cansel operation...");
            }

            if (args.ExcludeItem)
            {
                _excludeItem = true;

                Console.WriteLine($"Exclude {args.FilteredType}");
            }
        }

        #endregion

        #region Search

        /// <summary>
        /// Search files and directories by predicate.
        /// </summary>
        /// <param name="rootPath">The root path.</param>
        public void Search(string rootPath)
        {
            this.OnStart();

            if (rootPath == null)
            {
                throw new NullReferenceException($"{nameof(rootPath)}");
            }

            if (string.IsNullOrWhiteSpace(rootPath))
            {
                throw new ArgumentOutOfRangeException($"{nameof(rootPath)}");
            }

            var rootPathValidate = _validator.Exists(rootPath);

            switch (rootPathValidate)
            {
                case (SearchItems.Directory):
                {
                    this.DirectoryVisitor(rootPath);
                    break;
                }
                case (SearchItems.File):
                {
                    this.FileVisitor(rootPath);
                    break;
                }
                default:
                {
                    throw new FileNotFoundException("Search file or directory is absent or incorrect path. Please try again.");
                }
            }

            this.OnFinish();
        }

        /// <summary>
        /// The directory visitor.
        /// </summary>
        /// <param name="rootPath">The directory path.</param>
        private void DirectoryVisitor(string rootPath)
        {
            if (_cancelOperation) return;

            this.OnFoundedItem(new FoundedItemEventArgs(rootPath, SearchItems.Directory));

            if (_validator.IsFiltered(rootPath))
            {
                var filteredArgs = new FilteredItemEventArgs(rootPath, SearchItems.Directory);

                this.OnFilteredItem(filteredArgs);

                if (_cancelOperation) return;

                if (!_excludeItem)
                {
                    _saveManager.SaveItem(rootPath, _savedItems);
                }
            }

            foreach (var file in Directory.EnumerateFiles(rootPath))
            {
                this.FileVisitor(file);

                if (_cancelOperation) return;
            }

            var directories = Directory.GetDirectories(rootPath, "*.*", SearchOption.AllDirectories);

            if (directories.Any())
            {
                foreach (var directory in directories)
                {
                    this.DirectoryVisitor(directory);
                }
            }
        }

        /// <summary>
        /// The file visitor.
        /// </summary>
        /// <param name="rootPath">The file path.</param>
        private void FileVisitor(string rootPath)
        {
            if (_cancelOperation) return;

            this.OnFoundedItem(new FoundedItemEventArgs(rootPath, SearchItems.File));

            if (_cancelOperation) return;

            if (_validator.IsFiltered(rootPath))
            {
                var filteredArgs = new FilteredItemEventArgs(rootPath, SearchItems.File);

                this.OnFilteredItem(filteredArgs);

                if (_cancelOperation || _excludeItem)
                {
                    return;
                }

                _saveManager.SaveItem(rootPath, _savedItems);
            }
        }

        #endregion
    }
}
