using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZ3_NetworkService
{
    public static class Database
    {
        private static DataIO serializer = new DataIO();
        public static Dictionary<int, Model.ReactorModel> Reactors { get; private set; }
        public static List<int> ReactorIds { get; private set; }

        static Database()
        {
            Database.Reactors = Database.Load() ?? new Dictionary<int, Model.ReactorModel>();
            Database.ReactorIds = Database.Reactors.Keys.ToList();
        }

        public static bool Add(Model.ReactorModel reactor)
        {
            try
            {
                if (reactor is null || Database.Reactors.ContainsKey(reactor.Id))
                {
                    return false;
                }
                Database.Reactors.Add(reactor.Id, reactor);
                Database.ReactorIds.Add(reactor.Id);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool Remove(int id)
        {
            try
            {
                if (!Database.Reactors.Remove(id))
                {
                    return false;
                }
                Database.ReactorIds.Remove(id);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #region serialization
        /// <summary>
        /// Attempts to serialize database to fileName
        /// </summary>
        public static bool Save(string fileName = "database.xml")
        {
            try
            {
                return serializer.SerializeObject(Database.Reactors, fileName);
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Attempts to DeSerialize database from fileName. 
        /// </summary>
        public static Dictionary<int, Model.ReactorModel> Load(string fileName = "database.xml")
        {
            try
            {
                return serializer.DeSerializeObject<Dictionary<int, Model.ReactorModel>>(fileName);
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}
