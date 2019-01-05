using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace PZ3_NetworkService.ViewModel
{
    public class ReactorCellViewModel : BindableBase
    {
        public static event PropertyChangedEventHandler ReactorUntracked = delegate { };

        public MyICommand UntrackCommand { get; private set; }
        public BindingList<Model.ReactorModel> Collection { get; set; } = new BindingList<Model.ReactorModel>();
        private Model.ReactorModel selectedReactor;
        public Model.ReactorModel SelectedReactor
        {
            get => this.selectedReactor;
            set
            {
                this.selectedReactor = value;
                this.OnPropertyChanged("SelectedReactor");
                this.OnPropertyChanged("BorderBrush");
                this.OnPropertyChanged("Temperature");
                this.OnPropertyChanged("Title");
            }
        }
        public string Title
        {
            get => this.SelectedReactor?.ToString() ?? string.Empty;
        }
        public double Temperature
        {
            get => this.SelectedReactor?.Temperature ?? default(double);
        }
        public string BorderBrush
        {
            get
            {
                if (this.SelectedReactor != null)
                {
                    return this.SelectedReactor.IsTemperatureSafe() ? "#AEEA00" : "#d50000";
                }
                else
                {
                    return "#FF000000";
                }
            }
        }
        public ReactorCellViewModel()
        {
            this.UntrackCommand = new MyICommand(this.OnUntrack);
            this.Collection.RaiseListChangedEvents = true;
            this.Collection.ListChanged += this.Collection_ListChanged;
        }

        private void Collection_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemAdded)
            {
                this.SelectedReactor = this.Collection[e.NewIndex];
                this.SelectedReactor.PropertyChanged += this.SelectedReactor_PropertyChanged;
            }
        }


        private void SelectedReactor_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Temperature")
            {
                this.OnPropertyChanged("BorderBrush");
                this.OnPropertyChanged("Temperature");
            }
        }

        public void OnUntrack()
        {
            if (this.SelectedReactor != null)
            {
                this.SelectedReactor.PropertyChanged -= this.SelectedReactor_PropertyChanged;
                ReactorUntracked(this.SelectedReactor, new PropertyChangedEventArgs(""));
                this.SelectedReactor = null;
                this.Collection.Clear();
            }
        }
    }
}
