using System.Collections.ObjectModel;
using System.ServiceModel.Syndication;
using FeedCast.Models;

namespace FeedCast.ViewModels
{
    public class WhatsNewViewModel : ObservableCollection<Article>
    {
        public WhatsNewViewModel()
        {
            for (int i = 1; i <= 100; i++)
            {
                Add(new Article()
                {
                    Title = new TextSyndicationContent("Article title " + i),
                    Author = "Author " + i,
                    Summary = new TextSyndicationContent("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Praesent mi quam, condimentum eget sodales et, bibendum sit amet dui. Etiam sagittis dui vel massa ultrices sed laoreet elit consequat. Curabitur nunc lorem, accumsan vel cursus a, suscipit eget purus. Donec ornare nunc sed nunc accumsan quis suscipit nisl semper.")
                });
            }
        }
    }
}
