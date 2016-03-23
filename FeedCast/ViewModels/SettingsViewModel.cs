// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using FeedCast.Models;

namespace FeedCast.ViewModels
{
    public class SettingsViewModel
    {
        public Settings AppSettings { get; set; }

        public SettingsViewModel()
        {
            AppSettings = new Settings();
        }
    }
}
