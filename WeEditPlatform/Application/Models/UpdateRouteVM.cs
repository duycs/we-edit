namespace Application.Models
{
    public class UpdateRouteVM
    {
        public int RouteId { get; set; }
        public int FromOperationId { get; set; }
        public int ToOperationId { get; set; }
        public string? Description { get; set; }
    }
}
