using System;

namespace PZ3_NetworkService.Model
{
    public class ReactorTypeModel
    {
        public string Name { get; set; }
        public string ImgSrc { get; private set; }
        public ReactorTypeModel() { }

        public ReactorTypeModel(string name, string imgSrc)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.ImgSrc = imgSrc ?? throw new ArgumentNullException(nameof(imgSrc));
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
            return $"{this.Name} {this.ImgSrc}";
        }
        #endregion
    }
}
