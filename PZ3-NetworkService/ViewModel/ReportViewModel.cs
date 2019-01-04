using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PZ3_NetworkService.ViewModel
{
    public class ReportViewModel : BindableBase
    {
        public MyICommand ShowCommand { get; set; }
        private DateTime startDate;
        private DateTime endDate;
        private string textReport = string.Empty;
        public DateTime StartDate
        {
            get => this.startDate;
            set
            {
                if (DateTime.Compare(this.startDate, value) != 0)
                {
                    this.startDate = value;
                    this.OnPropertyChanged("StartDate");
                }
            }
        }
        public DateTime EndDate
        {
            get => this.endDate;
            set
            {
                if (DateTime.Compare(this.endDate, value) != 0)
                {
                    this.endDate = value;
                    this.OnPropertyChanged("EndDate");
                }
            }
        }
        public string TextReport
        {
            get => this.textReport;
            set
            {
                if (value != this.textReport)
                {
                    this.textReport = value;
                    this.OnPropertyChanged("TextReport");
                }
            }
        }
        public ReportViewModel()
        {
            this.startDate = DateTime.Today;
            this.endDate = DateTime.Today;
            this.ShowCommand = new MyICommand(this.OnShow);
        }
        public void OnShow()
        {
            Dictionary<int, List<string>> logDict = Log.ParseLogFile(callback: (string currLine) =>
            {
                var match = Regex.Match(currLine, @"(\d+/\d+/\d+)");
                string sdate = match.Value;
                DateTime date = DateTime.ParseExact(sdate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                return DateTime.Compare(date, this.startDate) >= 0 && DateTime.Compare(date, this.endDate) <= 0;
            });
            this.TextReport = this.ConvertLogDictToStr(logDict);
        }
        private string ConvertLogDictToStr(Dictionary<int, List<string>> logDict)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("REPORT:");
            List<int> ids = logDict.Keys.Where(x => Database.Reactors.ContainsKey(x)).ToList();
            ids.Sort();
            foreach (int id in ids)
            {
                builder.AppendLine($"- {Database.Reactors[id].Name}, ID: {id}");
                logDict[id].Sort((lhs, rhs) =>
                {
                    const string regexPattern = @"(\d+/\d+/\d+ \d+:\d+)";
                    const string dateTimePattern = @"dd/MM/yyyy hh:mm";
                    DateTime leftDate = DateTime.ParseExact(Regex.Match(lhs, regexPattern).Value, dateTimePattern, CultureInfo.InvariantCulture);
                    DateTime rightDate = DateTime.ParseExact(Regex.Match(rhs, regexPattern).Value, dateTimePattern, CultureInfo.InvariantCulture);
                    return DateTime.Compare(leftDate, rightDate);
                });
                foreach (string el in logDict[id])
                {
                    builder.AppendLine($"\t{el}");
                }
                builder.AppendLine();
            }
            return builder.ToString();
        }
    }
}
