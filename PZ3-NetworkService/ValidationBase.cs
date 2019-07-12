using System.Xml.Serialization;

namespace PZ3_NetworkService
{
    public abstract class ValidationBase : BindableBase
    {
        public ValidationErrors ValidationErrors { get; set; }

        [XmlIgnoreAttribute]
        public bool IsValid { get; private set; }

        protected ValidationBase()
        {
            this.ValidationErrors = new ValidationErrors();
        }

        protected abstract void ValidateSelf();

        public void Validate()
        {
            this.ValidationErrors.Clear();
            this.ValidateSelf();
            this.IsValid = this.ValidationErrors.IsValid;
            this.OnPropertyChanged("IsValid");
            this.OnPropertyChanged("ValidationErrors");
        }
    }
}