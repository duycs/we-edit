using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class OperationSetting : EntityBase
    {
        public int OperationId { get; set; }
        public Operation Operation { get; set; }
        public int SettingId { get; set; }
        public Setting Setting { get; set; }
    }
}
