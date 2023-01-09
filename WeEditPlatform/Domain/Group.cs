using System.Text.Json.Serialization;

namespace Domain
{
    public class Group : Enumeration
    {
        public static Group Admin = new Group(1, nameof(Admin).ToLowerInvariant());
        public static Group QC = new Group(2, nameof(QC).ToLowerInvariant());
        public static Group HighQuality = new Group(3, nameof(HighQuality).ToLowerInvariant());
        public static Group PhotoEditing = new Group(4, nameof(PhotoEditing).ToLowerInvariant());
        public static Group MergeRetouch = new Group(5, nameof(MergeRetouch).ToLowerInvariant());
        public static Group Video = new Group(6, nameof(Video).ToLowerInvariant());
        public static Group _2D3D = new Group(7, "2D3D".ToLowerInvariant());

        public ICollection<Staff> Staffs { get; set; }

        [JsonIgnore]
        public ICollection<StaffGroup> StaffGroups { get; set; }

        public Group(int id, string name) : base(id, name)
        {
        }

    }
}