using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZ3_NetworkService.ViewModel
{
    public class NetworkViewModel : BindableBase
    {
        private Model.ReactorModel selectedReactor = new Model.ReactorModel();
        public BindingList<Model.ReactorModel> UntrackedReactors { get; set; }
        public Model.ReactorModel SelectedReactor
        {
            get => this.selectedReactor;
            set
            {
                this.selectedReactor = value;
                this.OnPropertyChanged("SelectedReactor");
            }
        }
        public NetworkViewModel()
        {
            this.UntrackedReactors = new BindingList<Model.ReactorModel>(Database.Reactors.Values.ToList());
        }

    }
}
