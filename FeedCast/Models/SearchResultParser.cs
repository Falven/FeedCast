using System.Collections.ObjectModel;
using System.Xml;

namespace FeedCast.Models
{
    public class SearchResultParser : IXmlFeedParser
    {
        /// <summary>
        /// Object to moderate thread access to collection adding.
        /// </summary>
        private static readonly object _lockObject = new object();

        /// <summary>
        /// Default constructor
        /// </summary>
        public SearchResultParser() { }

        /// <summary>
        /// Parse the results of the Bing search
        /// </summary>
        /// <param name="reader">The xml reader containing the search results</param>
        public Collection<Article> ParseItems(XmlReader reader, Feed ownerFeed)
        {
            Collection<Article> results = new Collection<Article>();
            reader.ReadToFollowing("item");
            do
            {
                if (reader.ReadToFollowing("title"))
                {
                    string name = reader.ReadElementContentAsString();

                    if (reader.ReadToFollowing("link"))
                    {
                        string uri = reader.ReadElementContentAsString();
                        Article newResult = new Article
                        {
                            ArticleTitle = name,
                            ArticleBaseURI = uri
                        };
                        // Safely add the search result to the collection.
                        lock (_lockObject)
                        {
                            results.Add(newResult);
                        }
                    }
                }
            } while (reader.ReadToFollowing("item"));
            return results;
        }
    }
}
