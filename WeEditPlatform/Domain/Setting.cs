using System.Text.Json.Serialization;

namespace Domain
{
    public class Setting : EntityBase
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public string? Name { get; set; }
        public string? Description { get; set; }

        public ICollection<Operation>? Operations { get; set; }

        [JsonIgnore]
        public ICollection<OperationSetting>? OperationSettings { get; set; }

        public static Setting Create(string key, string value, string? name, string? description)
        {
            return new Setting
            {
                Key = key,
                Value = value,
                Name = name ?? key,
                Description = description
            };
        }

        public void Update(string key, string value, string? name, string? description)
        {
            if (!string.IsNullOrEmpty(key))
            {
                Key = key;
            }

            if (!string.IsNullOrEmpty(value))
            {
                Value = value;
            }

            if (!string.IsNullOrEmpty(name))
            {
                Name = name;
            }

            if (!string.IsNullOrEmpty(description))
            {
                Description = description;
            }
        }
    }
}
