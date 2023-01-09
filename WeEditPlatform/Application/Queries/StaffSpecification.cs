using Domain;
using Infrastructure.Models;
using Infrastructure.Pagging;
using Infrastructure.Repository;
using System.Linq.Expressions;

namespace Application.Queries
{
    public class StaffSpecification : SpecificationBase<Staff>
    {
        public StaffSpecification(bool isInclude, string? searchValue = "", List<ColumnOrder>? columnOrders = null) : base(isInclude)
        {
            // filter
            if (!string.IsNullOrEmpty(searchValue))
            {
                searchValue = searchValue.ToLower().Trim();
                Expression<Func<Staff, bool>> criteria = c =>
                    searchValue.Contains(c.FullName.ToLower()) || searchValue.Contains(c.Account.ToLower() ?? "")
                    || searchValue.Contains(c.Email.ToLower() ?? "");

                AddCriteria(criteria);
            }

            // order by columns
            if (columnOrders is not null && columnOrders.Any())
            {
                foreach (var columnOrder in columnOrders)
                {
                    switch (columnOrder.Name)
                    {
                        case nameof(Staff.FullName):
                            if (columnOrder.Order == Order.DESC)
                            {
                                AddOrderByDescending(w => w.FullName ?? "");
                            }
                            else
                            {
                                AddOrderBy(w => w.FullName ?? "");
                            }

                            break;

                        case nameof(Staff.Id):
                            if (columnOrder.Order == Order.DESC)
                            {
                                AddOrderByDescending(w => w.Id);
                            }
                            else
                            {
                                AddOrderBy(w => w.Id);
                            }

                            break;

                        case nameof(Staff.Email):
                            if (columnOrder.Order == Order.DESC)
                            {
                                AddOrderByDescending(w => w.Email);
                            }
                            else
                            {
                                AddOrderBy(w => w.Email);
                            }

                            break;

                        case nameof(Staff.Account):
                            if (columnOrder.Order == Order.DESC)
                            {
                                AddOrderByDescending(w => w.Account);
                            }
                            else
                            {
                                AddOrderBy(w => w.Account);
                            }

                            break;

                        default:
                            AddOrderByDescending(w => w.DateCreated);
                            break;
                    }
                }
            }

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
