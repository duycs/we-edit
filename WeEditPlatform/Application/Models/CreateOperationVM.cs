using Domain;

namespace Application.Models
{
    public class CreateOperationVM
    {
        public int FlowId { get; set; }
        public OperationType? Type { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ExecutionName { get; set; }
        public Setting[]? Settings { get; set; }
    }
}
