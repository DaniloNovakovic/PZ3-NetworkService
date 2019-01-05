using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace PZ3_NetworkService.ViewModel
{
    public class ReactorCellViewModel : BindableBase
    {
        public MyICommand UntrackCommand { get; private set; }
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
            }
        }
        public double Temperature
        {
            get => SelectedReactor?.Temperature ?? default(double);
        }
        public string BorderBrush
        {
            get {
                if (this.SelectedReactor != null)
                {
                    return this.SelectedReactor.IsTemperatureSafe() ? "#FF00FF00" : "#FFFF0000";
                } else
                {
                    return "#FF000000";
                }
            }
        }
        public ReactorCellViewModel()
        {
            this.UntrackCommand = new MyICommand(OnUntrack);
            //this.SelectedReactor.PropertyChanged += this.SelectedReactor_PropertyChanged;
        }

        private void SelectedReactor_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "Temperature")
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
                this.SelectedReactor = null;
            } 
        }
    }
}
