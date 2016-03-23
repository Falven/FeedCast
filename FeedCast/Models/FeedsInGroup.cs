// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;
using FeedCastLibrary;

namespace FeedCast.Models
{
    public class FeedsInGroup : ObservableCollection<Feed>, IComparable<FeedsInGroup>
    {
        public FeedsInGroup(string category)
        {
            Key = category;
        }

        public string Key { get; set; }

        public bool HasItems { get { return Count > 0; } }

        /// <summary>
        /// Compares this instance with the specified FeedsInGroup and indicates
        /// whether this instance precedes, follows, or appears in the 
        /// same position in the sort order as the specified FeedsInGroup.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>Less than zero if this instance precedes "other".
        /// Zero if this instance has the same position in the sort order as "other".
        /// Greater than 0 if This instance follows value or value is null.</returns>
        public int CompareTo(FeedsInGroup other)
        {
            return this.Key[0].CompareTo(other.Key[0]);
        }
    }
}
