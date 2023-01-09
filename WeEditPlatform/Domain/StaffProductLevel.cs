namespace Domain
{
    public class StaffProductLevel : EntityBase
    {
        public int StaffId { get; set; }
        public Staff Staff { get; set; }

        public int ProductLevelId { get; set; }
        public ProductLevel ProductLevel { get; set; }
    }
}
