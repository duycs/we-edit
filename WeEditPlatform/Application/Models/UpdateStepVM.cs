namespace Application.Models
{
    public class UpdateStepVM
    {
        public int StepId { get; set; }
        public string Name { get; set; }
        public int ProductLevelId { get; set; }
        public int GroupId { get; set; }
        public int EstimationInSeconds { get; set; }
    }
}
