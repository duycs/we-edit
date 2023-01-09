using Domain;
using Infrastructure.Repository;

namespace Application.Queries
{
    public class JobStepsOfJobSpecification : SpecificationBase<JobStep>
    {
        public JobStepsOfJobSpecification(int[] jobIds, bool isInclude) : base(isInclude)
        {
            AddCriteria(c => jobIds.Contains(c.JobId));

            if (isInclude)
            {
                AddInclude(c => c.Job);
                AddInclude($"{nameof(JobStep.Step)}.{nameof(Step.ProductLevel)}");
                AddInclude(c => c.Worker);
                AddInclude(c => c.Shift);
            }
        }
    }
}
