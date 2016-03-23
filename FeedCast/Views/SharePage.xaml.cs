// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows.Navigation;
using FeedCast.ViewModels;
using Microsoft.Phone.Controls;

namespace RSS_Reader_Mockup
{
    public partial class SharePage : PhoneApplicationPage
    {
        private string URL;

        public SharePage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Retrieve the url to send.
            if (!NavigationContext.QueryString.TryGetValue("url", out URL))
                throw new ArgumentException("Could not get 'URL' query string.");
        }

        private void OnMessagingTap(object sender, EventArgs e)
        {
            if(null != URL)
                ShareUtility.ShareSMS(URL);
        }

        private void OnEmailTap(object sender, EventArgs e)
        {
            if(null != URL)
                ShareUtility.ShareEmail(URL);
        }

        private void OnSocialTap(object sender, EventArgs e)
        {
            if(null != URL)
                ShareUtility.ShareLink(URL);
        }
    }
}