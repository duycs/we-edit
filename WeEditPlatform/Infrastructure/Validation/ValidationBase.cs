using FluentValidation;

namespace Infrastructure.Validation
{
    public abstract class ValidationBase<T> : AbstractValidator<T>
    {
    }
}
