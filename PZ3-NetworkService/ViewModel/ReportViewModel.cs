using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

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

        private FlowDocument flowDoc = new FlowDocument();

        public FlowDocument FlowDoc
        {
            get => this.flowDoc;
            set
            {
                this.flowDoc = value;
                this.OnPropertyChanged("FlowDoc");
            }
        }

        public ReportViewModel()
        {
            this.startDate = DateTime.ParseExact("01/01/2019", "dd/MM/yyyy", CultureInfo.InvariantCulture);
            this.endDate = DateTime.Today;
            this.ShowCommand = new MyICommand(this.OnShow);
        }

        public void OnShow()
        {
            var logDict = Log.ParseLogFile(callback: (string currLine) =>
            {
                var match = Regex.Match(currLine, @"(\d+/\d+/\d+)");
                string sdate = match.Value;
                var date = DateTime.ParseExact(sdate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                return DateTime.Compare(date, this.startDate) >= 0 && DateTime.Compare(date, this.endDate) <= 0;
            });
            //this.TextReport = this.ConvertLogDictToStr(logDict);
            this.FlowDoc = this.ConvertLogToFlowDocument(logDict);
        }

        private FlowDocument ConvertLogToFlowDocument(Dictionary<int, List<string>> logDict)
        {
            var doc = new FlowDocument();
            var paragraph = new Paragraph(new Run("REPORT:"))
            {
                Foreground = new SolidColorBrush(Colors.Purple),
                FontWeight = FontWeights.Bold,
                FontSize = 27
            };
            doc.Blocks.Add(paragraph);

            var ids = logDict.Keys.Where(x => Database.Reactors.ContainsKey(x)).ToList();
            ids.Sort();
            foreach (int id in ids)
            {
                paragraph = new Paragraph(new Run($"- {Database.Reactors[id].Name}, ID: {id}"))
                {
                    Foreground = new SolidColorBrush(Colors.Purple),
                    FontWeight = FontWeights.Bold,
                    FontSize = 17
                };
                doc.Blocks.Add(paragraph);
                foreach (string el in logDict[id])
                {
                    var col = Colors.Green;
                    var match = Regex.Match(el, @".*?([0-9.]+)$");
                    if (double.TryParse(match.Groups[1].Value, out double val))
                    {
                        if (val < Model.ReactorModel.MIN_SAFE_TEMP_CELS || val > Model.ReactorModel.MAX_SAFE_TEMP_CELS)
                        {
                            col = Colors.Red;
                        }
                    }
                    paragraph = new Paragraph(new Run($"\t{el}"))
                    {
                        Foreground = new SolidColorBrush(col),
                        FontWeight = FontWeights.Bold,
                        FontSize = 13
                    };
                    doc.Blocks.Add(paragraph);
                }
                doc.Blocks.Add(new Paragraph(new Run(Environment.NewLine)));
            }
            return doc;
        }

        private string ConvertLogDictToStr(Dictionary<int, List<string>> logDict)
        {
            var builder = new StringBuilder();
            builder.AppendLine("REPORT:");
            var ids = logDict.Keys.Where(x => Database.Reactors.ContainsKey(x)).ToList();
            ids.Sort();
            foreach (int id in ids)
            {
                builder.AppendLine($"- {Database.Reactors[id].Name}, ID: {id}");
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