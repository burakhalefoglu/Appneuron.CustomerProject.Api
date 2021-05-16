using Business.Handlers.CustomerDiscounts.Commands;
using FluentValidation;

namespace Business.Handlers.CustomerDiscounts.ValidationRules
{
    public class CreateCustomerDiscountValidator : AbstractValidator<CreateCustomerDiscountCommand>
    {
        public CreateCustomerDiscountValidator()
        {
            RuleFor(x => x.CustomerId).NotEmpty();
            RuleFor(x => x.DiscountId).NotEmpty();
        }
    }

    public class UpdateCustomerDiscountValidator : AbstractValidator<UpdateCustomerDiscountCommand>
    {
        public UpdateCustomerDiscountValidator()
        {
            RuleFor(x => x.CustomerId).NotEmpty();
            RuleFor(x => x.DiscountId).NotEmpty();
        }
    }
}