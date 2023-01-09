namespace Domain
{
    public class StaffRole : EntityBase
    {
        public int StaffId { get; set; }
        public Staff Staff { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}
