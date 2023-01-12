using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class CreateFlowVM
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public FlowStatus? Status { get; set; }
        public FlowType? Type { get; set; }
    }
}
