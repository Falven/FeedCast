// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Threading;
using FeedCastLibrary;

namespace FeedCast.Models
{
    /// <summary>
    /// Represents a category in the InitialLaunchPage used to initially populated the RSSReader.
    /// </summary>
    public class InitialCategory : Category
    {
        /// <summary>
        /// A comma-separated string of the initial feeds to be loaded with this InitialCategory.
        /// </summary>
        public string AssociatedFeeds { get; private set; }

        /// <summary>
        /// A list of the feeds to be loaded with this InitialCategory.
        /// </summary>
        public IList<Feed> Feeds { get; private set; }

        /// <summary>
        /// Constructs an InitialCategory with the provided parameters.
        /// </summary>
        /// <param name="title">The title of this InitialCategory.</param>
        /// <param name="feeds">One or more feeds to load when this InitialCategory is loaded.</param>
        public InitialCategory(string title, params Feed[] feeds) : base()
        {
            CategoryTitle = title;
            Feeds = new List<Feed>(feeds);

            // Building up AssociatedFeeds.
            StringBuilder sb = new StringBuilder();
            string separator = ", ";
            foreach (Feed f in feeds)
            {
                sb.Append(f.FeedTitle).Append(separator);
            }
            sb.Length -= separator.Length;
            AssociatedFeeds = sb.ToString();
        }
    }
}
