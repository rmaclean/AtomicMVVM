
namespace AtomicMVVM
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// This is the lighter version of <see cref="CoreData"/>. 
    /// It just handles INotifyPropertyChanged events.
    /// </summary>
    public class CoreDataLight : INotifyPropertyChanged
    {
        private Bootstrapper BootStrapper;

        /// <summary>
        /// Simple constructor for CoreDataLight - this has no bootstrapper assigned and thus will not cannot invoke on the view model.
        /// </summary>
        public CoreDataLight() : this(null){}

        /// <summary>
        /// Creates an instance of CoreDataLight - recommended to use this one as it allows the call to be called on the view model.
        /// </summary>
        /// <param name="bootstrapper"></param>
        public CoreDataLight(Bootstrapper bootstrapper)
        {
            this.BootStrapper = bootstrapper;
        }

        /// <summary>
        /// Raises the property changed event for a property.
        /// </summary>
        public void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                if (BootStrapper != null)
                {
                    var _  = BootStrapper.CurrentViewModel.InvokeAsync(() =>
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                    });
                }
                else
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        /// <summary>
        /// This event is raised when the <see cref="RaisePropertyChanged"/> method is called.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;        
    }
}
