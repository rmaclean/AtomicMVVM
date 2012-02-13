﻿
namespace MetroDemo.Converters
{
    using System;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Data;

    public class CountToVis : IValueConverter
    {
        public object Convert(object value, string typeName, object parameter, string language)
        {
            var countValue = (int)value;
            return countValue == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, string typeName, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}