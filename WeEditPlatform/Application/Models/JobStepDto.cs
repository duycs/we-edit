using Domain;

namespace Application.Models
{
    public class JobStepDto
    {
        public JobStep JobStep { get; set; }
        public List<Note> Notes { get; set; }
    }
}
