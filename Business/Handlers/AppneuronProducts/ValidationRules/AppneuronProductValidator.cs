using Business.Handlers.AppneuronProducts.Commands;
using FluentValidation;

namespace Business.Handlers.AppneuronProducts.ValidationRules
{
    public class CreateAppneuronProductValidator : AbstractValidator<CreateAppneuronProductCommand>
    {
        public CreateAppneuronProductValidator()
        {
            RuleFor(x => x.ProductName).NotEmpty();
        }
    }

    public class UpdateAppneuronProductValidator : AbstractValidator<UpdateAppneuronProductCommand>
    {
        public UpdateAppneuronProductValidator()
        {
            RuleFor(x => x.ProductName).NotEmpty();
        }
    }
}