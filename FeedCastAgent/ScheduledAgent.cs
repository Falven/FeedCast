// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Net;
using System.Windows;
using FeedCastLibrary;
using Microsoft.Phone.Scheduler;

namespace FeedCastAgent
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        /// <summary>
        /// Tracks whether the agent is initialized or not
        /// </summary>
        private static volatile bool _classInitialized;

        /// <summary>
        /// Links the background agent to the application settings
        /// </summary>
        private static Settings AgentSettings = new Settings();
        
        /// <summary>
        /// Stores the location of the database
        /// </summary>
        private static readonly string DBLocation = "Data Source=isostore:/LocalDatabase.sdf";
        
        /// <summary>
        /// DataBaseUtility allowing the agent use with the database
        /// </summary>
        protected static DataUtils DataBaseTools { get; private set; }

        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        public ScheduledAgent()
        {
            if (!_classInitialized)
            {
                _classInitialized = true;
                // Subscribe to the managed exception handler
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    Application.Current.UnhandledException += ScheduledAgent_UnhandledException;
                });
            }

            // Initialize the database utilities.
            LocalDatabaseDataContext db = new LocalDatabaseDataContext(DBLocation);
            DataBaseTools = new DataUtils(DBLocation);
        }
        

        /// Code to execute on Unhandled Exceptions
        private void ScheduledAgent_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {
            // Run the periodic task.
            // Mutex to control web access. Only begin the process if it's free.
            List<Feed> allFeeds = DataBaseTools.GetAllFeeds();
            Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    WebTools downloader = new WebTools(new SynFeedParser());
                    downloader.SingleDownloadFinished += SendToDatabase;
                    try
                    {
                        downloader.Download(allFeeds);
                    }
                    catch (WebException) { }
                });

            // Used to quickly invoke task when debugging.
#if DEBUG_AGENT
            ScheduledActionService.LaunchForTest(task.Name, System.TimeSpan.FromSeconds(60));
#endif
            
        }

        /// <summary>
        /// Send the articles that the background agent has downloaded to the database.
        /// </summary>
        /// <param name="sender">The object that calls the event</param>
        /// <param name="e">The value returned from the event raiser</param>
        private void SendToDatabase(object sender, SingleDownloadFinishedEventArgs e)
        {
            // Make sure its not null!
            if (e.DownloadedArticles != null)
            {
                DataBaseTools.AddArticles(e.DownloadedArticles, e.ParentFeed);

            }

            // Tell the scheduler that the background agent is done.
            NotifyComplete();
        }
    }
}