// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using Microsoft.Phone.BackgroundTransfer;
using System.IO.IsolatedStorage;

namespace FeedCastLibrary.Networking
{
    /// <summary>
    /// Represents a pool where backgroundtransfers are sent out as they become available.
    /// </summary>
    public class BackgroundTransferPool
    {
        /// <summary>
        /// Queue to keep track of what requests need to be sent out.
        /// </summary>
        private Queue<Uri> _requestQueue;

        /// <summary>
        /// Used to submit requests, remove requests from the queue, and retrieve active requests.
        /// </summary>
        //private BackgroundTransferService _transferService;

        /// <summary>
        /// Represents the default directory where BackgroundTransferItems are saved.
        /// </summary>
        public IsolatedStorageFile IsolatedStore { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workItems"></param>
        /// <param name="workItem"></param>
        public BackgroundTransferPool(BackgroundTransferItem[] workItems = null, BackgroundTransferItem workItem = null)
        {
            if (null != workItem)
            {
                QueueDownload(workItem);
            }

            if (null != workItems)
            {
                foreach (BackgroundTransferItem item in workItems)
                {
                    QueueDownload(item);
                }
            }

            //_transferService = new BackgroundTransferService();
        }

        /// <summary>
        /// 
        /// </summary>
        private void ResolveIsolatedStore()
        {
            if(null != IsolatedStore)
            {

            }
        }

        /// <summary>
        /// Queues the provided downloadUri to be downloaded as downloading becomes available.
        /// </summary>
        /// <param name="downloadUri">The uri resource to queue for download.</param>
        public void QueueDownload(BackgroundTransferItem workItem)
        {
            // Check if null to prevent null state.
            if (null != workItem)
            {
                Uri downloadUri = workItem.DownloadLocation;
                if (null != downloadUri)
                {
                    _requestQueue.Enqueue(downloadUri);
                }
                else
                {
                    throw new ArgumentNullException("workItem", "Invalid DownloadLocation specified.");
                }
            }
            else
            {
                throw new ArgumentNullException("workItem", "Invalid workItem specified.");
            }
        }

        protected virtual void Serialize()
        {

        }
    }

    public class BackgroundTransferItem
    {
        /// <summary>
        /// The Uri of the resource you would like to download.
        /// </summary>
        public Uri DownloadLocation { get; set; }

        public BackgroundTransferItem(Uri downloadLocation = null)
        {
            this.DownloadLocation = downloadLocation;
        }
    }
}
