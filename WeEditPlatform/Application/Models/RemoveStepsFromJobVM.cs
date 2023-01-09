using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class RemoveStepsFromJobVM
    {
        public int JobId { get; set; }
        public int[] StepIds { get; set; }
    }
}
