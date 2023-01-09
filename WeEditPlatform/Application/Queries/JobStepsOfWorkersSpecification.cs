using Domain;
using Infrastructure.Repository;

namespace Application.Queries
{
    public class JobStepsOfWorkersSpecification : SpecificationBase<JobStep>
    {
        public JobStepsOfWorkersSpecification(int workerId, bool isInclude) : base(isInclude)
        {
            AddCriteria(c => c.DateDeleted == 0 && c.WorkerId != null && c.WorkerId == workerId);

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
