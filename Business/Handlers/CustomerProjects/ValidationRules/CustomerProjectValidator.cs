using Business.Handlers.CustomerProjects.Commands;
using FluentValidation;

namespace Business.Handlers.CustomerProjects.ValidationRules
{
    public class CreateCustomerProjectValidator : AbstractValidator<CreateCustomerProjectCommand>
    {
        public CreateCustomerProjectValidator()
        {
            RuleFor(x => x.ProjectName).NotEmpty();
        }
    }

    public class UpdateCustomerProjectValidator : AbstractValidator<UpdateCustomerProjectCommand>
    {
        public UpdateCustomerProjectValidator()
        {
            RuleFor(x => x.ProjectId).NotEmpty();
            RuleFor(x => x.ProjectName).NotEmpty();
            RuleFor(x => x.Statuse).NotNull();
        }
    }
}