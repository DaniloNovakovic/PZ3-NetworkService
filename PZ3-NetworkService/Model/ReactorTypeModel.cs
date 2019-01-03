using System;

namespace PZ3_NetworkService.Model
{
    [Serializable]
    public class ReactorTypeModel
    {
        public string Name { get; set; }
        public string ImgSrc { get; set; }
        public ReactorTypeModel()
        {
            this.Name = string.Empty;
            this.ImgSrc = string.Empty;
        }

        public ReactorTypeModel(string name, string imgSrc)
        {
            this.Name = name ?? string.Empty;
            this.ImgSrc = imgSrc ?? string.Empty;
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
