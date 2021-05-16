using Business.Handlers.Votes.Commands;
using FluentValidation;

namespace Business.Handlers.Votes.ValidationRules
{
    public class CreateVoteValidator : AbstractValidator<CreateVoteCommand>
    {
        public CreateVoteValidator()
        {
            RuleFor(x => x.VoteName).NotEmpty();
            RuleFor(x => x.VoteValue).NotEmpty();
            RuleFor(x => x.CustomerProjects).NotEmpty();
        }
    }

    public class UpdateVoteValidator : AbstractValidator<UpdateVoteCommand>
    {
        public UpdateVoteValidator()
        {
            RuleFor(x => x.VoteName).NotEmpty();
            RuleFor(x => x.VoteValue).NotEmpty();
            RuleFor(x => x.CustomerProjects).NotEmpty();
        }
    }
}