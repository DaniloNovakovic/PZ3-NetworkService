using System.ComponentModel;
using System.Linq;

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
            ReactorCellViewModel.ReactorUntracked += this.OnReactorUntracked;
        }

        private void OnReactorUntracked(object sender, PropertyChangedEventArgs e)
        {
            if (sender is Model.ReactorModel reactor)
            {
                this.UntrackedReactors.Add(Database.Reactors[reactor.Id]);
            }
        }
    }
}