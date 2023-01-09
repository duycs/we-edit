namespace Domain
{
    public abstract class EntityBase
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public long DateDeleted { get; set; }
    }
}
