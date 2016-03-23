// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;

namespace FeedCast.Models
{
    class SortedObservableSet<T> : ObservableCollection<T> where T : IComparable<T>
    {
        protected override void InsertItem(int index, T item)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].CompareTo(item) == 0)
                {
                    return;
                }

                if (this[i].CompareTo(item) > 0)
                {
                    base.InsertItem(i, item);
                    return;
                }
            }
            base.InsertItem(this.Count, item);
        }
    }
}
