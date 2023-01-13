namespace Application.Models
{
    public class CreateRouteVM
    {
        public int FromOperationId { get; set; }
        public int ToOperationId { get; set; }
        public string? Description { get; set; }
    }
}
