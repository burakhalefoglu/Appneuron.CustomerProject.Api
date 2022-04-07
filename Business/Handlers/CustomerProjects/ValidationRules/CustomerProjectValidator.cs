using Business.Handlers.CustomerProjects.Commands;
using FluentValidation;

namespace Business.Handlers.CustomerProjects.ValidationRules;

public class CreateCustomerProjectValidator : AbstractValidator<CreateCustomerProjectCommand>
{
    public CreateCustomerProjectValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
    }
}