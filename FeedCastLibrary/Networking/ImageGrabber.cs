// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ServiceModel.Syndication;
using System;

namespace FeedCastLibrary
{
    /// <summary>
    /// Helper class to obtain the images for feeds.
    /// </summary>
    public static class ImageGrabber
    {
        /// <summary>
        /// String array holding all of the default image locations
        /// </summary>
        private static string[] DefaultImages = { 
                                                     "/Images/FeedCastImg0.jpg",
                                                     "/Images/FeedCastImg1.jpg",
                                                     "/Images/FeedCastImg2.png",
                                                     "/Images/FeedCastImg3.jpg",
                                                     "/Images/FeedCastImg4.jpg",
                                                     "/Images/FeedCastImg5.jpg",
                                                     "/Images/FeedCastImg6.png",
                                                     "/Images/FeedCastImg7.jpg"
                                                 };
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
            // If there's no image, set it to a default one.
            if (!imageIsOkay)
            {
                feed.ImageURL = GetDefaultImage();
            }
            return imageIsOkay;
        }

        /// <summary>
        /// Randomly chooses a default image.
        /// </summary>
        /// <returns>Returns the string location to a default image</returns>
        public static string GetDefaultImage()
        {
            Random random = new Random();
            return (DefaultImages[random.Next(0, 7)]);
        }
    }
}
