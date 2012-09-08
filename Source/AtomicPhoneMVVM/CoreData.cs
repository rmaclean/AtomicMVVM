//-----------------------------------------------------------------------
// Project: AtomicPhoneMVVM https://bitbucket.org/rmaclean/atomicmvvm
// License: MS-PL http://www.opensource.org/licenses/MS-PL
// Notes:
//-----------------------------------------------------------------------  

namespace AtomicPhoneMVVM
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using Microsoft.Phone.Controls;

    /// <summary>
    /// Provides the implementation details for the view model page and helper methods.
    /// </summary>
    public class CoreData : INotifyPropertyChanged
    {
        /// <summary>
        /// The page as a message reciever.
        /// </summary>
        public IPushMessage Page { get; internal set; }

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Occurs when a property is changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Instructs the page to navigate to the specific page.
        /// </summary>
        /// <param name="page">The path to the new page.</param>
        public void Navigate(string page)
        {
            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri(page, UriKind.Relative));
        }

        /// <summary>
        /// Instructs the page to navigate back in the phone back stack.
        /// </summary>
        public void NavigateBack()
        {
            (Application.Current.RootVisual as PhoneApplicationFrame).GoBack();
        }

        /// <summary>
        /// Pushes a message to the page.
        /// </summary>
        /// <param name="message">The message</param>
        public void PushMessage(string message)
        {
            if (Page != null)
            {
                Page.Push(message);
            }
        }
    }
}
