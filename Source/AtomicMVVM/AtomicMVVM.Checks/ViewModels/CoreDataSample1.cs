///-----------------------------------------------------------------------
/// Project: AtomicMVVM https://bitbucket.org/rmaclean/atomicmvvm
/// License: MS-PL http://www.opensource.org/licenses/MS-PL
/// Notes:
///-----------------------------------------------------------------------


namespace AtomicMVVM.ViewModels
{
    public class CoreDataSample1 : CoreData
    {
        private string input;

        public string Input
        {
            get { return input; }
            set
            {
                if (value != input)
                {
                    input = value;
                    RaisePropertyChanged("Input");
                }
            }
        }

        [TriggerProperty("Horse")]
        public void Meh()
        {
        }

        [ReevaluateProperty("Input")]
        public bool CanGo()
        {
            return true;
        }

        public void Go()
        {
            // does nothing
        }
    }
}
