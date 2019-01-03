using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PZ3_NetworkService.ViewModel
{

    public class NetworkDataViewModel : BindableBase
    {
        public BindingList<Model.ReactorModel> ReactorList { get; private set; } = new BindingList<Model.ReactorModel>();
        public MyICommand AddReactorCommand { get; set; }
        public MyICommand DeleteReactorCommand { get; set; }
        public MyICommand FilterListCommand { get; set; }

        private readonly CollectionView reactorTypeNames;
        private readonly CollectionView filterTypeNames;
        public CollectionView ReactorTypeNames { get => this.reactorTypeNames; }
        public CollectionView FilterTypeNames { get => this.filterTypeNames; }

        private Model.ReactorModel currentReactor = new Model.ReactorModel();
        private string addTypeName = string.Empty;
        private string filterTypeName = string.Empty;
        private int filterId = 0;

        public bool[] FilterModeArray { get; } = new bool[] { false, true }; // {lt, gt}
        public int FilterSelectedMode
        {
            get { return Array.IndexOf(this.FilterModeArray, true); }
        }

        public string AddTypeName
        {
            get => this.addTypeName;
            set
            {
                if (this.addTypeName != value && Database.ReactorTypes.ContainsKey(value))
                {
                    this.addTypeName = value;
                    this.CurrentReactor.Type = Database.ReactorTypes[value];
                    this.OnPropertyChanged("AddTypeName");
                }
            }
        }
        public string FilterTypeName
        {
            get => this.filterTypeName;
            set
            {
                if (this.filterTypeName != value)
                {
                    this.filterTypeName = value;
                    this.OnPropertyChanged("FilterTypeName");
                }
            }
        }
        public int FilterId
        {
            get => this.filterId;
            set
            {
                if (value != this.filterId)
                {
                    this.filterId = value;
                    this.OnPropertyChanged("FilterId");
                }
            }
        }


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
            this.reactorTypeNames = new CollectionView(Database.ReactorTypes.Values.ToList());
            this.filterTypeNames = new CollectionView(Database.ReactorTypes.Values.ToList());
        }
        private void OnAdd()
        {
            this.CurrentReactor.Validate();
            this.CurrentReactor.ValidateUniqueId();
            if (this.CurrentReactor.IsValid && Database.Add(Model.ReactorModel.Copy(this.CurrentReactor)))
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
            this.ReactorList.Clear();
            foreach (KeyValuePair<int, Model.ReactorModel> pair in Database.Reactors)
            {
                int id = pair.Key;
                string typeName = pair.Value.Type.Name;

                if (typeName != this.FilterTypeName)
                    continue;

                switch (this.FilterSelectedMode)
                {
                    case 0:
                        if (id < this.FilterId)
                        {
                            this.ReactorList.Add(Database.Reactors[id]);
                        }
                        break;
                    case 1:
                        if (id > this.FilterId)
                        {
                            this.ReactorList.Add(Database.Reactors[id]);
                        }
                        break;
                }
            }
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
