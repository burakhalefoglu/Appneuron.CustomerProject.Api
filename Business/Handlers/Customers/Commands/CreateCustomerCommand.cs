using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using Microsoft.AspNetCore.Http;
using IResult = Core.Utilities.Results.IResult;

namespace Business.Handlers.Customers.Commands
{
    /// <summary>
    /// </summary>
    public class CreateCustomerCommand : IRequest<IResult>
    {
        public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, IResult>
        {
            private readonly ICustomerRepository _customerRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public CreateCustomerCommandHandler(ICustomerRepository customerRepository,
                IHttpContextAccessor httpContextAccessor)
            {
                _customerRepository = customerRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ConsoleLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
            {
                var userId = _httpContextAccessor.HttpContext?.User.Claims
                    .FirstOrDefault(x => x.Type.EndsWith("nameidentifier"))?.Value;

                var isCustomerExist = await _customerRepository.GetAsync(c => c.Id == Convert.ToInt64(userId) && c.Status == true);
                if (isCustomerExist != null) return new ErrorResult(Messages.AlreadyExist);
                var addedCustomer = new Customer
                {
                    Id = Convert.ToInt64(userId),
                };

                await _customerRepository.AddAsync(addedCustomer);
                return new SuccessResult(Messages.Added);
            }
        }
    }
}