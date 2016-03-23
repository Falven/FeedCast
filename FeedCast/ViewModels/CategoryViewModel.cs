// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.ObjectModel;
using FeedCast.Models;

namespace FeedCast.ViewModels
{
    public class CategoryViewModel : ObservableCollection<Category>
    {
        public CategoryViewModel()
        {
            Add(new Category { CategoryTitle = "Technology" });
            Add(new Category { CategoryTitle = "Medicine" });
            Add(new Category { CategoryTitle = "Art" });
        }
    }
}
