// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.ComponentModel;
using System.Collections.Generic;
using FeedCastLibrary;
using FeedCast.Models;
using System.Net;

namespace FeedCast.ViewModels
{
    /// <summary>
    /// ViewModel for the mainpage's what's new panorama item.
    /// Add Articles here to populate that section of the UI.
    /// </summary>
    public class MainPageWhatsNewViewModel : ObservableCollection<Article>
    {
        /// <summary>
        /// ViewModel for What's New panel, handles what articles are shown & what happens when refresh is tapped. 
        /// </summary>
        public MainPageWhatsNewViewModel()
        {
            /* Suman, add the ARTICLES to this collection.
             * Just add them, dont worry about LINKING or ANYTHING to do with the UI, it is all done already.
             * See MainPageAllFeedsViewModel to add FEEDS.
             * See MainPageAllCategoriesViewModel to add CATEGORIES
             */

            Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    List<Article> articleList = App.DataBaseUtility.WhatsNewCollection(10);

                    foreach (Article a in articleList)
                    {
                        Add(a);
                    }
                });

            //Update!!
            if (!Settings.InitialLaunchSetting)
            {
                //LoadContent();
            }

        }

        /// <summary>
        /// Gets all the Feeds from the Database
        /// </summary>
        public void LoadContent(Action callback)
        {
            ContentLoader loadContent = new ContentLoader(callback);
            loadContent.LoadingFinished += DisplayFeeds;
            loadContent.DownloadFeeds(App.DataBaseUtility.GetAllFeeds());
        }



        /// <summary>
        /// Saves Articles to the Database 
        /// Displays the 20 most recent. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DisplayFeeds(object sender, ContentLoader.LoadingFinishedEventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                //If empty then just pull the 20 most recent; scenario is that the user removes all the feeds, adds some, and taps refresh. 
                if (this.Count > 0)
                {
                    List<Article> newArticles = App.DataBaseUtility.UpdateWhatsNewCollection();
                    for (int i = 0; i < newArticles.Count; i++)
                    {
                        Insert(0, newArticles[i]);
                        if (newArticles.Count >= 10 && i >= 10)
                        {
                            RemoveAt(10);
                        }
                    }
                }
                else
                {
                    foreach (Article a in App.DataBaseUtility.WhatsNewCollection(10))
                    {
                        Add(a);
                    }
                }

                (sender as ContentLoader)._callback();
            });
            
            System.Diagnostics.Debug.WriteLine("DONE");

        }
    }
}

