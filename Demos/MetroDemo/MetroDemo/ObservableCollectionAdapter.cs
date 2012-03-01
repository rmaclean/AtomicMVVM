using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace ObservableCollectionExample
{
    public class ObservableCollectionAdapter : IValueConverter
    {        
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((INotifyCollectionChanged)value).ToObservableVector();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
