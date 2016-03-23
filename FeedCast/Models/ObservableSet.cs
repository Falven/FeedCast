// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;

namespace FeedCast.Models
{
    class ObservableSet<T> : ObservableCollection<T> where T : IEquatable<T>
    {
        protected override void InsertItem(int index, T item)
        {
            if (!this.Contains(item))
            {
                base.InsertItem(this.Count, item);
            }
        }

        protected override void SetItem(int index, T item)
        {
            if (!this.Contains(item))
            {
                base.SetItem(index, item);
            }
        }
    }
}
