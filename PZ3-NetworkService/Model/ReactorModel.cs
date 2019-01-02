using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZ3_NetworkService.Model
{
    [Serializable]
    public class ReactorModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ReactorTypeModel Type { get; set; }
        public double Temperature { get; set; }
        public const int MIN_SAFE_TEMP_CELS = 250;
        public const int MAX_SAFE_TEMP_CELS = 350;

        public ReactorModel() { }
        public ReactorModel(int id, string name, ReactorTypeModel type, double temperature)
        {
            this.Id = id;
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.Type = type ?? throw new ArgumentNullException(nameof(type));
            this.Temperature = temperature;
        }
        #region overrides
        public override bool Equals(object obj)
        {
            return this.ToString().Equals(obj?.ToString() ?? string.Empty);
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override string ToString()
        {
            return $"Id:{this.Id}, Name:{this.Name}, Temperature:{this.Temperature}, Type:{this.Type}";
        }
        #endregion
    }

}
