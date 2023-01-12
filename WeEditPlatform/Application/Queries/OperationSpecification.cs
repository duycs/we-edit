using Domain;
using Infrastructure.Models;
using Infrastructure.Pagging;
using Infrastructure.Repository;
using System.Linq.Expressions;

namespace Application.Queries
{
    public class OperationSpecification : SpecificationBase<Operation>
    {
        public OperationSpecification(bool isInclude, string? searchValue = "", List<ColumnOrder>? columnOrders = null) : base(isInclude)
        {
            Expression<Func<Operation, bool>> expression = c => c.DateDeleted == 0;

            // filter
            if (!string.IsNullOrEmpty(searchValue))
            {
                searchValue = searchValue.ToLower().Trim();
                Expression<Func<Operation, bool>> searchExpression = c =>
                    searchValue.Contains(c.Name.ToLower() ?? "") || searchValue.Contains(c.Description.ToLower() ?? "")
                    || searchValue.Contains(c.ExecutionName.ToLower() ?? "");

                expression = AndAlso(expression, searchExpression);
            }

            AddCriteria(expression);

            // order by columns
            if (columnOrders is not null && columnOrders.Any())
            {
                foreach (var columnOrder in columnOrders)
                {
                    switch (columnOrder.Name)
                    {
                        case nameof(Operation.Name):
                            if (columnOrder.Order == Order.DESC)
                            {
                                AddOrderByDescending(w => w.Name ?? "");
                            }
                            else
                            {
                                AddOrderBy(w => w.Name ?? "");
                            }

                            break;

                        case nameof(Operation.ExecutionName):
                            if (columnOrder.Order == Order.DESC)
                            {
                                AddOrderByDescending(w => w.ExecutionName ?? "");
                            }
                            else
                            {
                                AddOrderBy(w => w.ExecutionName ?? "");
                            }

                            break;


                        case nameof(Operation.Id):
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
                AddInclude(w => w.Settings);
            }
        }
    }
}