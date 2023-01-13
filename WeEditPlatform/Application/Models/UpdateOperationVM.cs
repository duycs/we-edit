using Domain;

namespace Application.Models
{
    public class UpdateOperationVM
    {
        public int Id { get; set; }
        public OperationType? Type { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ExecutionName { get; set; }
        public Setting[]? Settings { get; set; }
        public bool FirstRoute { get; set; }
    }
}
