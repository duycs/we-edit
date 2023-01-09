using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class CreateJobVM
    {
        public DateTime Date { get; set; }
        public int LocationId { get; set; }
        public int CSOStaffId { get; set; }
        public string JobName { get; set; }
        public string Code { get; set; }
        public string Instruction { get; set; }
        public string InputInfo { get; set; }
        public int InputNumber { get; set; }
        public int DeliverTypeId { get; set; }
        public int ProductLevelId { get; set; }
        public DateTime Deadline { get; set; }
        public int AppId { get; set; }
    }
}
