using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZ3_NetworkService
{
    public class ValidationErrors : BindableBase
    {
        private readonly Dictionary<string, string> validationErrors = new Dictionary<string, string>();

        public bool IsValid
        {
            get
            {
                return this.validationErrors.Count < 1;
            }
        }

        public string this[string fieldName]
        {
            get
            {
                return this.validationErrors.ContainsKey(fieldName) ?
                    this.validationErrors[fieldName] : string.Empty;
            }



            set
            {
                if (this.validationErrors.ContainsKey(fieldName))
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        this.validationErrors.Remove(fieldName);
                    }
                    else
                    {
                        this.validationErrors[fieldName] = value;
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        this.validationErrors.Add(fieldName, value);
                    }
                }
                //this.OnPropertyChanged();
                this.OnPropertyChanged("InValid");
            }
        }

        public void Clear()
        {
            this.validationErrors.Clear();
        }
    }
}
