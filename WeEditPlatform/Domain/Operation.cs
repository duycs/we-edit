using System.Text.Json.Serialization;

namespace Domain
{
    /// <summary>
    /// eg: Instance of an Action
    /// </summary>
    public class Operation : EntityBase
    {
        public Guid Uid { get; set; }
        public OperationType? Type { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ExecutionName { get; set; }


        [JsonIgnore]
        public ICollection<OperationSetting>? OperationSettings { get; set; }
        public ICollection<Setting>? Settings { get; set; }

        public int? FlowId { get; set; }
        public Flow? Flow { get; set; }


        public static Operation Create(Flow flow, OperationType? type, string? name, string? description,
            string? executionName, Setting[]? settings)
        {
            return new Operation()
            {
                Uid = Guid.NewGuid(),
                Flow = flow,
                Type = type,
                Name = name,
                Description = description,
                ExecutionName = executionName,
                Settings = settings
            };
        }

        public Operation Update(OperationType? type, string? name, string? description,
           string? executionName, Setting[]? settings)
        {
            if (type != null)
            {
                Type = type;
            }

            if (!string.IsNullOrEmpty(name))
            {
                Name = name;
            }

            if (!string.IsNullOrEmpty(description))
            {
                Description = description;
            }

            if (!string.IsNullOrEmpty(executionName))
            {
                ExecutionName = executionName;
            }

            if (settings != null && settings.Any())
            {
                Settings = settings;
            }

            return this;
        }
    }
}
