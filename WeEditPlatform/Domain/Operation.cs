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

        /// This FromOperation has many Routes
        public ICollection<Route>? Routes { get; set; }
        public bool FirstRoute { get; set; }

        /// <summary>
        /// Active: invoking in route
        /// </summary>
        public bool Active { get; set; }


        public static Operation Create(Flow flow, OperationType? type, string? name, string? description,
            string? executionName, bool? firstRoute, Setting[]? settings)
        {
            return new Operation()
            {
                Uid = Guid.NewGuid(),
                Flow = flow,
                Type = type,
                Name = name,
                Description = description,
                ExecutionName = executionName,
                FirstRoute = firstRoute ?? false,
                Settings = settings
            };
        }

        public Operation Update(OperationType? type, string? name, string? description,
           string? executionName, bool? firstRoute, Setting[]? settings)
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

            if (firstRoute != null)
            {
                FirstRoute = firstRoute ?? false;
            }

            return this;
        }

        public bool IsFirstRoute()
        {
            return FirstRoute;
        }

        public bool IsActive()
        {
            return Active;
        }

        public void SetActive()
        {
            Active = true;
        }
    }
}
