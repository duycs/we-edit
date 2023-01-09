using Domain;
using Infrastructure.Models;
using Infrastructure.Pagging;
using Infrastructure.Repository;
using System.Linq.Expressions;

namespace Application.Queries
{
    public class JobSpecification : SpecificationBase<Job>
    {
        public JobSpecification(bool isInclude, string? searchValue = "", List<ColumnOrder>? columnOrders = null) : base(isInclude)
        {
            // filter
            if (!string.IsNullOrEmpty(searchValue))
            {
                searchValue = searchValue.ToLower().Trim();
                Expression<Func<Job, bool>> criteria = c =>
                    searchValue.Contains(c.Code.ToLower()) || searchValue.Contains(c.Instruction.ToLower() ?? "");

                AddCriteria(criteria);
            }

            // order by columns
            if (columnOrders is not null && columnOrders.Any())
            {
                foreach (var columnOrder in columnOrders)
                {
                    switch (columnOrder.Name)
                    {
                        case nameof(Job.Code):
                            if (columnOrder.Order == Order.DESC)
                            {
                                AddOrderByDescending(w => w.Code ?? "");
                            }
                            else
                            {
                                AddOrderBy(w => w.Code ?? "");
                            }

                            break;

                        case nameof(Job.Id):
                            if (columnOrder.Order == Order.DESC)
                            {
                                AddOrderByDescending(w => w.Id);
                            }
                            else
                            {
                                AddOrderBy(w => w.Id);
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
                AddInclude(w => w.Steps);
                AddInclude(w => w.JobSteps);
                AddInclude(w => w.ProductLevel);
                AddInclude(w => w.CSO);
            }
        }
    }
}
