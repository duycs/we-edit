using Domain;
using Infrastructure.Repository;

namespace Application.Queries
{
    public class TodoJobsSpecification : SpecificationBase<Job>
    {
        public TodoJobsSpecification(int[] jobIds, bool isInclude) : base(isInclude)
        {
            AddCriteria(c => jobIds.Contains(c.Id));

            if (IsInclude)
            {
                AddInclude(w => w.Steps);
                AddInclude(w => w.JobSteps);
                AddInclude(w => w.ProductLevel);
                AddInclude(w => w.CSO);
            }
        }
    }
}
