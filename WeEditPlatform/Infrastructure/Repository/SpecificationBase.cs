using System.Linq.Expressions;

namespace Infrastructure.Repository
{
    public abstract class SpecificationBase<T> : ISpecification<T>
    {
        private Func<T, bool> _compiledExpression;

        public bool IsInclude { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecificationBase{T}"/> class.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        protected SpecificationBase(Expression<Func<T, bool>> criteria, bool isInclude)
        {
            Criteria = criteria;
            IsInclude = isInclude;
        }

        protected SpecificationBase(Expression<Func<T, bool>> criteria)
        {
            Criteria = c => true;
        }

        /// <summary>
        /// Default do not filter
        /// </summary>
        protected SpecificationBase(bool isInclude)
        {
            Criteria = c => true;
            IsInclude = isInclude;
        }

        protected SpecificationBase()
        {
            Criteria = c => true;
            IsInclude = true;
        }

        /// <summary>
        /// Gets the criteria.
        /// </summary>
        /// <value>The criteria.</value>
        public Expression<Func<T, bool>> Criteria { get; private set; }

        // TODO: And condition
        public List<Expression<Func<T, bool>>> Criterias { get; private set; }

        public Expression<Func<T, object>> OrderBy { get; private set; }
        public Expression<Func<T, object>> OrderByDescending { get; private set; }

        protected virtual void AddCriteria(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }

        /// <summary>
        /// ref: https://www.codeproject.com/Articles/895951/Combining-expressions-to-dynamically-append-criter
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Expression<Func<T, Boolean>> OrElse(Expression<Func<T, Boolean>> left, Expression<Func<T, Boolean>> right)
        {
            Expression<Func<T, Boolean>> combined = Expression.Lambda<Func<T, Boolean>>(
                Expression.OrElse(
                    left.Body,
                    new ExpressionParameterReplacer(right.Parameters, left.Parameters).Visit(right.Body)
                    ), left.Parameters);

            return combined;
        }


        public static Expression<Func<T, Boolean>> AndAlso(Expression<Func<T, Boolean>> left, Expression<Func<T, Boolean>> right)
        {
            Expression<Func<T, Boolean>> combined = Expression.Lambda<Func<T, Boolean>>(
                Expression.AndAlso(
                    left.Body,
                    new ExpressionParameterReplacer(right.Parameters, left.Parameters).Visit(right.Body)
                    ), left.Parameters);

            return combined;
        }

        /// <summary>
        /// Gets the compiled expression.
        /// </summary>
        /// <value>The compiled expression.</value>
        private Func<T, bool> CompiledExpression => _compiledExpression ?? (_compiledExpression = Criteria.Compile());

        /// <summary>
        /// Determines whether [is satisfied by] [the specified entity].
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns><c>true</c> if [is satisfied by] [the specified entity]; otherwise, <c>false</c>.</returns>
        public bool IsSatisfiedBy(T entity)
        {
            return CompiledExpression(entity);
        }

        /// <summary>
        /// Gets the includes.
        /// </summary>
        /// <value>The includes.</value>
        public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();

        /// <summary>
        /// Gets the include strings.
        /// </summary>
        /// <value>The include strings.</value>
        public List<string> IncludeStrings { get; } = new List<string>();

        /// <summary>
        /// Adds the include.
        /// </summary>
        /// <param name="includeExpression">The include expression.</param>
        protected virtual void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }

        /// <summary>
        /// Adds the include.
        /// </summary>
        /// <param name="includeString">The include string.</param>
        protected virtual void AddInclude(string includeString)
        {
            IncludeStrings.Add(includeString);
        }

        protected void AddOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }
        protected void AddOrderByDescending(Expression<Func<T, object>> orderByDescExpression)
        {
            OrderByDescending = orderByDescExpression;
        }
    }

    public class ExpressionParameterReplacer : ExpressionVisitor
    {
        private IDictionary<ParameterExpression, ParameterExpression> ParameterReplacements { get; set; }

        public ExpressionParameterReplacer
        (IList<ParameterExpression> fromParameters, IList<ParameterExpression> toParameters)
        {
            ParameterReplacements = new Dictionary<ParameterExpression, ParameterExpression>();

            for (int i = 0; i != fromParameters.Count && i != toParameters.Count; i++)
            { ParameterReplacements.Add(fromParameters[i], toParameters[i]); }
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            ParameterExpression replacement;

            if (ParameterReplacements.TryGetValue(node, out replacement))
            { node = replacement; }

            return base.VisitParameter(node);
        }
    }
}
