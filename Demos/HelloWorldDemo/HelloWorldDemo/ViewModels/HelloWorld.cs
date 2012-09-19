//-----------------------------------------------------------------------
// Project: AtomicMVVM https://bitbucket.org/rmaclean/atomicmvvm
// License: MS-PL http://www.opensource.org/licenses/MS-PL
// Notes:
//-----------------------------------------------------------------------

using AtomicMVVM;

namespace HelloWorldDemo.ViewModels
{
    public class HelloWorld : CoreData
    {
        private string _UserName;

        public string Username
        {
            get { return _UserName; }
            set
            {
                _UserName = value;
                RaisePropertyChanged();
            }
        }

        private string _Message;

        public string Message
        {
            get { return _Message; }
            set
            {
                _Message = value;
                RaisePropertyChanged();
            }
        }


        public void SetMessage()
        {
            Message = "Hello " + Username;
        }

    }
}
