using System.Collections.Generic;
using System.Linq;

namespace PZ3_NetworkService
{
    public static class Database
    {
        private static readonly DataIO serializer = new DataIO();
        public static SortedDictionary<int, Model.ReactorModel> Reactors { get; private set; }
        public static List<int> ReactorIds { get; private set; }
        public static SortedDictionary<string, Model.ReactorTypeModel> ReactorTypes { get; private set; }

        static Database()
        {
            Database.Reactors = Database.Load() ?? new SortedDictionary<int, Model.ReactorModel>();
            Database.ReactorIds = Database.Reactors.Keys.ToList();
            Database.ReactorTypes = new SortedDictionary<string, Model.ReactorTypeModel>()
            {
                { "thermal", new Model.ReactorTypeModel("thermal", "/Images/thermal_power_plant.jpg")},
                { "fusion", new Model.ReactorTypeModel("fusion","/Images/fusion_power_plant.jpg") }
            };
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
                return serializer.SerializeObject(Database.Reactors.Values.ToList(), fileName);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Attempts to DeSerialize database from fileName.
        /// </summary>
        public static SortedDictionary<int, Model.ReactorModel> Load(string fileName = "database.xml")
        {
            try
            {
                var reactors = serializer.DeSerializeObject<List<Model.ReactorModel>>(fileName);
                var retVal = new SortedDictionary<int, Model.ReactorModel>();
                foreach (var reactor in reactors)
                {
                    retVal[reactor.Id] = reactor;
                }
                return retVal;
            }
            catch
            {
                return null;
            }
        }

        #endregion serialization
    }
}