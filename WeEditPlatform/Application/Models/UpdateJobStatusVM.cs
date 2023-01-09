using Domain;

namespace Application.Models
{
    public class UpdateJobStatusVM
    {
        public int JobId { get; set; }
        public JobStatus JobStatus { get; set; }
    }
}
