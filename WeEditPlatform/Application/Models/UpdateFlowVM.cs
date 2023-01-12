using Domain;

namespace Application.Models
{
    public class UpdateFlowVM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public FlowStatus? Status { get; set; }
        public FlowType? Type { get; set; }
    }
}
