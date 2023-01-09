using System.Text.Json.Serialization;

namespace Domain
{
    public class Role : Enumeration
    {
        public static Role Admin = new Role(1, nameof(Admin).ToLowerInvariant());
        public static Role CSO = new Role(2, nameof(CSO).ToLowerInvariant());
        public static Role Editor = new Role(3, nameof(Editor).ToLowerInvariant());
        public static Role QC = new Role(4, nameof(QC).ToLowerInvariant());

        public ICollection<Staff> Staffs { get; set; }

        [JsonIgnore]
        public ICollection<StaffRole> StaffRoles { get; set; }

        public Role(int id, string name) : base(id, name)
        {
        }
    }
}