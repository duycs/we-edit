using Domain;
using Infrastructure.Repository;
using System.Linq.Expressions;

namespace Application.Queries
{
    public class NoteSpecification : SpecificationBase<Note>
    {
        public NoteSpecification(string objectName, int[] objectIds, string? searchValue = "") : base()
        {
            Expression<Func<Note, bool>> expression = n =>
                n.DateDeleted == 0 && n.ObjectName.ToLower().Trim() == objectName.ToLower().Trim()
                && objectIds.Contains(n.ObjectId);

            if (!string.IsNullOrEmpty(searchValue))
            {
                Expression<Func<Note, bool>> searchExpression = n => n.Title.ToLower().Contains(searchValue.ToLower())
                    || n.Description.ToLower().Contains(searchValue.ToLower());
                expression = AndAlso(expression, searchExpression);
            }

            AddCriteria(expression);
        }
    }
}
