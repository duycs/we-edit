namespace Application.Models
{
    public class UpdateStaffVM
    {
        public int StaffId { get; set; }
        public string? FullName { get; set; }
        public string? Account { get; set; }
        public int[]? RoleIds { get; set; }
        public int[]? ProductLevelIds { get; set; }
        public int[]? GroupIds { get; set; }
    }
}
