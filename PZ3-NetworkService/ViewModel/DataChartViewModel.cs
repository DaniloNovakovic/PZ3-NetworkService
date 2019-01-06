using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace PZ3_NetworkService.ViewModel
{
    public class MyLine
    {
        public double X1 { get; set; } = 0;
        public double Y1 { get; set; } = 0;
        public double X2 { get; set; } = 0;
        public double Y2 { get; set; } = 0;

        public MyLine(double x1 = 0, double y1 = 0, double x2 = 0, double y2 = 0)
        {
            this.X1 = x1;
            this.Y1 = y1;
            this.X2 = x2;
            this.Y2 = y2;
        }
    }
    public class DataChartViewModel : BindableBase
    {
        public ObservableCollection<MyLine> Points { get; set; } = new ObservableCollection<MyLine>()
        {
            new MyLine(0, 0, 150, 150),
            new MyLine(150, 150, 20, 167)
        };
        public BindingList<Model.ReactorModel> Reactors { get; private set; }
        public Model.ReactorModel SelectedReactor { get; set; }

        public int ChartHeight { get; set; } = 300;
        public int ChartWidth { get; set; } = 400;
        private int limit = 10;
        public DataChartViewModel()
        {
            this.Reactors = new BindingList<Model.ReactorModel>(Database.Reactors.Values.ToList());
            if(this.Reactors.Count > 0)
            {
                this.SelectedReactor = Reactors[0];
            }
        }
        public int Limit
        {
            get => this.limit;
            set
            {
                if (value != this.limit)
                {
                    this.limit = value < 0 ? 0 : value;
                    this.OnPropertyChanged("Limit");
                }
            }
        }

        #region ConvertRange
        public static int ConvertRange(
            int originalStart, int originalEnd, // original range
            int newStart, int newEnd, // desired range
            int value) // value to convert
        {
            double scale = (double)(newEnd - newStart) / (originalEnd - originalStart);
            return (int)(newStart + ((value - originalStart) * scale));
        }
        public static double ConvertRange(
            double originalStart, double originalEnd,
            double newStart, double newEnd,
            double value)
        {
            double scale = (newEnd - newStart) / (originalEnd - originalStart);
            return newStart + ((value - originalStart) * scale);
        }
        #endregion
    }

}
