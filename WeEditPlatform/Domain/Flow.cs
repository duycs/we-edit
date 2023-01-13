using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Flow : EntityBase
    {
        public Guid uid { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public FlowStatus? Status { get; set; }
        public FlowType? Type { get; set; }

        public ICollection<Operation>? Operations { get; set; }


        public static Flow Create(string? name, string? description, FlowStatus? status, FlowType? type)
        {
            return new Flow()
            {
                uid = Guid.NewGuid(),
                Name = name,
                Description = description,
                Status = status,
                Type = type
            };
        }

        public Flow Update(string? name, string? description, FlowStatus? status, FlowType? type)
        {
            if (!string.IsNullOrEmpty(name))
            {
                Name = name;
            }

            if (!string.IsNullOrEmpty(description))
            {
                Description = description;
            }

            if (status != null)
            {
                Status = status;
            }

            if (type != null)
            {
                Type = type;
            }

            return this;
        }

        public Operation? FirstRouteOperation()
        {
            if (Operations == null || !Operations.Any()) return null;

            // only once Operation then this is the first
            if (Operations.Count == 1) return Operations.First();

            return Operations.FirstOrDefault(o => o.IsFirstRoute());
        }
    }
}
