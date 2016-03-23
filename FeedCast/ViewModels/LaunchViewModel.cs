// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.ObjectModel;
using FeedCast.Models;

namespace FeedCast.ViewModels
{
    public class LaunchViewModel : ObservableCollection<InitialCategory>
    {
        public LaunchViewModel()
        {
            Add(new InitialCategory("Business", "CNN Money, Fox Business"));
            Add(new InitialCategory("Entertainment", "Huffington Post Entertainment, Hollywood Reporter"));
            Add(new InitialCategory("Fashion", "Us Magazine Style, Style.com"));
            Add(new InitialCategory("Health", "Huffington Post Health"));
            Add(new InitialCategory("Music", "MTV, VH1"));
            Add(new InitialCategory("News", "BBC, CNN"));
            Add(new InitialCategory("Politics", "Reuters, NY Times"));
            Add(new InitialCategory("Sports", "ESPN"));
            Add(new InitialCategory("Technology", "Engadget, Wired"));
        }
    }
}
