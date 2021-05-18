using Business.BusinessAspects;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.IoC;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.Customers.Queries
{
    public class GetCustomerQuery : IRequest<IDataResult<Customer>>
    {
        public class GetCustomerQueryHandler : IRequestHandler<GetCustomerQuery, IDataResult<Customer>>
        {
            private readonly ICustomerRepository _customerRepository;
            private readonly IMediator _mediator;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public GetCustomerQueryHandler(ICustomerRepository customerRepository, IMediator mediator)
            {
                _customerRepository = customerRepository;
                _mediator = mediator;
                _httpContextAccessor = ServiceTool.ServiceProvider.GetService<IHttpContextAccessor>();
            }

            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<Customer>> Handle(GetCustomerQuery request, CancellationToken cancellationToken)
            {
                int userId = Int32.Parse(_httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type.EndsWith("nameidentifier"))?.Value);

                var customer = await _customerRepository.GetAsync(p => p.UserId == userId);
                return new SuccessDataResult<Customer>(customer);
            }
        }
    }
}