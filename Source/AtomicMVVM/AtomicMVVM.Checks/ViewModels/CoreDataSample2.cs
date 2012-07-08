///-----------------------------------------------------------------------
/// Project: AtomicMVVM https://bitbucket.org/rmaclean/atomicmvvm
/// License: MS-PL http://www.opensource.org/licenses/MS-PL
/// Notes:
///-----------------------------------------------------------------------


namespace AtomicMVVM.ViewModels
{
    public class CoreDataSample2 : CoreData
    {
        public CoreDataSample2(string s)
        {
            this.Input = s;
        }

        public string Input { get; set; }
    }
}
