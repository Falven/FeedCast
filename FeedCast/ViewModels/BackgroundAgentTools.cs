// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.
//#define DEBUG_AGENT

using System;
using Microsoft.Phone.Scheduler;

namespace FeedCast.ViewModels
{
    public class BackgroundAgentTools
    {
        private PeriodicTask periodicDownload;
        private string periodicTaskName;

        /// <summary>
        /// Constructor
        /// </summary>
        public BackgroundAgentTools()
        {
            periodicTaskName = "FeedCastAgent";
        }

        /// <summary>
        /// Creates or renews the periodic agent on the scheduler.
        /// </summary>
        /// <returns>Whether the periodic agent is created or not</returns>
        public bool StartPeriodicAgent()
        {
            periodicDownload = ScheduledActionService.Find(periodicTaskName) as PeriodicTask;
            bool wasAdded = true;

            // If the task already exists and the IsEnabled property is false, then background
            // agents have been disabled by the user.
            if (periodicDownload != null && !periodicDownload.IsEnabled)
            {
                // Can't add the agent. Return false!
                wasAdded = false;
            }

            // If the task already exists and background agents are enabled for the
            // application, then remove the agent and add again to update the scheduler.
            if (periodicDownload != null && periodicDownload.IsEnabled)
            {
                ScheduledActionService.Remove(periodicTaskName);
            }

            periodicDownload = new PeriodicTask(periodicTaskName);
            periodicDownload.Description = "Allows FeedCast to download new articles on a regular schedule.";
            ScheduledActionService.Add(periodicDownload);

            // If debugging is enabled, use LaunchForTest to launch the agent in one minute.
#if(DEBUG_AGENT)
            ScheduledActionService.LaunchForTest(periodicTaskName, TimeSpan.FromSeconds(60));
#endif
            return wasAdded;
        }
    }
}
