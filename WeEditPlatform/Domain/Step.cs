using System.Text.Json.Serialization;

namespace Domain
{
    public class Step : EntityBase
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public int OrderNumber { get; set; }

        public int GroupId { get; set; }

        public int ProductLevelId { get; set; }
        public ProductLevel ProductLevel { get; set; }

        public int EstimationInSeconds { get; set; }


        [JsonIgnore]
        public ICollection<JobStep>? JobSteps { get; set; }
        public ICollection<Job>? Jobs { get; set; }


        public static Step Create(string name, string code, int orderNumber,
            ProductLevel? productLevel, int groupId = 0, int estimationInSeconds = 0)
        {
            return new Step()
            {
                Name = name,
                Code = code,
                OrderNumber = orderNumber,
                ProductLevel = productLevel,
                GroupId = groupId,
                EstimationInSeconds = estimationInSeconds
            };
        }

        public Step UpdateStep(string name, ProductLevel? productLevel, int groupId, int estimationInSeconds)
        {
            if (!string.IsNullOrEmpty(name))
            {
                Name = name;
            }

            if (productLevel != null)
            {
                ProductLevel = productLevel;
            }

            if (groupId > 0)
            {
                GroupId = groupId;
            }

            if (estimationInSeconds > 0)
            {
                EstimationInSeconds = estimationInSeconds;
            }

            return this;
        }

        public Step UpdateEstimationInSeconds(int estimationInSeconds)
        {
            EstimationInSeconds = estimationInSeconds;
            return this;
        }
    }
}
