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
        public static BindingList<Model.ReactorModel> ReactorList { get; private set; } = new BindingList<Model.ReactorModel>();
        static NetworkDataViewModel()
        {
            RefreshList();
        }
        private static void RefreshList()
        {
            ReactorList.Clear();
            foreach (Model.ReactorModel reactor in Database.Reactors.Values)
            {
                ReactorList.Add(reactor);
            }
        }

    }
}
