using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        public MyLine(Point p1, Point p2)
        {
            this.X1 = p1.X;
            this.Y1 = p1.Y;
            this.X2 = p2.X;
            this.Y2 = p2.Y;
        }
    }
    public class DataChartViewModel : BindableBase
    {
        public MyICommand ShowChartCommand { get; set; }
        public BindingList<Model.ReactorModel> Reactors { get; private set; }
        public ObservableCollection<MyLine> Lines { get; set; } = new ObservableCollection<MyLine>();
        public List<Point> Points { get; set; } = new List<Point>();

        public Model.ReactorModel SelectedReactor { get; set; }

        public int ChartHeight { get; set; } = 300;
        public int ChartWidth { get; set; } = 500;
        private int limit = 10;
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

        public DataChartViewModel()
        {
            this.Reactors = new BindingList<Model.ReactorModel>(Database.Reactors.Values.ToList());
            if (this.Reactors.Count > 0)
            {
                this.SelectedReactor = this.Reactors[0];
            }
            this.ShowChartCommand = new MyICommand(this.OnShowChart);
        }
        public void OnShowChart()
        {
            var tuple = this.FetchFromLog(this.SelectedReactor.Id, this.Limit);
            List<DateTime> timeList = tuple.Item1;
            List<double> tempsList = tuple.Item2;
            this.Points = this.ConvertToPoints(timeList, tempsList);
        }
        public List<Point> ConvertToPoints(List<DateTime> timeList, List<double> tempsList)
        {
            var retVal = new List<Point>();
            var maxTime = timeList.Max();
            var minTime = timeList.Min();
            var maxTemp = tempsList.Max();
            var minTemp = tempsList.Min();

            return retVal;
        }
        /// <summary>
        /// Finds 'N' number of last changes for reactor with given id and returns Tuple(timeVals, temperatures)
        /// </summary>
        /// <param name="id">Id of Reactor</param>
        /// <param name="n">Number of last changed values to search for</param>
        /// <returns>Tuple(timeList, temperaturesList)</returns>
        public Tuple<List<DateTime>, List<double>> FetchFromLog(int id, int n)
        {
            var logDict = this.GetLogDict(id);
            return this.ParseToTuple(logDict.ContainsKey(id) ? logDict[id] : new List<string>(), n);
        }
        public Dictionary<int, List<string>> GetLogDict(int reactorId)
        {
            return Log.ParseLogFile(ascending: false, callback: (currLine) =>
              {
                  var match = Regex.Match(currLine, @".*(\d+)\D+([0-9.]+)$");
                  if (int.TryParse(match.Groups[1].Value, out int id))
                  {
                      return id == reactorId;
                  }
                  return false;
              });
        }
        public Tuple<List<DateTime>, List<double>> ParseToTuple(List<string> strList, int n)
        {
            List<double> tempList = new List<double>();
            List<DateTime> timeList = new List<DateTime>();
            for (int i = 0; i < n && i < strList.Count; ++i)
            {
                var match = Regex.Match(strList[i], @"^(\d+/\d+/\d+ \d+:\d+).*?([0-9.]+)$");
                string sdate = match.Groups[1].Value;
                string stemp = match.Groups[2].Value;
                DateTime date = DateTime.ParseExact(sdate, @"dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                if (double.TryParse(stemp, out double temp))
                {
                    tempList.Add(temp);
                    timeList.Add(date);
                }
            }
            return new Tuple<List<DateTime>, List<double>>(timeList, tempList);
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
