using Business.Handlers.Industries.Commands;
using FluentValidation;

namespace Business.Handlers.Industries.ValidationRules
{
    public class CreateIndustryValidator : AbstractValidator<CreateIndustryCommand>
    {
        public CreateIndustryValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }

    public class UpdateIndustryValidator : AbstractValidator<UpdateIndustryCommand>
    {
        public UpdateIndustryValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}