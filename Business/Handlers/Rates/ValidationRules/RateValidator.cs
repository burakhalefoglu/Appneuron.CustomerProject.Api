using Business.Handlers.Rates.Commands;
using FluentValidation;

namespace Business.Handlers.Rates.ValidationRules;

    public class RateValidator : AbstractValidator<CreateRateCommand>
    {
        public RateValidator()
        {
            RuleFor(x => x.Value).NotEmpty();
        }
    }
