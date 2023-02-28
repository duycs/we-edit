using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class PhotoDto
    {
        public string Src { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public PhotoDto(string src, string name = "", string description = "")
        {
            Src = src;
            Name = name;
            Description = description;
        }
    }
}
