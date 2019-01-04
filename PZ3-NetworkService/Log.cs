using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZ3_NetworkService
{
    public static class Log
    {
        public static bool Append(object outObj, string fileName = "log.txt")
        {
            if (outObj is null)
            {
                return false;
            }
            try
            {
                File.AppendAllText(Environment.CurrentDirectory + @"\" + fileName, outObj.ToString() + Environment.NewLine);
                return true;
            }
            catch (Exception err)
            {
                Console.Error.WriteLine(err.Message);
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
            return $"{currDate.ToString(@"dd/MM/yyyy',' HH:mm")}: ${id}, ${temperature}";
        }
    }
}
