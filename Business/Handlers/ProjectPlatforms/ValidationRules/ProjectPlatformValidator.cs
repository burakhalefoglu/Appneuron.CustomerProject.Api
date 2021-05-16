using Business.Handlers.ProjectPlatforms.Commands;
using FluentValidation;

namespace Business.Handlers.ProjectPlatforms.ValidationRules
{
    public class CreateProjectPlatformValidator : AbstractValidator<CreateProjectPlatformCommand>
    {
        public CreateProjectPlatformValidator()
        {
            RuleFor(x => x.ProjectId).NotEmpty();
            RuleFor(x => x.GamePlatformId).NotEmpty();
        }
    }

    public class UpdateProjectPlatformValidator : AbstractValidator<UpdateProjectPlatformCommand>
    {
        public UpdateProjectPlatformValidator()
        {
            RuleFor(x => x.ProjectId).NotEmpty();
            RuleFor(x => x.GamePlatformId).NotEmpty();
        }
    }
}