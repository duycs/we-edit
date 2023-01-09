using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Extensions
{
    public static class StringExtension
    {
        public static bool IsUuid(this string id)
        {
            return id.Contains("-") || id.Contains("_");
        }
    }
}
