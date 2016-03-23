using System.Collections.ObjectModel;
using FeedCastLibrary;

namespace FeedCast.ViewModels
{
    public class FeedPageViewModel : ObservableCollection<Article>
    {
        public Feed Feed { get; set; }

        public FeedPageViewModel(int feed)
        {
            Feed = App.DataBaseUtility.QueryFeed(feed);

            foreach (Article a in App.DataBaseUtility.GetFeedArticles(feed))
            {
                Add(a);
            }

        }
    }
}
