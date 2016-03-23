// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Xml;
using Microsoft.Phone.Net.NetworkInformation;
using System.Threading;

namespace FeedCast.Models
{
    /// <summary>
    /// Handles downloading of feeds and search results.
    /// </summary>
    public sealed class WebTools : DependencyObject
    {
        /// <summary>
        /// Dictionary that stores all the downloaded feeds to a list of their respective articles.
        /// </summary>
        public IDictionary<Feed, ICollection<Article>> Downloads { get; private set; }

        /// <summary>
        /// Parser used to parse SyndicationItems into Articles.
        /// </summary>
        private IXmlFeedParser _parser;

        /// <summary>
        /// Object used to lock an action.
        /// </summary>
        private object _lockObject;

        /// <summary>
        /// Number of requests remaining to return.
        /// </summary>
        private int _numOfRequests;

        /// <summary>
        /// Specifies if this WebTools Object is currently downloading.
        /// This could have changed by the time you checked.
        /// </summary>
        public bool IsDownloading
        {
            get { return (bool)GetValue(IsDownloadingProperty); }
            set
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        SetValue(IsDownloadingProperty, value);
                    });
            }
        }

        /// <summary>
        /// The IsDownloading Dependency Property that specifies if this WebTools Object is currently downloading.
        /// </summary>
        public static readonly DependencyProperty IsDownloadingProperty =
            DependencyProperty.Register(
            "IsDownloading",
            typeof(bool),
            typeof(WebTools),
            new PropertyMetadata(false));


        #region Events
        /// <summary>
        /// Event raised after all feed downloads have finished.
        /// </summary>
        public event EventHandler<AllDownloadsFinishedEventArgs> AllDownloadsFinished;

        /// <summary>
        /// Event raised after each feed download has finished.
        /// </summary>
        public event EventHandler<SingleDownloadFinishedEventArgs> SingleDownloadFinished;

        /// <summary>
        /// Event raised when a download fails.
        /// </summary>
        public event EventHandler<DownloadFailedEventArgs> DownloadFailed;
        #endregion

        public WebTools(IXmlFeedParser parser)
        {
            if (null != parser)
            {
                _parser = parser;
            }
            else
            {
                throw new ArgumentNullException("No parser");
            }

            Downloads = new Dictionary<Feed, ICollection<Article>>();

            _lockObject = new object();
        }

        /// <summary>
        /// Downloads the provided Feed.
        /// You cannot call download while downloads are in progress.
        /// </summary>
        /// <param name="feed">feed to download.</param>
        public void Download(Feed feed)
        {
            if (null != feed)
            {
                Download(new List<Feed> { feed });
            }
        }

        /// <summary>
        /// Downloads the provided List of Feeds.
        /// You cannot call download while downloads are in progress.
        /// </summary>
        /// <param name="feeds">List of feeds to download.</param>
        public void Download(IList<Feed> feeds)
        {
            if (null != feeds && feeds.Count > 0)
            {
                _numOfRequests = feeds.Count;
                IsDownloading = true;

                // Download each separate feed
                foreach (Feed feed in feeds)
                {
                    string feedURI = feed.FeedBaseURI;
                    if (null != feedURI)
                    {
                        try
                        {
                            HttpWebRequest feedRequest = HttpWebRequest.Create(feedURI) as HttpWebRequest;

                            // If the user has 
                            if (App.ApplicationSettings.WifionlySetting)
                            {
                                try
                                {
                                    feedRequest.SetNetworkRequirement(NetworkSelectionCharacteristics.NonCellular);
                                }
                                catch (NetworkException ne)
                                {
                                    if (null != DownloadFailed)
                                    {
                                        DownloadFailed(this, new DownloadFailedEventArgs(ne)
                                        { 
                                            WifiException = (ne.NetworkErrorCode == NetworkError.NetworkSelectionRequirementFailed)
                                        });
                                    }
                                    return;
                                }
                            }
                            if (null != feedRequest)
                            {
                                RequestState feedState = new RequestState()
                                    {
                                        // Change the owner to the parent HTTPWebRequest.
                                        Request = feedRequest,
                                        // Change the argument to be the current feed to be downloaded.
                                        Argument = feed,
                                    };

                                // Begin download.
                                feedRequest.BeginGetResponse(ResponseCallback, feedState);
                            }
                        }
                        catch (WebException we)
                        {
                            if (null != DownloadFailed)
                            {
                                DownloadFailed(this, new DownloadFailedEventArgs(we)
                                {
                                    BadConnectionException = true
                                });
                            }
                            return;
                        }
                    }
                }
            }
            else
            {
                throw new ArgumentNullException("No feeds");
            }
        }

        /// <summary>
        /// Callback method called when the "Download" method returns from an HTTPWebRequest.
        /// </summary>
        /// <param name="result">The result of the asynchronous operation.</param>
        private void ResponseCallback(IAsyncResult result)
        {
            RequestState state = (RequestState)result.AsyncState;
            try
            {
                HttpWebRequest request = state.Request as HttpWebRequest;
                if (null != request)
                {
                    // Retrieve response.
                    using (HttpWebResponse response = request.EndGetResponse(result) as HttpWebResponse)
                    {
                        if (null != response && response.StatusCode == HttpStatusCode.OK)
                        {
                            using (XmlReader reader = XmlReader.Create(response.GetResponseStream()))
                            {
                                Feed parentFeed = state.Argument as Feed;

                                if (null != parentFeed)
                                {
                                    // Collection to store all articles
                                    Collection<Article> parsedArticles = _parser.ParseItems(reader, parentFeed);

                                    // Raise event for a single feed downloaded.
                                    if (null != SingleDownloadFinished)
                                    {
                                        SingleDownloadFinished(this, new SingleDownloadFinishedEventArgs(parentFeed, parsedArticles));
                                    }

                                    // Add to all downloads dictionary and raise AllDownloadsFinished if all async requests have finished.
                                    Downloads.Add(parentFeed, parsedArticles);

                                    lock (_lockObject)
                                    {
                                        IsDownloading = ((--_numOfRequests) > 0);
                                    }
                                    if (_numOfRequests <= 0 && null != AllDownloadsFinished)
                                    {
                                        AllDownloadsFinished(this, new AllDownloadsFinishedEventArgs(Downloads));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (WebException we)
            {
                if (null != DownloadFailed)
                {
                    DownloadFailed(this, new DownloadFailedEventArgs(we)
                    {
                        BadConnectionException = true
                    });
                }
                return;
            }
            catch (XmlException e)
            {
                if (null != DownloadFailed)
                {
                    DownloadFailed(this, new DownloadFailedEventArgs(e));
                }
                return;
            }
        }
    }

    /// <summary>
    /// Class that passes data across asynchronous calls.
    /// </summary>
    public class RequestState
    {
        /// <summary>
        /// The owner of this request state.
        /// </summary>
        public WebRequest Request { get; set; }

        /// <summary>
        /// The argument for this request.
        /// </summary>
        public object Argument { get; set; }

        /// <summary>
        /// Creates a new, empty request state with default porperty values.
        /// Request = null
        /// Argument = null
        /// AddToDatabase = null
        /// AddToUI = null
        /// </summary>
        public RequestState()
        {
            this.Request = null;
            this.Argument = null;
        }

        /// <summary>
        /// Creates an instance of the webrequest class with given parameters.
        /// </summary>
        /// <param name="argument">The argument for this request.</param>
        /// <param name="addToDatabase">Boolean denoting whether to add the argument to the database.</param>
        /// <param name="addToUI">Boolean denoting whether to add the argument to the UI.</param>
        public RequestState(object argument)
        {
            this.Argument = argument;
        }

        /// <summary>
        /// Creates an instance of the webrequest class with given parameters.
        /// </summary>
        /// <param name="owner">The owner of this request state.</param>
        /// <param name="argument">The argument for this request.</param>
        /// <param name="addToDatabase">Boolean denoting whether to add the argument to the database.</param>
        /// <param name="addToUI">Boolean denoting whether to add the argument to the UI.</param>
        public RequestState(WebRequest owner, object argument)
            : this(argument)
        {
            this.Request = owner;
        }
    }

    /// <summary>
    /// Event arguments passed when all Feed downloads have finished.
    /// </summary>
    public class AllDownloadsFinishedEventArgs : EventArgs
    {
        /// <summary>
        /// A dictionary of Downloaded feeds to a list of its respective downloaded articles.
        /// </summary>
        public IDictionary<Feed, ICollection<Article>> Downloads { get; set; }

        /// <summary>
        /// Creates a new instance of DownloadFinishedEventArgs which
        /// provides arguments relevant to the download performed.
        /// </summary>
        public AllDownloadsFinishedEventArgs() : this(null) { }

        /// <summary>
        /// Creates a new instance of DownloadFinishedEventArgs which
        /// provides arguments relevant to the download performed.
        /// </summary>
        /// <param name="downloads">A dictionary of Downloaded feeds 
        /// to a list of its respective downloaded articles.</param>
        public AllDownloadsFinishedEventArgs(IDictionary<Feed, ICollection<Article>> downloads)
            : base()
        {
            this.Downloads = downloads;
        }
    }

    /// <summary>
    /// Event argument passed when a single Feed download has finished.
    /// Warning, this event is raised many times (once per feed downloaded).
    /// </summary>
    public class SingleDownloadFinishedEventArgs : EventArgs
    {
        /// <summary>
        /// The feed whose articles were downloaded.
        /// </summary>
        public Feed ParentFeed { get; set; }

        /// <summary>
        /// Articles corresponding to ParentFeed that were downloaded.
        /// </summary>
        public IList<Article> DownloadedArticles { get; set; }

        /// <summary>
        /// Creates a new instance of SingleDownloadFinishedEventArgs which
        /// provides arguments relevant to the download performed.
        /// </summary>
        public SingleDownloadFinishedEventArgs() : this(null, null) { }

        /// <summary>
        /// Creates a new instance of SingleDownloadFinishedEventArgs which
        /// provides arguments relevant to the download performed.
        /// </summary>
        /// <param name="downloadedFeeds">The feed whose articles were downloaded.</param>
        /// <param name="downloadedArticles">Articles corresponding to ParentFeed that were downloaded.</param>
        public SingleDownloadFinishedEventArgs(Feed parentFeed, IList<Article> downloadedArticles)
        {
            this.ParentFeed = parentFeed;
            this.DownloadedArticles = downloadedArticles;
        }
    }

    /// <summary>
    /// Event arguments passes when a Webtools download has failed.
    /// </summary>
    public class DownloadFailedEventArgs : EventArgs
    {
        /// <summary>
        /// The Exception that caused the download to fail.
        /// </summary>
        public Exception ExceptionThrown { get; set; }

        /// <summary>
        /// States if the exception indicates that the user has no wifi.
        /// </summary>
        public bool WifiException { get; set; }

        /// <summary>
        /// States if the exception indicates has no/poor connection.
        /// </summary>
        public bool BadConnectionException { get; set; }

        /// <summary>
        /// Creates a new instance of DownloadFinishedEventArgs which
        /// provides arguments relevant to the download performed.
        /// </summary>
        public DownloadFailedEventArgs() : this(null) { }

        /// <summary>
        /// Creates a new instance of DownloadFinishedEventArgs which
        /// provides arguments relevant to the download performed.
        /// </summary>
        /// <param name="downloadedFeed">The feed that was downloaded.</param>
        /// <param name="downloadedArticles">A List of articles
        /// downloaded for the specified feed.</param>
        public DownloadFailedEventArgs(Exception exceptionThrown)
            : base()
        {
            this.ExceptionThrown = exceptionThrown;
            this.WifiException = false;
            this.BadConnectionException = false;
        }
    }
}
