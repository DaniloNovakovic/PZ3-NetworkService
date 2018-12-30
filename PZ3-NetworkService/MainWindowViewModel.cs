using PZ3_NetworkService.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZ3_NetworkService
{
    public class MainWindowViewModel : BindableBase
    {
        public MyICommand<string> NavCommand { get; private set; }
        private DataChartViewModel dataChartViewModel = new DataChartViewModel();
        private NetworkDataViewModel networkDataViewModel = new NetworkDataViewModel();
        private NetworkViewModel networkViewModel = new NetworkViewModel();
        private ReportViewModel reportViewModel = new ReportViewModel();
        private BindableBase currentViewModel;

        public MainWindowViewModel()
        {
            this.NavCommand = new MyICommand<string>(this.OnNav);
            this.CurrentViewModel = this.networkDataViewModel;
        }

        public BindableBase CurrentViewModel
        {
            get { return this.currentViewModel; }
            set
            {
                this.SetProperty(ref this.currentViewModel, value);
            }
        }

        private void OnNav(string destination)
        {
            switch (destination)
            {
                case "network":
                    this.CurrentViewModel = this.networkViewModel;
                    break;
                case "networkData":
                    this.CurrentViewModel = this.networkDataViewModel;
                    break;
                case "dataChart":
                    this.CurrentViewModel = this.dataChartViewModel;
                    break;
                case "report":
                    this.CurrentViewModel = this.reportViewModel;
                    break;
            }
        }
    }
}
