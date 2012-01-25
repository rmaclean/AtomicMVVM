using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MetroDemo.Converters
{
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
