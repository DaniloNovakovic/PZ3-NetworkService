using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PZ3_NetworkService
{
    public static class Log
    {
        public static bool Append(object outObj, string fileName = "log.txt")
        {
            if (outObj is null || fileName is null)
            {
                Debug.Fail("Log.Append: Both input parameters must not be null!");
                return false;
            }
            try
            {
                File.AppendAllText(Environment.CurrentDirectory + @"\" + fileName, outObj.ToString() + Environment.NewLine);
                return true;
            }
            catch (Exception err)
            {
                Debug.Fail(err.Message);
                return false;
            }
        }
        public static string ConvertToLogFormat(Model.ReactorModel reactor)
        {
            return ConvertToLogFormat(reactor.Id, reactor.Temperature);
        }
        public static string ConvertToLogFormat(int id, double temperature)
        {
            var currDate = DateTime.Now;
            return $"{currDate.ToString(@"dd/MM/yyyy',' HH:mm:ss")}: {id}, {temperature}";
        }

        /// <summary>
        /// Parses given file into a dictionary where keys are reactor id, and values are list of lines that met callback condition
        /// </summary>
        /// <param name="fileName">Name of file to parse</param>
        /// <param name="callback">Receives current line and outputs true if it should be contained in returned dictionary</param>
        /// <returns></returns>
        public static Dictionary<int, List<string>> ParseLogFile(string fileName = "log.txt", bool ascending = true, Func<string, bool> callback = null)
        {
            Dictionary<int, List<string>> retVal = new Dictionary<int, List<string>>();
            try
            {
                string filePath = Environment.CurrentDirectory + @"\" + fileName;
                if (File.Exists(filePath))
                {
                    using (StreamReader sr = new StreamReader(filePath))
                    {
                        while (!sr.EndOfStream)
                        {
                            string currLine = sr.ReadLine();
                            if (!callback?.Invoke(currLine) ?? false)
                            {
                                continue;
                            }
                            var match = Regex.Match(currLine, @"([0-9/]+)\D+([0-9:]+)[:]\s+(\d+)\D+([0-9.]+)");
                            string sdate = match.Groups[1].Value;
                            string stime = match.Groups[2].Value;
                            string sid = match.Groups[3].Value;
                            string stemp = match.Groups[4].Value;
                            if (int.TryParse(sid, out int id))
                            {
                                if (!retVal.ContainsKey(id))
                                {
                                    retVal[id] = new List<string>();
                                }
                                retVal[id].Add($"{sdate} {stime}, CHANGED STATE: {stemp}");
                            }
                            else
                            {
                                Debug.Fail($"Failed to parse id={sid}");
                            }
                        }
                        SortDictValues(ref retVal, ascending);
                    }
                }
                else
                {
                    Debug.Fail($"File \"{filePath}\" does not exist.");
                }
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.Message);
            }
            return retVal;
        }
        public static void SortDictValues(ref Dictionary<int, List<string>> logDict, bool ascending = true)
        {
            foreach (int key in logDict.Keys)
            {
                logDict[key].Sort((lhs, rhs) =>
                {
                    try
                    {
                        const string regexPattern = @"[0-9/]+\D[0-9:]+[^:]";
                        //const string dateTimePattern = @"dd/MM/yyyy HH:mm:ss";
                        string leftMatch = Regex.Match(lhs, regexPattern).Value;
                        string rightMatch = Regex.Match(rhs, regexPattern).Value;
                        DateTime leftDate = DateTime.Parse(leftMatch, CultureInfo.InvariantCulture);
                        DateTime rightDate = DateTime.Parse(rightMatch, CultureInfo.InvariantCulture);
                        if (ascending)
                        {
                            return DateTime.Compare(leftDate, rightDate);
                        }
                        else
                        {
                            return DateTime.Compare(rightDate, leftDate);
                        }
                    }
                    catch (Exception err)
                    {
                        Debug.Fail(err.Message);
                        return 0;
                    }
                });
            }
        }
    }
}
