using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZ3_NetworkService.Model
{
    [Serializable]
    public class ReactorModel : ValidationBase
    {
        private double temp;
        private string name;
        public int Id { get; private set; }
        public string Name
        {
            get => this.name;
            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    this.OnPropertyChanged("Name");
                }
            }
        }
        public ReactorTypeModel Type { get; private set; }
        public double Temperature
        {
            get => this.temp;
            set
            {
                if (this.temp != value)
                {
                    this.temp = value;
                    this.OnPropertyChanged("Temperature");
                }
            }
        }
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

        protected override void ValidateSelf()
        {
            if (string.IsNullOrWhiteSpace(this.Name))
            {
                this.ValidationErrors["Name"] = "Name field is required.";
            }
            if (this.Temperature < MIN_SAFE_TEMP_CELS || this.Temperature > MAX_SAFE_TEMP_CELS)
            {
                this.ValidationErrors["Temperature"] = "WARNING: Unsafe temperature!";
            }
        }
        #endregion
    }

}
