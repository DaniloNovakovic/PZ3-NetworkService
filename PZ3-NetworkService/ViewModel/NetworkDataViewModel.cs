using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZ3_NetworkService.ViewModel
{
    public class NetworkDataViewModel : BindableBase
    {
        public BindingList<Model.ReactorModel> ReactorList { get; private set; } = new BindingList<Model.ReactorModel>();
        public MyICommand AddReactorCommand { get; set; }
        public MyICommand DeleteReactorCommand { get; set; }
        public MyICommand FilterListCommand { get; set; }
        private Model.ReactorModel currentReactor = new Model.ReactorModel();
        public Model.ReactorModel CurrentReactor
        {
            get => this.currentReactor;
            set
            {
                this.currentReactor = value;
                this.OnPropertyChanged("CurrentReactor");
            }
        }

        public NetworkDataViewModel()
        {
            this.RefreshList();
            this.AddReactorCommand = new MyICommand(this.OnAdd);
            this.DeleteReactorCommand = new MyICommand(this.OnDelete);
            this.FilterListCommand = new MyICommand(this.OnFilter);
        }
        private void OnAdd()
        {
            this.CurrentReactor.Validate();
            if (this.CurrentReactor.IsValid && Database.Add(this.CurrentReactor))
            {
                this.RefreshList();
                // + Restart Metering Simulator?
            }
        }
        private void OnDelete()
        {
            // TODO: Implement this function
        }
        private void OnFilter()
        {
            // TODO: Implement this function
        }
        private void RefreshList()
        {
            this.ReactorList.Clear();
            foreach (Model.ReactorModel reactor in Database.Reactors.Values)
            {
                this.ReactorList.Add(reactor);
            }
        }

    }
}
