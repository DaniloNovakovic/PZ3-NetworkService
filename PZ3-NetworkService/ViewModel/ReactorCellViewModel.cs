using System.ComponentModel;

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

        public bool ButtonEnabled
        {
            get => this.Collection.Count > 0;
        }

        public string GridVisibility
        {
            get => this.Collection.Count > 0 ? "Hidden" : "Visible";
        }

        public string Title
        {
            get => this.SelectedReactor?.ToString() ?? string.Empty;
        }

        public double? Temperature
        {
            get => this.SelectedReactor?.Temperature;
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
            this.OnPropertyChanged("GridVisibility");
            this.OnPropertyChanged("ButtonEnabled");
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