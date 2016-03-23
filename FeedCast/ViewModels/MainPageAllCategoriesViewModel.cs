// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.ObjectModel;
using FeedCast.Models;
using FeedCastLibrary;
using System.Collections.Generic;

namespace FeedCast.ViewModels
{
    /// <summary>
    /// ViewModel for category page; add categories here to be reflected in the 
    /// </summary>
    public class MainPageAllCategoriesViewModel : ObservableCollection<Category>
    {
        public MainPageAllCategoriesViewModel()
        {
            foreach (Category c in App.DataBaseUtility.GetAllCategories())
            {
                Add(c);
            }
        }
    }
}
