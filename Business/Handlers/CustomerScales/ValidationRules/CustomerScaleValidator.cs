using Business.Handlers.CustomerScales.Commands;
using FluentValidation;

namespace Business.Handlers.CustomerScales.ValidationRules
{
    public class CreateCustomerScaleValidator : AbstractValidator<CreateCustomerScaleCommand>
    {
        public CreateCustomerScaleValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
        }
    }

    public class UpdateCustomerScaleValidator : AbstractValidator<UpdateCustomerScaleCommand>
    {
        public UpdateCustomerScaleValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
        }
    }
}