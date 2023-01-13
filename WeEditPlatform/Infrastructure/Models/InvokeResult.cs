using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models
{
    public class InvokeResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public InvokeResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
