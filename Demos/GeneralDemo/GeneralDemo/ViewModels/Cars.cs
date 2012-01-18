using System.Collections.ObjectModel;
using AtomicMVVM;
using System.Linq;

namespace GeneralDemo.ViewModels
{
    using MakeData = HeaderedArray<HeaderedArray<string>>;
    using ModelData = HeaderedArray<string>;
    using System.Collections.Generic;

    public class HeaderedArray<T>
    {
        public string Name { get; private set; }
        public T[] Data { get; private set; }

        public HeaderedArray(string name, T[] data)
        {
            this.Name = name;
            this.Data = data;
        }
    }

    public class Cars : CoreData
    {
        // this should be in the model but is okasy for our needs.
        private MakeData[] data = new[]{
            new MakeData("VW", new []
            {
                new ModelData("Polo", new [] { "Black","Red","White" }), 
                new ModelData("Golf", new [] { "Black","Red","White" }),
                new ModelData("Fox", new [] { "Black","Yellow","White" }),
            }),
            new MakeData("Toyota", new[]
            {
                new ModelData("Yaris", new [] { "Silver","Blue","White" }),
                new ModelData("Camry", new [] { "Silver","Blue","White" }), 
                new ModelData("Prius", new [] { "Green" }), 
            }),
            new MakeData("Ford", new[]
            {
                new ModelData("Focus", new [] { "Orange","Blue","Red" }),
                new ModelData("Fiesta", new [] { "Silver","Blue","White", "Black" }), 
                new ModelData("Mustang", new [] { "Red", "Black" }), 
                new ModelData("F200", new [] { "Black" }), 
            })
        };

        private string[] _makes;
        private ObservableCollection<string> _models = new ObservableCollection<string>();
        private ObservableCollection<string> _colours = new ObservableCollection<string>();

        public string[] Makes
        {
            get { return _makes; }
            set
            {
                _makes = value;
            }
        }

        private string _selectedMake;

        public string SelectedMake
        {
            get { return _selectedMake; }
            set
            {
                _selectedMake = value;
                RaisePropertyChanged("SelectedMake");
            }
        }

        private string _selectedModel;

        public string SelectedModel
        {
            get { return _selectedModel; }
            set
            {
                _selectedModel = value;
                RaisePropertyChanged("SelectedModel");
            }
        }

        public ObservableCollection<string> Models
        {
            get { return _models; }
            set
            {
                _models = value;
            }
        }

        public ObservableCollection<string> Colours
        {
            get { return _colours; }
            set
            {
                _colours = value;
            }
        }

        public Cars()
        {
            _makes = data.Select(_ => _.Name).ToArray();
        }

        public void GoToMenu()
        {
            App.Bootstrapper.ChangeView<Menu>();
        }

        [TriggerProperty("SelectedMake")]
        public void SetModels()
        {
            if (string.IsNullOrWhiteSpace(this.SelectedMake))
            {
                this.Models.Clear();
                return;
            }

            var models = data.Single(_ => _.Name == this.SelectedMake).Data.Select(_ => _.Name);
            UpdateCollection(this.Models, models);
        }

        private void UpdateCollection<T>(IList<T> collection, IEnumerable<T> newData)
        {
            var remove = collection.Except(newData).ToList();
            var add = newData.Except(collection).ToList();
            foreach (var item in remove)
            {
                collection.Remove(item);
            }

            foreach (var item in add)
            {
                collection.Add(item);
            }
        }

        [TriggerProperty("SelectedModel")]
        [TriggerProperty("SelectedMake")]
        public void SetColours()
        {
            if (string.IsNullOrWhiteSpace(this.SelectedMake) || string.IsNullOrWhiteSpace(this.SelectedModel))
            {
                this.Colours.Clear();
                return;
            }

            var colours = data.Single(_ => _.Name == this.SelectedMake).Data.Single(_ => _.Name == this.SelectedModel).Data;
            UpdateCollection(this.Colours, colours);
        }
    }
}
