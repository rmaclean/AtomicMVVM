
namespace Silverlight5.ViewModels
{
    using AtomicMVVM;

    public class Demo : CoreData
    {
        private string message = "Welcome to the AtomicMVVM demo for SL 5";

        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                RaisePropertyChanged("Message");
            }
        }

        public void ChangeMessage()
        {
            Message = "And now it is changed!";
        }
    }
}
