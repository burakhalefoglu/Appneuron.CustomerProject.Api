using Business.Handlers.GamePlatforms.Commands;
using FluentValidation;

namespace Business.Handlers.GamePlatforms.ValidationRules
{
    public class CreateGamePlatformValidator : AbstractValidator<CreateGamePlatformCommand>
    {
        public CreateGamePlatformValidator()
        {
            RuleFor(x => x.PlatformName).NotNull();
        }
    }

    public class UpdateGamePlatformValidator : AbstractValidator<UpdateGamePlatformCommand>
    {
        public UpdateGamePlatformValidator()
        {
            RuleFor(x => x.PlatformName).NotEmpty();
            RuleFor(x => x.PlatformDescription).NotEmpty();
        }
    }
}