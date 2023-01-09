namespace Domain
{
    public class StaffGroup : EntityBase
    {
        public int StaffId { get; set; }
        public Staff Staff { get; set; }
        public int GroupId { get; set; }
        public Group Group { get; set; }
    }
}
