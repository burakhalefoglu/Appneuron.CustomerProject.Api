using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.Rates.ValidationRules;
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

namespace Business.Handlers.Rates.Commands;

public class CreateRateCommand : IRequest<IResult>
{
    public short Value { get; set; }

    public class CreateRateCommandHandler : IRequestHandler<CreateRateCommand, IResult>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMailService _mailService;
        private readonly IMediator _mediator;
        private readonly IRateRepository _rateRepository;

        public CreateRateCommandHandler(IRateRepository rateRepository,
            IMediator mediator,
            IHttpContextAccessor httpContextAccessor, IMailService mailService)
        {
            _rateRepository = rateRepository;
            _httpContextAccessor = httpContextAccessor;
            _mailService = mailService;
            _mediator = mediator;
        }

        [ValidationAspect(typeof(RateValidator), Priority = 1)]
        [CacheRemoveAspect("Get")]
        [TransactionScopeAspect]
        [LogAspect(typeof(ConsoleLogger))]
        [SecuredOperation(Priority = 1)]
        public async Task<IResult> Handle(CreateRateCommand request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext?.User.Claims
                .FirstOrDefault(x => x.Type.EndsWith("nameidentifier"))?.Value;

            await _mediator.Send(new CreateCustomerInternalCommand(), cancellationToken);

            await _rateRepository.AddAsync(new Rate
            {
                Value = request.Value,
                CustomerId = Convert.ToInt64(userId)
            });
            //send email for us..
            await _mailService.Send(new EmailMessage
            {
                Content = new TextPart(TextFormat.Html)
                    {Text = $"<p>feedback rate from userıd: {userId}, feedback: {request.Value} </p>"},
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