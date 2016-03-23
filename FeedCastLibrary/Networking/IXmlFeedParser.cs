// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.ObjectModel;
using System.Xml;

namespace FeedCastLibrary
{
    public interface IXmlFeedParser
    {
        /// <summary>
        /// Parses the provided XmlReader into a collection of Articles,
        /// taking into account properties from parentFeed.
        /// </summary>
        /// <param name="reader">The XmlReader to use to parse the article items</param>
        /// <param name="ownerFeed">The feed owner of the articles to parse</param>
        /// <returns></returns>
        Collection<Article> ParseItems(XmlReader reader, Feed ownerFeed);
    }
}
