namespace Application.Models
{
    public class UpdateNoteVM
    {
        public int NoteId { get; set; }
        public int NoterId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ObjectName { get; set; }
        public int ObjectId { get; set; }
    }
}
