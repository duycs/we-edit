using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class RemoveRolesForStaffVM
    {
        public int[] RoleIds { get; set; }
        public int StaffId { get; set; }
    }
}
