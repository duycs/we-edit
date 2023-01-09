using Domain;
using Infrastructure.Repository;
using System.Linq.Expressions;

namespace Application.Queries
{
    public class StaffByUserIdSpecification : SpecificationBase<Staff>
    {
        public StaffByUserIdSpecification(string userId, bool isInclude) : base(isInclude)
        {
            Expression<Func<Staff, bool>> expression = s =>
             s.DateDeleted == 0 && s.UserId.ToLower() == userId.ToLower();

            AddCriteria(expression);

            // include related entity
            if (IsInclude)
            {
                AddInclude(w => w.Shifts);
                AddInclude(w => w.Roles);
                AddInclude(w => w.Groups);
                AddInclude(w => w.ProductLevels);
                AddInclude(w => w.JobSteps);

                //AddInclude(w => w.StaffRoles);
                //AddInclude(w => w.StaffGroup);
                //AddInclude(w => w.StaffProductLevels);
            }
        }
    }
}
