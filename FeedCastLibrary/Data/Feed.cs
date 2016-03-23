// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace FeedCastLibrary
{
    public partial class Feed : IComparable<Feed>, IEquatable<Feed>
    {
        /// <summary>
        /// Get the corresponding name group key for this feed.
        /// </summary>
        /// <param name="feed">The feed whose group key to retrieve.</param>
        /// <returns>The group key associated with this feed name.
        /// Ex: a feed called "Microsoft" returns "m".</returns>
        public string GetNameKey()
        {
            return this.FeedTitle[0].ToString().ToLower();
        }

        /// <summary>
        /// Compares this instance with the specified feed and indicates
        /// whether this instance precedes, follows, or appears in the 
        /// same position in the sort order as the specified feed.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>Less than zero if this instance precedes "other".
        /// Zero if this instance has the same position in the sort order as "other".
        /// Greater than 0 if This instance follows value or value is null.</returns>
        public int CompareTo(Feed other)
        {
            return this.FeedTitle.CompareTo(other.FeedTitle);
        }

        /// <summary>
        /// Determines whether two Feed objects have the same value.
        /// </summary>
        /// <param name="other"> The feed to compare this feed to.</param>
        /// <returns>True if this feed has the same value as "other" feed,
        /// otherwise false.</returns>
        public bool Equals(Feed other)
        {
            return this.FeedBaseURI.Equals(other.FeedBaseURI);
        }

        /// <summary>
        /// Returns a string that represents this feed.
        /// </summary>
        /// <returns>A string representation of this feed.</returns>
        public override string ToString()
        {
            return this.FeedTitle;
        }
    }
}
