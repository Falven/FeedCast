using System.Collections.ObjectModel;
using FeedCastLibrary;

namespace FeedCast.ViewModels
{
    public class CategoryPageViewModel : ObservableCollection<Article>
    {
        public Category Category { get; set; }

        public CategoryPageViewModel(int category)
        {
            Category = App.DataBaseUtility.QueryCategory(category);

            foreach (Article a in App.DataBaseUtility.GetCategoryArticles(category))
            {
                Add(a);   
            }
        }
    }
}
