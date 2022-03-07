using Business.Handlers.Feedbacks.Commands;
using FluentValidation;

namespace Business.Handlers.Feedbacks.ValidationRules;

    public class FeedbackValidator : AbstractValidator<CreateFeedbackCommand>
    {
        public FeedbackValidator()
        {
            RuleFor(x => x.Message).NotEmpty();
        }
    }
