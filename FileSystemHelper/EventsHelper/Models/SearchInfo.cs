using EventsHelper.Enums;
using FileSystemSearch.Enums;

namespace EventsHelper.Models
{
    /// <summary>
    /// Represents a model of the <see cref="SearchInfo"/> class.
    /// </summary>
    public class SearchInfo
    {
        /// <summary>
        /// Gets or sets search path.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets item type.
        /// </summary>
        public SearchItems ItemType { get; set; }

        /// <summary>
        /// Gets or sets event type.
        /// </summary>
        public EventTypes EventType { get; set; }
    }
}
