using System.Text.Json.Serialization;

namespace Domain
{
    /// <summary>
    /// Product level same as Skill
    /// </summary>
    public class ProductLevel : EntityBase
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [JsonIgnore]
        public virtual ICollection<StaffProductLevel>? StaffProductLevels { get; set; }
        public virtual ICollection<Staff>? Staffs { get; set; }

        public virtual ICollection<Job>? Jobs { get; set; }

        public virtual ICollection<Step>? Steps { get; set; }


        public static ProductLevel Create(string code, string name, string description)
        {
            return new ProductLevel()
            {
                Code = code,
                Name = name,
                Description = description
            };
        }

        public ProductLevel Update(string code, string name, string description)
        {
            if (!string.IsNullOrEmpty(code))
            {
                Code = code;
            }

            if (!string.IsNullOrEmpty(name))
            {
                Name = name;
            }

            if (!string.IsNullOrEmpty(description))
            {
                Description = description;
            }

            return this;
        }

        /// <summary>
        /// Eg: PE-LV1
        /// </summary>
        /// <param name="sub"></param>
        /// <returns></returns>
        public string GetCodeWithSub(string sub)
        {
            return string.Format("{0}-{1}", this.Code, sub);
        }
    }
}
