// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;

namespace FeedCast.Views
{
    /// <summary>
    /// Intermediate page where the user decides if they want to add a new feed or category.
    /// </summary>
    public partial class AddMenu : PhoneApplicationPage
    {
        public AddMenu()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (e.NavigationMode == NavigationMode.New)
            {
                NavigationService.RemoveBackEntry();
            }
        }

        /// <summary>
        /// Navigate to the NewFeed page, where the user can add new feeds.
        /// </summary>
        private void OnNewFeedTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/NewFeed", UriKind.Relative));
        }

        /// <summary>
        /// Navigate to the NewCategory page, where the user can add new categories.
        /// </summary>
        private void OnNewCategoryTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/NewCategory", UriKind.Relative));
        }
    }
}