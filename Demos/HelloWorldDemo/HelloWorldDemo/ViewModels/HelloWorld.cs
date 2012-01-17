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
                RaisePropertyChanged("Username");
            }
        }

        private string _Message;

        public string Message
        {
            get { return _Message; }
            set
            {
                _Message = value;
                RaisePropertyChanged("Message");
            }
        }


        public void SetMessage()
        {
            Message = "Hello " + Username;
        }

    }
}
