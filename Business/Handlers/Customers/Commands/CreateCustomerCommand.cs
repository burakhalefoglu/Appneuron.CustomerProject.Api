using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.Customers.ValidationRules;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Business.Handlers.Customers.Commands
{
    /// <summary>
    /// </summary>
    public class CreateCustomerCommand : IRequest<IResult>
    {
        public short CustomerScaleId { get; set; }
        public short IndustryId { get; set; }

        public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, IResult>
        {
            private readonly ICustomerRepository _customerRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly IMediator _mediator;

            public CreateCustomerCommandHandler(ICustomerRepository customerRepository,
                IMediator mediator,
                IHttpContextAccessor httpContextAccessor)
            {
                _customerRepository = customerRepository;
                _mediator = mediator;
                _httpContextAccessor = httpContextAccessor;
            }

            [ValidationAspect(typeof(CreateCustomerValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
            {
                var userId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.Claims
                    .FirstOrDefault(x => x.Type.EndsWith("nameidentifier"))?.Value);

                var isCustomerExist = await _customerRepository.GetAsync(c => c.UserId == userId);
                if (isCustomerExist != null) return new ErrorResult(Messages.CustomerNotFound);
                var addedCustomer = new Customer
                {
                    UserId = userId,
                    CustomerScaleId = request.CustomerScaleId,
                    IndustryId = request.IndustryId
                };

                _customerRepository.Add(addedCustomer);
                await _customerRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Added);
            }
        }
    }
}