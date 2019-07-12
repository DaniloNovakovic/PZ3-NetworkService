using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace PZ3_NetworkService.ViewModel
{
    public class MyLine
    {
        public double X1 { get; set; } = 0;
        public double Y1 { get; set; } = 0;
        public double X2 { get; set; } = 0;
        public double Y2 { get; set; } = 0;
        public double Opacity { get; set; } = 1;

        public SolidColorBrush Brush { get; set; }

        public MyLine(double x1 = 0, double y1 = 0, double x2 = 0, double y2 = 0, Color? brushColor = null, double opacity = 1)
        {
            this.X1 = x1;
            this.Y1 = y1;
            this.X2 = x2;
            this.Y2 = y2;
            this.Brush = new SolidColorBrush(brushColor ?? Colors.Green);
            this.Opacity = opacity;
        }

        public MyLine(MyPoint p1, MyPoint p2, Color? brushColor = null, double opacity = 1)
            : this(p1.X, p1.Y, p2.X, p2.Y, brushColor, opacity) { }

        public override string ToString()
        {
            return $"({this.X1},{this.Y1}):({this.X2},{this.Y2})";
        }
    }

    public class MyLabel
    {
        public string Content { get; set; } = string.Empty;
        public double X { get; set; } = 0;
        public double Y { get; set; } = 0;
        public double Angle { get; set; } = 0;
    }

    public class MyPoint
    {
        public string ToolTip { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double cX { get => this.X - this.R / 2; }
        public double cY { get => this.Y - this.R / 2; }
        public double R { get; set; }
        public Color Color { get; set; }

        public MyPoint(double x = 5, double y = 5, double r = 10, string toolTip = "", Color? color = null)
        {
            this.X = x;
            this.Y = y;
            this.R = r;
            this.ToolTip = toolTip;
            this.Color = color ?? Colors.Purple;
        }
    }

    public class DataChartViewModel : BindableBase
    {
        public MyICommand ShowChartCommand { get; set; }
        public BindingList<Model.ReactorModel> Reactors { get; private set; }
        private ObservableCollection<MyLine> lines = new ObservableCollection<MyLine>();

        public ObservableCollection<MyLine> Lines
        {
            get => this.lines;
            set
            {
                this.lines = value;
                this.OnPropertyChanged("Lines");
            }
        }

        private ObservableCollection<MyPoint> points = new ObservableCollection<MyPoint>();

        public ObservableCollection<MyPoint> Points
        {
            get => this.points;
            set
            {
                this.points = value;
                this.OnPropertyChanged("Points");
            }
        }

        private ObservableCollection<MyLabel> labels = new ObservableCollection<MyLabel>();

        public ObservableCollection<MyLabel> Labels
        {
            get => this.labels;
            set
            {
                this.labels = value;
                this.OnPropertyChanged("Labels");
            }
        }

        public Model.ReactorModel SelectedReactor { get; set; }
        private DateTime maxTime;
        private DateTime minTime;
        private double maxTemp;
        private double minTemp;

        public int MarginTop { get; private set; } = 10;
        public int MarginLeft { get; private set; } = 50;
        public int MarginBottom { get; private set; } = 60;
        public int MarginRight { get; private set; } = 30;
        public int CanvasHeight { get; private set; } = 350;
        public int CanvasWidth { get; private set; } = 600;
        public int ChartHeight { get => this.CanvasHeight - (this.MarginTop + this.MarginBottom); }
        public int ChartWidth { get => this.CanvasWidth - (this.MarginLeft + this.MarginRight); }
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

        private double yLabelSpace => this.ChartHeight / 5;
        private double xLabelSpace => this.ChartWidth / 5;

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
            var timeList = tuple.Item1;
            var tempsList = tuple.Item2;
            var myPoints = this.ConvertToPoints(timeList, tempsList);
            var myLines = this.ConnectPoints(myPoints);
            this.AddAxesLines(ref myLines);
            this.AddHorizontalLines(ref myLines, tempsList);
            this.AddVerticalLines(ref myLines, timeList);
            this.Lines = new ObservableCollection<MyLine>(myLines);

            this.Points = new ObservableCollection<MyPoint>(myPoints);

            var labels = new List<MyLabel>();
            this.AddLabels(ref labels);
            this.Labels = new ObservableCollection<MyLabel>(labels);
        }

        public void AddLabels(ref List<MyLabel> myLabels)
        {
            if (myLabels is null)
            {
                myLabels = new List<MyLabel>();
            }
            for (double y = this.MarginTop; y <= this.ChartHeight + this.MarginTop; y += this.yLabelSpace)
            {
                var label = new MyLabel()
                {
                    Content = string.Format("{0:0.0}", this.ScalePointToTemp(y, this.minTemp, this.maxTemp)),
                    X = 0,
                    Y = y - 10
                };
                myLabels.Add(label);
            }
            for (double x = this.MarginLeft; x <= this.ChartWidth + this.MarginLeft; x += this.xLabelSpace)
            {
                var label = new MyLabel()
                {
                    Content = this.ScalePointToTime(x, this.minTime, this.maxTime).ToString(@"dd/MM/yyyy HH:mm:ss"),
                    X = x - 10,
                    Y = this.MarginTop + this.ChartHeight,
                    Angle = 30
                };
                myLabels.Add(label);
            }
        }

        public void AddAxesLines(ref List<MyLine> myLines)
        {
            if (myLines is null)
            {
                myLines = new List<MyLine>();
            }
            myLines.Add(new MyLine(this.MarginLeft, this.MarginTop, this.MarginLeft, this.MarginTop + this.ChartHeight, Colors.Purple));
            myLines.Add(new MyLine(this.MarginLeft, this.MarginTop + this.ChartHeight, this.MarginLeft + this.ChartWidth, this.MarginTop + this.ChartHeight, Colors.Purple));
        }

        public void AddHorizontalLines(ref List<MyLine> myLines, List<double> tempsList)
        {
            if (myLines is null)
            {
                myLines = new List<MyLine>();
            }
            for (double y = this.MarginTop; y < this.ChartHeight; y += this.yLabelSpace)
            {
                var line = new MyLine(this.MarginLeft, y, this.MarginLeft + this.ChartWidth, y, Colors.Purple, 0.3);
                myLines.Add(line);
            }
        }

        public void AddVerticalLines(ref List<MyLine> myLines, List<DateTime> timesList)
        {
            if (myLines is null)
            {
                myLines = new List<MyLine>();
            }
            for (double x = this.MarginLeft; x <= this.ChartWidth + this.MarginLeft; x += this.xLabelSpace)
            {
                var line = new MyLine(x, this.MarginTop, x, this.MarginTop + this.ChartHeight, Colors.Purple, 0.3);
                myLines.Add(line);
            }
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
            var tempList = new List<double>();
            var timeList = new List<DateTime>();
            for (int i = 0; i < n && i < strList.Count; ++i)
            {
                var match = Regex.Match(strList[i], @"^([0-9/]+ [0-9:]+).*?([0-9.]+)$");
                string sdate = match.Groups[1].Value;
                string stemp = match.Groups[2].Value;
                var date = DateTime.Parse(sdate, CultureInfo.InvariantCulture);
                if (double.TryParse(stemp, out double temp))
                {
                    tempList.Add(temp);
                    timeList.Add(date);
                }
            }
            return new Tuple<List<DateTime>, List<double>>(timeList, tempList);
        }

        public DateTime ScalePointToTime(double x, DateTime minTime, DateTime maxTime)
        {
            double dTimeStart = ConvertToUnixTimestamp(minTime);
            double dTimeEnd = ConvertToUnixTimestamp(maxTime);
            double dTime = ConvertRange(0, this.ChartWidth, dTimeStart, dTimeEnd, x - this.MarginLeft);
            var ts = TimeSpan.FromSeconds(dTime);
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return origin + ts;
        }

        public double ScalePointToTemp(double y, double minTemp, double maxTemp)
        {
            double tempY = y - this.MarginTop;
            return ConvertRange(0, this.ChartHeight, maxTemp, minTemp, tempY);
        }

        public double ScaleTemp(double minTemp, double maxTemp, double temperature)
        {
            return this.ChartHeight - ConvertRange(minTemp, maxTemp, 0, this.ChartHeight, temperature) + this.MarginTop;
        }

        public double ScaleTime(DateTime minTime, DateTime maxTime, DateTime time)
        {
            return ConvertRange(minTime, maxTime, 0, this.ChartWidth, time) + this.MarginLeft;
        }

        public List<MyPoint> ConvertToPoints(List<DateTime> timeList, List<double> tempsList)
        {
            var retVal = new List<MyPoint>();
            this.maxTime = timeList.Max();
            this.minTime = timeList.Min();
            this.maxTemp = tempsList.Max();
            this.minTemp = tempsList.Min();

            int n = Math.Min(timeList.Count, tempsList.Count);
            for (int i = 0; i < n; ++i)
            {
                var pt = new MyPoint
                {
                    X = this.ScaleTime(this.minTime, this.maxTime, timeList[i]),
                    //X = ConvertRange(n, 0, 0, this.ChartWidth, i) + this.MarginLeft,
                    Y = this.ScaleTemp(this.minTemp, this.maxTemp, tempsList[i]),
                    ToolTip = $"Temp: {tempsList[i]}, {Environment.NewLine}Time: {timeList[i].ToString(@"dd/MM/yyyy HH:mm:ss")}"
                };
                Debug.WriteLine($" time:{timeList[i]}, temp:{tempsList[i]} => pt:{pt}");
                retVal.Add(pt);
            }

            return retVal;
        }

        public List<MyLine> ConnectPoints(List<MyPoint> points)
        {
            var retVal = new List<MyLine>();
            points.Sort((lhs, rhs) =>
            {
                return lhs.X.CompareTo(rhs.X);
            });
            int n = points.Count - 1;
            double maxSafeY = this.ScaleTemp(this.minTemp, this.maxTemp, Model.ReactorModel.MIN_SAFE_TEMP_CELS);
            double minSafeY = this.ScaleTemp(this.minTemp, this.maxTemp, Model.ReactorModel.MAX_SAFE_TEMP_CELS);
            for (int i = 0; i < n; ++i)
            {
                var col = Colors.Green;
                double maxSafeX = this.LineEquationGetX(points[i], points[i + 1], maxSafeY);
                double minSafeX = this.LineEquationGetX(points[i], points[i + 1], minSafeY);
                var minSafePoint = new MyPoint(minSafeX, minSafeY);
                var maxSafePoint = new MyPoint(maxSafeX, maxSafeY);
                var leftPoint = points[i];
                var rightPoint = points[i + 1];

                if ((leftPoint.Y > maxSafeY && rightPoint.Y > maxSafeY)
                    || (leftPoint.Y < minSafeY && rightPoint.Y < minSafeY))
                {
                    col = Colors.Red;
                }
                else if (leftPoint.Y > rightPoint.Y)
                {
                    if (leftPoint.Y > maxSafeY && rightPoint.Y < maxSafeY)
                    {
                        retVal.Add(new MyLine(leftPoint, maxSafePoint, Colors.Red));
                        leftPoint = maxSafePoint;
                    }
                    if (rightPoint.Y < minSafeY && leftPoint.Y > minSafeY)
                    {
                        retVal.Add(new MyLine(leftPoint, minSafePoint, Colors.Green));
                        leftPoint = minSafePoint;
                        col = Colors.Red;
                    }
                }
                else
                {
                    if (leftPoint.Y < minSafeY && rightPoint.Y > minSafeY)
                    {
                        retVal.Add(new MyLine(leftPoint, minSafePoint, Colors.Red));
                        leftPoint = minSafePoint;
                    }
                    if (rightPoint.Y > maxSafeY && leftPoint.Y < maxSafeY)
                    {
                        retVal.Add(new MyLine(leftPoint, maxSafePoint, Colors.Green));
                        leftPoint = maxSafePoint;
                        col = Colors.Red;
                    }
                }

                var line = new MyLine(leftPoint, rightPoint, col);
                Debug.WriteLine(line);
                retVal.Add(line);
            }
            return retVal;
        }

        public double LineEquationGetX(MyPoint p1, MyPoint p2, double y)
        {
            double k = (p2.Y - p1.Y) / (p2.X - p1.X);
            double n = p1.Y - k * p1.X;
            return (y - n) / k;
        }

        public static double ConvertToUnixTimestamp(DateTime date)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var diff = date - origin;
            return diff.TotalSeconds;
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
            DateTime originalStart, DateTime originalEnd,
            double newStart, double newEnd,
            DateTime value)
        {
            double dTime = ConvertToUnixTimestamp(value);
            double dTimeStart = ConvertToUnixTimestamp(originalStart);
            double dTimeEnd = ConvertToUnixTimestamp(originalEnd);
            return ConvertRange(dTimeStart, dTimeEnd, newStart, newEnd, dTime);
        }

        public static double ConvertRange(
            double originalStart, double originalEnd,
            double newStart, double newEnd,
            double value)
        {
            double scale = (newEnd - newStart) / (originalEnd - originalStart);
            return newStart + ((value - originalStart) * scale);
        }

        #endregion ConvertRange
    }
}