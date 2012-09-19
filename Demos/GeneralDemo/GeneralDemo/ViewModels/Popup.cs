//-----------------------------------------------------------------------
// Project: AtomicMVVM https://bitbucket.org/rmaclean/atomicmvvm
// License: MS-PL http://www.opensource.org/licenses/MS-PL
// Notes:
//-----------------------------------------------------------------------

using System.Windows;
using AtomicMVVM;

namespace GeneralDemo.ViewModels
{
    public class Popup : CoreData
    {
        private string _fullName;

        public string FullName
        {
            get { return _fullName; }
            set
            {
                _fullName = value;
                RaisePropertyChanged();
            }
        }

        public void ShowName()
        {
            MessageBox.Show("Hello " + this.FullName);
        }

        public void SayGoodbye()
        {
            MessageBox.Show("Good bye " + this.FullName);
        }

        [ReevaluateProperty("FullName")]
        public bool CanShowName()
        {
            return !(string.IsNullOrWhiteSpace(this.FullName));
        }

        [ReevaluateProperty("FullName")]
        public bool CanSayGoodbye()
        {
            return CanShowName();
        }
    }
}
