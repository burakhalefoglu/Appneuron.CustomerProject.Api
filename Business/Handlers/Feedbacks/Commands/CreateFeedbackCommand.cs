using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.Feedbacks.ValidationRules;
using Business.Internals.Handlers.Customers.Commands;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Transaction;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Mail;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using Microsoft.AspNetCore.Http;
using MimeKit;
using MimeKit.Text;
using IResult = Core.Utilities.Results.IResult;

namespace Business.Handlers.Feedbacks.Commands;

public class CreateFeedbackCommand : IRequest<IResult>
{
    public string Message { get; set; }

    public class CreateFeedbackCommandHandler : IRequestHandler<CreateFeedbackCommand, IResult>
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMailService _mailService;
        private readonly IMediator _mediator;

        public CreateFeedbackCommandHandler(IFeedbackRepository feedbackRepository,
            IMediator mediator,
            IHttpContextAccessor httpContextAccessor, IMailService mailService)
        {
            _feedbackRepository = feedbackRepository;
            _httpContextAccessor = httpContextAccessor;
            _mailService = mailService;
            _mediator = mediator;
        }

        [ValidationAspect(typeof(FeedbackValidator), Priority = 1)]
        [CacheRemoveAspect("Get")]
        [TransactionScopeAspect]
        [LogAspect(typeof(ConsoleLogger))]
        [SecuredOperation(Priority = 1)]
        public async Task<IResult> Handle(CreateFeedbackCommand request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext?.User.Claims
                .FirstOrDefault(x => x.Type.EndsWith("nameidentifier"))?.Value;

            await _mediator.Send(new CreateCustomerInternalCommand(), cancellationToken);

            await _feedbackRepository.AddAsync(new Feedback
            {
                Message = request.Message,
                CustomerId = Convert.ToInt64(userId)
            });
            //send email for us..
            //send email for us..
            await _mailService.Send(new EmailMessage
            {
                Content = new TextPart(TextFormat.Html)
                    {Text = $"<p>feedback detail message from userıd: {userId}, feedback: {request.Message} </p>"},
                FromAddresses =
                {
                    new EmailAddress
                    {
                        Address = "info@appneuron.com",
                        Name = "Appneuron"
                    }
                },
                Subject = "Feedback...",
                ToAddresses =
                {
                    new EmailAddress
                    {
                        Address = "info@appneuron.com",
                        Name = "no name"
                    }
                }
            });
            return new SuccessResult(Messages.Added);
        }
    }
}