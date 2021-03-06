﻿using System;

namespace PZ3_NetworkService.Model
{
    [Serializable]
    public class ReactorModel : ValidationBase
    {
        private double temp;
        private string name;
        private ReactorTypeModel type = new ReactorTypeModel();
        private int id;

        public int Id
        {
            get => this.id;
            set
            {
                if (this.id != value)
                {
                    this.id = value;
                    this.OnPropertyChanged("Id");
                }
            }
        }

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

        public ReactorTypeModel Type
        {
            get => this.type;
            set
            {
                if (!this.type.Equals(value))
                {
                    this.type = value;
                    this.OnPropertyChanged("Type");
                }
            }
        }

        public double Temperature
        {
            get => this.temp;
            set
            {
                if (this.temp != value)
                {
                    this.temp = value;
                    //this.ValidateTemperature();
                    this.OnPropertyChanged("Temperature");
                }
            }
        }

        public const int MIN_SAFE_TEMP_CELS = 250;
        public const int MAX_SAFE_TEMP_CELS = 350;

        public ReactorModel()
        {
            this.Name = string.Empty;
            this.Type = new ReactorTypeModel();
            this.Temperature = ReactorModel.MIN_SAFE_TEMP_CELS;
        }

        public ReactorModel(int id, string name, ReactorTypeModel type, double temperature)
        {
            this.Id = id;
            this.Name = name ?? string.Empty;
            this.Type = type ?? new ReactorTypeModel();
            this.Temperature = temperature;
        }

        public static ReactorModel Copy(ReactorModel reactor)
        {
            return new ReactorModel(reactor.id, reactor.name, reactor.type, reactor.temp);
        }

        #region overrides

        public override bool Equals(object obj)
        {
            if (obj is ReactorModel reactor)
            {
                return this.Id == reactor.Id;
            }
            return this.ToFullString().Equals(obj?.ToString() ?? string.Empty);
        }

        public override int GetHashCode()
        {
            return this.ToFullString().GetHashCode();
        }

        public override string ToString()
        {
            return $"{this.Name}, ID: {this.Id}";
        }

        protected override void ValidateSelf()
        {
            if (string.IsNullOrWhiteSpace(this.Name))
            {
                this.ValidationErrors["Name"] = "Name field is required.";
            }
        }

        #endregion overrides

        public string ToFullString()
        {
            return $"Id:{this.Id}, Name:{this.Name}, Temperature:{this.Temperature}, Type:{this.Type}";
        }

        public void ValidateTemperature()
        {
            if (!this.IsTemperatureSafe())
            {
                this.ValidationErrors["Temperature"] = "WARNING: Unsafe temperature!";
            }
            this.OnPropertyChanged("ValidationErrors");
        }

        public bool IsTemperatureSafe()
        {
            return this.Temperature >= MIN_SAFE_TEMP_CELS && this.Temperature <= MAX_SAFE_TEMP_CELS;
        }

        public void ValidateUniqueId()
        {
            if (Database.Reactors.ContainsKey(this.Id))
            {
                this.ValidationErrors["Id"] = "Id must be unique!";
            }
            this.OnPropertyChanged("ValidationErrors");
        }
    }
}