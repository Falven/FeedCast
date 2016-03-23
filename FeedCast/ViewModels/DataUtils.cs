// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

//---------------------------------------------------------------------------------------------------
// This class is used to initiate the database and modify the data in it.
// The methods include: SaveChangesToDB(), NotifyPropertyChanged(string propertyName),
// QueryForWhatsNew(), unReadNumberForACategory(Category category)
// unReadNumberForAFeed(Feed feed), AddCategory(Category newCategory),
// AddFeed(Feed newFeed, Category_Feed newCat_Feed), AddArticle(Article newArticle), AddImage(Image newImage)
// DeleteFeed(Feed deleteFeed, Category_Feed deleteCat_Feed), DeleteArticle(Article deleteArticle),
// DeleteImage(Image deleteImage), QueryCategory(string categoryTitle), QueryFeed(string feedTitle),
// QueryArticle(string articleTitle), QueryImage(string imageURL) (not updated :( )
// This class also has four ObservableCollections, one for categories, one for feed,
// one for articles, and one for images.
//---------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using FeedCast.Models;
using System;
using System.Threading;
using System.Windows;
using System.Data.Linq;

namespace FeedCast.ViewModels
{
    public class DataUtils : INotifyPropertyChanged
    {

        //Database!
        private LocalDatabaseDataContext db;

        private Mutex dbMutex = new Mutex(false, "DBControl");

        //Contructor to initialize the database
        public DataUtils(string dbConnectionString)
        {
            db = new LocalDatabaseDataContext(dbConnectionString);
        }

        //Used when the database is modified.
        public void SaveChangesToDB()
        {
            try
            {

                dbMutex.WaitOne();
                db.SubmitChanges();
                dbMutex.ReleaseMutex();
            }
            catch (ChangeConflictException e)
            {
                System.Diagnostics.Debug.WriteLine("ERROR!!!: " + e);
                return;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("ERROR!!!: " + e);
            }
        }

        //Notify Silverlight to update the UI 
        // if the database changes
        #region INotifyProperyChangedMembers

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

       
        #region Featured
        /// <summary>
        /// Queries espeically for Featured. 
        /// </summary>
        public List<Feed> QueryForUnreadFeeds()
        {
            // Query the database for feeds with unread articles.
            var c = from Feed q in db.Feed
                    where q.UnreadCount != 0
                    orderby q.ViewCount descending
                    select q;
            return c.ToList();
        }

        public List<Article> QueryForSortedFeed(List<Feed> c, int[] maxThresIndexes, int index)
         {

            // Query for the newest article of the chosen feed.
            var a = from Article o in db.Article
                    where o.FeedID == c[maxThresIndexes[index]].FeedID
                    where o.Read == false
                    orderby o.PublishDate descending
                    select o;
            return a.ToList();

        }
        #endregion



               //Check if the feed is there before adding
        public bool checkFeed(string feedURI, int catID)
        {
            bool result;
            List<Category_Feed> cat_feed = new List<Category_Feed>();
            var c = from q in db.Feed
                    where q.FeedBaseURI == feedURI
                    select q;

            foreach (Feed f in c)
            {
                var d = from q in db.Category_Feed
                        where q.CategoryID == catID
                        where q.FeedID == f.FeedID
                        select q;
                cat_feed = d.ToList();
            }
            result = (cat_feed.Count() == 0);

            return result;
        }

        //Check if the feed is there before adding
        public bool checkIfFeedExists(int feedID)
        {
            List<Feed> feed = new List<Feed>();
            var c = from q in db.Feed
                    where q.FeedID == feedID
                    select q;

            return (feed.Count() == 0);
        }

        //Intial List of What's New articles
        public List<Article> WhatsNewCollection(int initialCount)
        {
            List<Article> w = new List<Article>();
            var whatsnew = from Article q in db.Article
                           //where (q.PublishDate).Value.CompareTo(DateTime.Now) < 0
                           where q.PublishDate.Value.AddDays(3).CompareTo(DateTime.Now) > 0
                           orderby q.PublishDate descending
                           select q;

            w = whatsnew.ToList();

            
            if (w.Count != 0)
            {
                if (w.Count >= 20)
                {
                    w.RemoveRange(initialCount, w.Count() - initialCount);
                    SynFeedParser.latestDate = w[0].PublishDate.Value;
		}
            }
            return w;
        }


        //Articles that have been downloaded since the last refresh
        public List<Article> UpdateWhatsNewCollection()
        {
            List<Article> w = new List<Article>();
            var whatsnew = from Article q in db.Article
                           //where (q.PublishDate).Value.CompareTo(DateTime.Now) < 0
                           where q.PublishDate.Value.CompareTo(SynFeedParser.latestDate) > 0
                           orderby q.PublishDate ascending
                           select q;
            
            w = whatsnew.ToList();
           
            if (w.Count() != 0 && whatsnew != null)
            {
                SynFeedParser.latestDate = w[0].PublishDate.Value;
            }
            return w;
        }



        //When the user hits the bottom, the application is supposed to load more articles
        public List<Article> NextWhatsNewCollection(int count)
        {
            List<Article> w = new List<Article>();
            var whatsnew = from Article q in db.Article
                           where (q.PublishDate).Value.CompareTo(DateTime.Now) < 0
                           where q.PublishDate.Value.AddDays(1).CompareTo(DateTime.Now) > 0
                           orderby q.PublishDate descending
                           select q;

            w = whatsnew.ToList();
            

            if (w.Count > 0)
            {
                w.RemoveRange(0, count);
                if (w.Count > count)
                {
                    w.RemoveRange(count, w.Count() - count);
                }
            }
            return w;
        }
        
        //For the long list selector
        public string GetFeedNameKey(Feed f)
        {
            char key = char.ToLower(f.FeedTitle[0]);
            if (key < 'a' || key > 'z')
            {
                key = '#';
            }
            return key.ToString();
        }

        //Add a category to the category table
        public bool AddCategory(Category newCategory)
        {
            dbMutex.WaitOne();
            var c = from q in db.Category
                    where q.CategoryTitle == newCategory.CategoryTitle
                    select q;

            bool checkCategory = (c.Count() == 0);

            if (checkCategory)
            {

                db.Category.InsertOnSubmit(newCategory);

                //db.SubmitChanges();
                
            }

            dbMutex.ReleaseMutex();

            return checkCategory;
        }

        //Add a feed to the feed table, and a corresponding entry in Category_Feed Table
        public bool AddFeed(Feed newFeed, Category cat)
        {
            dbMutex.WaitOne();
            bool checkfeed = checkFeed(newFeed.FeedBaseURI, cat.CategoryID);
            bool feedexists = checkIfFeedExists(newFeed.FeedID);
            //Checks if the feed is in the database
            if (feedexists)
            {
                newFeed.SharedCount = 0;
                newFeed.UnreadCount = 0;
                newFeed.ViewCount = 0;
                newFeed.FavoritedCount = 0;
                db.Feed.InsertOnSubmit(newFeed);
                db.SubmitChanges();

                Category_Feed newCat_Feed = new Category_Feed { CategoryID = cat.CategoryID, FeedID = newFeed.FeedID };
                db.Category_Feed.InsertOnSubmit(newCat_Feed);
                db.SubmitChanges(); 
            }
            //If it does, but not to the category, add it to the category
            else if (checkfeed)
            {
                Category_Feed newCat_Feed = new Category_Feed { CategoryID = cat.CategoryID, FeedID = newFeed.FeedID };
                db.Category_Feed.InsertOnSubmit(newCat_Feed);
                db.SubmitChanges(); 
            }
            //Else do nothing.
            dbMutex.ReleaseMutex();
            return (checkfeed || feedexists);
        }

        public void AddCat_Feed(int newFeed, int cat)
        {
            Category_Feed newCat_Feed = new Category_Feed { CategoryID = cat, FeedID = newFeed };
            db.Category_Feed.InsertOnSubmit(newCat_Feed);
            db.SubmitChanges();

        }       

        //Add an article to the article table!
        public bool AddArticle(Article newArticle)
        {

            dbMutex.WaitOne();
            var c = from q in db.Article
                    where q.ArticleBaseURI == newArticle.ArticleBaseURI
                    select q;

            Feed feed = QueryFeed(Convert.ToInt32(newArticle.FeedID));



            var d = from q in db.Article
                    where q.ArticleBaseURI == newArticle.ArticleBaseURI
                    select q;


            bool checkArticle = (d.Count() == 0);

            if (checkArticle)
            {
                newArticle.Read = false;
                newArticle.Favorite = false;
                //feed.UnreadCount++;
                db.Article.InsertOnSubmit(newArticle);
               // db.SubmitChanges();
            }

            dbMutex.ReleaseMutex();

            return checkArticle;
        }

        //Corner Case (if the feed doesnt belong to any other category)
        public bool checkDelete(Feed feed)
        {
            var d = from o in db.Category_Feed
                    where o.FeedID == feed.FeedID
                    select o;
            return (d.Count() == 1);
        }

        //Delete a category.
        public void DeleteCategory(Category deleteCategory)
        {
            
            var q = from o in db.Category_Feed
                    where o.CategoryID == deleteCategory.CategoryID
                    select o;
            foreach (Category_Feed deleteCat_Feed in q)
            {
                Feed feed = QueryFeed(Convert.ToInt32(deleteCat_Feed.FeedID));
                if (checkDelete(feed))
                {
                    DeleteFeed(feed);
                }
                db.Category_Feed.DeleteOnSubmit(deleteCat_Feed);
            }

            db.Category.DeleteOnSubmit(deleteCategory);
            db.SubmitChanges();
        }

        //Delete a feed.
        public void DeleteFeed(Feed deleteFeed)
        {
            var q = from o in db.Category_Feed
                    where o.FeedID == deleteFeed.FeedID
                    select o;
            foreach (Category_Feed deleteCat_Feed in q)
            {
                db.Category_Feed.DeleteOnSubmit(deleteCat_Feed);
            }

            var a = from o in db.Article
                    where o.FeedID == deleteFeed.FeedID
                    select o;

            foreach(Article art in a)
            {
                db.Article.DeleteOnSubmit(art);
            }

            db.Feed.DeleteOnSubmit(deleteFeed);
            db.SubmitChanges();
        }

        //Delete an article.
        public void DeleteArticle(Article deleteArticle)
        {
            
            db.Article.DeleteOnSubmit(deleteArticle);
            db.SubmitChanges();
        }

       
        //Query for a particular category
        public Category QueryCategory(int category)
        {

            var c = from Category q in db.Category
                    where q.CategoryID == category
                    select q;

            return c.Take(1).Single();
        }

        //Query for a particular category
        public Category QueryCategory(string category)
        {

            var c = from Category q in db.Category
                    where q.CategoryTitle == category
                    select q;

            return c.Take(1).Single();
            
        }

        //Query for a particular feed
        public Feed QueryFeed(string feedTitle)
        {

            var c = from Feed q in db.Feed
                    where q.FeedTitle == feedTitle
                    select q;

            return c.Take(1).Single();
        }

        //Query for a particular feed
        public Feed QueryFeed(int feed)
        {

            var c = from Feed q in db.Feed
                    where q.FeedID == feed
                    select q;

            return c.Take(1).Single();
        }

        //Query for a particular article
        public Article QueryArticle(int articleID)
        {
            var c = from Article q in db.Article
                    where q.ArticleID == articleID
                    select q;

            return c.Take(1).Single();
            
        }

     
        //Get all the Categories
        public List<Category> GetAllCategories()
        {
            List<Category> cat;
            var c = from q in db.Category
                   // orderby q.CategoryTitle ascending
                    select q;
            cat = c.ToList();
            return cat;
        }

        //Get all the Feeds
        public List<Feed> GetAllFeeds()
        {
            dbMutex.WaitOne();
            List<Feed> f;
            var c = from q in db.Feed
                    select q;
            f = c.ToList();
            dbMutex.ReleaseMutex();
            return f;
        }

        //Get all the Articles no filter
        public List<Article> GetAllArticles()
        {
            List<Article> a;
            var c = from q in db.Article
                    orderby q.PublishDate descending
                    select q;
            a = c.ToList();
            return a;
        }

        //Get all the Articles filtered by feed
        public List<Article> GetFeedArticles(int feedID)
        {
            List<Article> a;
            var q = from Article o in db.Article
                    where o.FeedID == feedID
                    orderby o.PublishDate descending
                    select o;

            a = q.ToList();

            return a;
        }

 

        //Get all the Articles filtered by Category
        public List<Article> GetCategoryArticles(int categoryID)
        {
            
            List<Article> cat = new List<Article>();
            //Get the corresponding Cat_Feeds with the category ID
             var q = from o in db.Category_Feed
                    where o.CategoryID == categoryID
                    select o;

            //Favorites?
             if (categoryID == 1)
             {
                 //For every feed corresponding to the category, 
                 foreach (Category_Feed f in q)
                 {
                     //Get each article
                     foreach (Article a in GetFeedArticles(Convert.ToInt32(f.FeedID)))
                     {
                         //If favorited, add
                         if (Convert.ToBoolean(a.Favorite))
                         {
                             cat.Add(a);
                         }
                     }
                 }
             }

             else
             {
                 //Not Favorites!
                 foreach (Category_Feed f in q)
                 {
                     //For every Cat_Feed get articles
                     foreach (Article a in GetFeedArticles(Convert.ToInt32(f.FeedID)))
                     {
                         cat.Add(a);
                     }
                 }
             }
            
            //Return list.
            return cat;
        }

        public void PrintOutput()
        {
            List<Article> a;
            List<Feed> f;
            List<Category> c;
            var o = from q in db.Article
                    select q;
            a = o.ToList();
            var l = from q in db.Feed
                    select q;
            f = l.ToList();
            var m = from q in db.Category
                    select q;
            c = m.ToList();

            foreach (Article article in a)
            {
                System.Diagnostics.Debug.WriteLine(article.ArticleTitle);
                System.Diagnostics.Debug.WriteLine(article.ArticleID);
            }

            foreach (Feed feed in f)
            {
                System.Diagnostics.Debug.WriteLine(feed.FeedTitle);
                System.Diagnostics.Debug.WriteLine(feed.FeedID);
            }

            foreach (Category category in c)
            {
                System.Diagnostics.Debug.WriteLine(category.CategoryTitle);
                System.Diagnostics.Debug.WriteLine(category.CategoryID);
            }
        
        }
  

    }
}
