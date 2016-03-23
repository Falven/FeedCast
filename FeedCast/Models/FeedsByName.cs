// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using LocalDatabase.Model;



namespace FeedCast.Models
{
    public class FeedsByName : List<FeedsInGroup>
    {
        private static readonly string Groups = "#abcdefghijklmnopqrstuvwxyz";

        public FeedsByName()
        {
            //Sorted list of feeds
            List<Feed> feedList = new List<Feed>() { new Feed{FeedTitle = "DustinTimes"}, new Feed{FeedTitle ="Engadget"}, new Feed{FeedTitle ="Gizmodo"}, new Feed{FeedTitle ="Wired"} };

            Dictionary<string, FeedsInGroup> groups = new Dictionary<string, FeedsInGroup>();

            foreach (char c in Groups)
            {
                FeedsInGroup group = new FeedsInGroup(c.ToString());
                this.Add(group);
                groups[c.ToString()] = group;
            }

            foreach (Feed f in feedList)
            {
                string s = App.ViewModel.GetFeedNameKey(f);
                groups[s].Add(f);
            }
        }
    }
}
