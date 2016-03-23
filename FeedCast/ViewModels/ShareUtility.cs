// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using Microsoft.Phone.Tasks;

namespace FeedCast.ViewModels
{
    public static class ShareUtility
    {
        // The body for each message.
        private static readonly string _messageTitle = "Cool article";
        private static readonly string _messageBody = "Hey, check out this article: ";

        public static void ShareLink(string link)
        {
            ShareLinkTask shareLinkTask = new ShareLinkTask();
            shareLinkTask.Title = _messageTitle;
            shareLinkTask.Message = _messageBody + link;
            shareLinkTask.LinkUri = new Uri(link, UriKind.Absolute);
            shareLinkTask.Show();
        }

        public static void ShareSMS(string link)
        {
            SmsComposeTask smsComposeTask = new SmsComposeTask();
            smsComposeTask.Body = _messageBody + link;
            smsComposeTask.Show();
        }

        public static void ShareEmail(string link)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();
            emailComposeTask.Subject = _messageTitle;
            emailComposeTask.Body = _messageBody + link;
            emailComposeTask.Show();
        }

        public static void LaunchBrowser(string link)
        {
            WebBrowserTask webBrowserTask = new WebBrowserTask();
            webBrowserTask.Uri = new Uri(link, UriKind.Absolute);
            webBrowserTask.Show();
        }
    }
}
