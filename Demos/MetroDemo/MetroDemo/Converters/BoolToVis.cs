﻿
namespace MetroDemo.Converters
{
    using System;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Data;

    public class BoolToVis : IValueConverter
    {
        public object Convert(object value, string typeName, object parameter, string language)
        {
            var boolValue = (bool)value;
            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, string typeName, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
