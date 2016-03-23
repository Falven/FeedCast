// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.ObjectModel;
using System.ServiceModel.Syndication;
using FeedCast.Models;
using System.Diagnostics;

namespace FeedCast.ViewModels
{
    public class MainPageViewModel
    {
        public ObservableCollection<Article> WhatsNewArticles { get; set; }
        public ObservableCollection<Category> AllCategories { get; private set; }
        public ObservableCollection<Feed> AllFeeds { get; private set; }

        public MainPageViewModel()
        {
            WhatsNewArticles = new ObservableCollection<Article>();
            AllCategories = new ObservableCollection<Category>();
            AllFeeds = new ObservableCollection<Feed>();
        }
    }
}
