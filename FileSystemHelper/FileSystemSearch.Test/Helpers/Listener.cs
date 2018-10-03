using System;
using System.Collections.Generic;
using FileSystemSearch.EventArguments;

namespace FileSystemSearch.Test.Helpers
{
    /// <summary>
    /// Represents a model of the <see cref="Listener"/> class.
    /// </summary>
    public class Listener
    {
        private readonly List<SearchInfo> _eventInfoList = new List<SearchInfo>();

        public int _countForCansel = 10;
        public int _countForExclude = 10;

        /// <summary>
        /// Initialize a new instance of the <see cref="Listener"/> class.
        /// </summary>
        public Listener(FileSystemVisitor visitor)
        {
            visitor.Start += StartHandler;
            visitor.Finish += FinishHandler;
            visitor.FoundedItem += FoundHandler;
            visitor.FilteredItem += FilteredHandler;
        }

        /// <summary>
        /// Gets or sets count collection elements.
        /// </summary>
        public int Count => _eventInfoList.Count;

        /// <summary>
        /// Indexator for access to the collection elements.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public SearchInfo this[int index]
        {
            get
            {
                if (index < 0 || index > _eventInfoList.Count)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(index)}");
                }

                return _eventInfoList[index];
            }
        }

        /// <summary>
        /// Handler for start event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The event args.</param>
        private void StartHandler(object sender, EventArgs eventArgs)
        {
            var searchInfo = new SearchInfo
            {
                EventType = EventTypes.Start
            };

            _eventInfoList.Add(searchInfo);
        }

        /// <summary>
        /// Handler for finish event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The event args.</param>
        private void FinishHandler(object sender, EventArgs eventArgs)
        {
            var searchInfo = new SearchInfo
            {
                EventType = EventTypes.Finish
            };

            _eventInfoList.Add(searchInfo);
        }

        /// <summary>
        /// Handler for found event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The event args.</param>
        private void FoundHandler(object sender, FoundedItemEventArgs eventArgs)
        {
            //condition for cansel operation
            if (_eventInfoList.Count < _countForCansel)
            {
                var searchInfo = new SearchInfo
                {
                    Path = eventArgs.FoundedPath,
                    ItemType = eventArgs.FoundedType,
                    EventType = EventTypes.Founded
                };

                _eventInfoList.Add(searchInfo);
            }
            else
            {
                eventArgs.CancelRequested = true;
            }
        }

        /// <summary>
        /// Handler for filtered event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The event args.</param>
        private void FilteredHandler(object sender, FilteredItemEventArgs eventArgs)
        {
            var searchInfo = new SearchInfo
            {
                Path = eventArgs.FilteredPath,
                ItemType = eventArgs.FilteredType,
                EventType = EventTypes.Filtered
            };

            _eventInfoList.Add(searchInfo);

            //condition for exclude files
            if (_eventInfoList.Count > _countForExclude)
            {
                eventArgs.ExcludeItem = true;
            }
        }
    }
}
