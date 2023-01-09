using FluentValidation;
using Infrastructure.Validation;

namespace Application.Models
{
    public class CreateStaffVMValidator : ValidationBase<CreateStaffVM>
    {
        public CreateStaffVMValidator()
        {
            RuleFor(v => v.Email).NotNull().NotEmpty().EmailAddress();
        }
    }
}
