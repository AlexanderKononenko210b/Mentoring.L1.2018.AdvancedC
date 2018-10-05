using System;
using System.Collections.Generic;
using EventsHelper.Enums;
using EventsHelper.Models;
using FileSystemSearch;
using FileSystemSearch.Models;

namespace EventsHelper.Services
{
    /// <summary>
    /// Represents a model of the <see cref="Listener"/> class.
    /// </summary>
    public class Listener
    {
        private readonly int _countForCansel;
        private readonly int _countForExclude;

        private readonly List<SearchInfo> _eventList = new List<SearchInfo>();

        private int _filteredEvents;
        
        /// <summary>
        /// Initialize a new instance of the <see cref="Listener"/> class.
        /// </summary>
        /// <param name="visitor">The <see cref="FileSystemVisitor"/> instance.</param>
        /// <param name="countForCansel">The count filtered items for cansel operation.</param>
        /// <param name="countForExclude">The count filtered items for exclude directories and files from saving.</param>
        public Listener(FileSystemVisitor visitor, int countForCansel, int countForExclude)
        {
            visitor.Start += StartHandler;
            visitor.Finish += FinishHandler;
            visitor.FoundItem += FoundHandler;
            visitor.FilteredItem += FilteredHandler;

            _countForCansel = countForCansel;
            _countForExclude = countForExclude;
        }

        /// <summary>
        /// Gets or sets count collection elements.
        /// </summary>
        public int Count => _eventList.Count;

        /// <summary>
        /// Indexator for access to the collection elements.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public SearchInfo this[int index]
        {
            get
            {
                if (index < 0 || index > _eventList.Count)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(index)}");
                }

                return _eventList[index];
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

            _eventList.Add(searchInfo);
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

            _eventList.Add(searchInfo);
        }

        /// <summary>
        /// Handler for found event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The event args.</param>
        private void FoundHandler(object sender, FoundItemEventArgs eventArgs)
        {
            //condition for cansel operation
            if (_filteredEvents < _countForCansel)
            {
                var searchInfo = new SearchInfo
                {
                    Path = eventArgs.FoundPath,
                    ItemType = eventArgs.FoundType,
                    EventType = EventTypes.Found
                };

                _eventList.Add(searchInfo);
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

            _eventList.Add(searchInfo);
            _filteredEvents++;

            //condition for exclude files
            if (_filteredEvents > _countForExclude)
            {
                eventArgs.ExcludeItem = true;
            }
        }
    }
}
