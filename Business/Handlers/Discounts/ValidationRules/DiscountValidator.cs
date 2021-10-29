using Business.Handlers.Discounts.Commands;
using FluentValidation;

namespace Business.Handlers.Discounts.ValidationRules
{
    public class CreateDiscountValidator : AbstractValidator<CreateDiscountCommand>
    {
        public CreateDiscountValidator()
        {
            RuleFor(x => x.DiscountName).NotEmpty();
            RuleFor(x => x.Percent).NotEmpty();
        }
    }

    public class UpdateDiscountValidator : AbstractValidator<UpdateDiscountCommand>
    {
        public UpdateDiscountValidator()
        {
            RuleFor(x => x.DiscountName).NotEmpty();
            RuleFor(x => x.Percent).NotEmpty();
        }
    }
}