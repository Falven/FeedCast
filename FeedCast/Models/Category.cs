// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace FeedCast.Models
{
    public partial class Category : IComparable<Category>, IEquatable<Category>
    {
        /// <summary>
        /// Compares this instance with the specified category and indicates
        /// whether this instance precedes, follows, or appears in the 
        /// same position in the sort order as the specified category.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>Less than zero if this instance precedes "other".
        /// Zero if this instance has the same position in the sort order as "other".
        /// Greater than 0 if This instance follows value or value is null.</returns>
        public int CompareTo(Category other)
        {
            return this.CategoryTitle.CompareTo(other.CategoryTitle);
        }

        /// <summary>
        /// Determines whether two category objects have the same value.
        /// </summary>
        /// <param name="other">The category to compare this category to.</param>
        /// <returns>True if this category has the same value as "other" category,
        /// otherwise false.</returns>
        public bool Equals(Category other)
        {
            return this.CategoryTitle.Equals(other.CategoryTitle);
        }

        /// <summary>
        /// Returns a string that represents this category.
        /// </summary>
        /// <returns>A string representation of this category.</returns>
        public override string ToString()
        {
            return this.CategoryTitle;
        }
    }
}
