using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ServiceModel.Syndication;

namespace FeedCast.Models
{
    /// <summary>
    /// Helper class to obtain the images for feeds.
    /// </summary>
    public static class ImageGrabber
    {
        /// <summary>
        /// Determines whether or not the imageURL field of a Feed is empty or not.
        /// </summary>
        /// <param name="feed">The feed to be looked at</param>
        /// <returns>Whether or not the imageURL is empty or not</returns>
        public static bool IfImageExists(Feed feed)
        {
            return (feed.ImageURL != null);
        }

        /// <summary>
        /// Determines whether or not the imageURL field of a SyndicationFeed is empty or not.
        /// </summary>
        /// <param name="feed">The SyndicationFeed to be looked at</param>
        /// <returns>Whether or not the imageURL is empty or not</returns>
        public static bool IfImageExists(SyndicationFeed synFeed)
        {
            return (synFeed.ImageUrl != null);
        }

        /// <summary>
        /// Gets the image url for a feed. If there is no usable one, then use a default image.
        /// </summary>
        /// <param name="feed">The feed to get the image for</param>
        /// <param name="synFeed">The SyndicationFeed to obtain data from</param>
        /// <returns>Whether or not the feed had an obtainable image</returns>
        public static bool GetImage(Feed feed, SyndicationFeed synFeed)
        {
            bool imageIsOkay = false;

            if (IfImageExists(synFeed))
            {
                string url = synFeed.ImageUrl.ToString();

                // Make sure that an image exists in the right filetype.
                if (url.EndsWith("png") || url.EndsWith("jpg") || url.EndsWith("jpeg"))
                {
                    feed.ImageURL = url;
                    imageIsOkay = true;
                }
            }
            // If there's no image, set it to the default one.
            feed.ImageURL = "/Images/FeedCastDefault.jpg";
            return imageIsOkay;
        }
    }
}
