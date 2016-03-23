// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Data;
using System.Windows;
using System.Windows.Media;

namespace FeedCast.Converters
{
    public class IsReadToFontFamilyConverter : IValueConverter
    {

        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(null != value)
            {
                return (((bool)value) ? Application.Current.Resources["PhoneFontFamilySemiLight"] : Application.Current.Resources["PhoneFontFamilySemiBold"]);
            }
            return Application.Current.Resources["PhoneFontFamilySemiLight"];
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            FontFamily fontFamily = value as FontFamily;
            if (null != fontFamily)
            {
                return fontFamily == Application.Current.Resources["PhoneFontFamilySemiLight"];
            }
            return false;
        }
    }
}
