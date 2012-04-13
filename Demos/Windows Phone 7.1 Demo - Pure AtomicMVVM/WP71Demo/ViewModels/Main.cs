
namespace WP71Demo.ViewModels
{
    using AtomicMVVM;

    public class Main : CoreData
    {
        private int position = -1;
        private string[] messages = new[]{
            "These are not the droids you are looking for",
            "I love you - I know",
            "May the force be with you",
            "Made the kessel run in 12 parsecs"
        };

        private string message;

        public string Message
        {
            get { return message; }
            set
            {
                if (value != message)
                {
                    message = value;
                    RaisePropertyChanged("Message");
                }
            }
        }

        public Main()
        {
            CycleMessages();
        }

        public void CycleMessages()
        {
            position++;
            if (position == messages.Length)
            {
                position = 0;
            }

            Message = messages[position];
        }
    }
}
