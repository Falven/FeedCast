// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using FeedCast.Models;
using FeedCastLibrary;

namespace FeedCast.ViewModels
{
    public class MainPageAllFeedsViewModel : ObservableCollection<FeedsInGroup>
    {
        /// <summary>
        /// All the groups for the long list selector.
        /// </summary>
        private static readonly string Groups = "#abcdefghijklmnopqrstuvwxyz";

        /// <summary>
        /// A dictionary of group letter to feeds in such group.
        /// </summary>
        private Dictionary<string, FeedsInGroup> _groupings;

        public MainPageAllFeedsViewModel()
        {
            _groupings = new Dictionary<string, FeedsInGroup>();

            foreach (char c in Groups)
            {
                FeedsInGroup group = new FeedsInGroup(c.ToString());
                this.Add(group);
                _groupings[c.ToString()] = group;
            }

            List<Feed> feeds = App.DataBaseUtility.GetAllFeeds();
            foreach (Feed f in feeds)
            {
                AddFeed(f);
            }
        }
        //Where do you reference the database? Where are you getting the feeds from?   
        public void AddFeed(Feed feed)
        {
            _groupings[feed.GetNameKey()].Add(feed);
        }

        public void RemoveFeed(Feed feed)
        {
            _groupings[feed.GetNameKey()].Remove(feed);
        }

        public bool ContainsFeed(Feed feed)
        {
            return _groupings[feed.GetNameKey()].Contains(feed);
        }
    }
}
