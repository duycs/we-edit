using Domain;
using Infrastructure.Models;
using Infrastructure.Pagging;
using Infrastructure.Repository;
using System.Linq.Expressions;

namespace Application.Queries
{
    public class ProductLevelSpecification : SpecificationBase<ProductLevel>
    {

        public ProductLevelSpecification(bool isInclude, string? searchValue = "", List<ColumnOrder>? columnOrders = null) : base(isInclude)
        {

            // filter
            if (!string.IsNullOrEmpty(searchValue))
            {
                searchValue = searchValue.ToLower().Trim();
                Expression<Func<ProductLevel, bool>> criteria = c =>
                    searchValue.Contains(c.Code.ToLower()) || searchValue.Contains(c.Name.ToLower() ?? "") || searchValue.Contains(c.Description.ToLower() ?? "");

                AddCriteria(criteria);
            }

            // order by columns
            if (columnOrders is not null && columnOrders.Any())
            {
                foreach (var columnOrder in columnOrders)
                {
                    switch (columnOrder.Name)
                    {
                        case nameof(ProductLevel.Code):
                            if (columnOrder.Order == Order.DESC)
                            {
                                AddOrderByDescending(w => w.Code ?? "");
                            }
                            else
                            {
                                AddOrderBy(w => w.Code ?? "");
                            }

                            break;

                        case nameof(ProductLevel.Name):
                            if (columnOrder.Order == Order.DESC)
                            {
                                AddOrderByDescending(w => w.Name ?? "");
                            }
                            else
                            {
                                AddOrderBy(w => w.Name ?? "");
                            }

                            break;

                        case nameof(ProductLevel.Id):
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
                AddInclude(w => w.Jobs);
                AddInclude(w => w.Staffs);
            }
        }
    }
}