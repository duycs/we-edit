using Domain;
using Infrastructure.Repository;

namespace Application.Queries
{
    public class StaffFreeSpecification : SpecificationBase<Staff>
    {
        public StaffFreeSpecification() : base()
        {
            AddCriteria(c => c.DateDeleted == 0 && !c.IsAssigned);

            AddInclude(c => c.Shifts);
            AddInclude(c => c.ProductLevels);
            AddInclude(c => c.Roles);
            AddInclude(c => c.Groups);
        }
    }
}
