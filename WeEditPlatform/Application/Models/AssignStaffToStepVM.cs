using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class AssignStaffToStepVM
    {
        public int JobId { get; set; }
        public int StaffId { get; set; }
        public int StepId { get; set; }
        public int? EstimationInSeconds { get; set; }
    }
}
