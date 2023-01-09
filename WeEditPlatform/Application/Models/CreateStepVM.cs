using Domain;

namespace Application.Models
{
    public class CreateStepVM
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public int OrderNumber { get; set; }
        public int ProductLevelId { get; set; }
        public int GroupId { get; set; }
        public int EstimationInSeconds { get; set; }
    }
}
