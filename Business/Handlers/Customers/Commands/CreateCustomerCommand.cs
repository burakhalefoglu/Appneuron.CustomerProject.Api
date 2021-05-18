using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.Customers.ValidationRules;
using Business.Handlers.CustomerScales.Queries;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.IoC;
using Core.Utilities.Results;
using Core.Utilities.Security.Encyption;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.Customers.Commands
{
    /// <summary>
    ///
    /// </summary>
    public class CreateCustomerCommand : IRequest<IResult>
    {
        public short CustomerScaleId { get; set; }
        public short IndustryId { get; set; }

        public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, IResult>
        {
            private readonly ICustomerRepository _customerRepository;
            private readonly IMediator _mediator;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public CreateCustomerCommandHandler(ICustomerRepository customerRepository, IMediator mediator)
            {
                _customerRepository = customerRepository;
                _mediator = mediator;
                _httpContextAccessor = ServiceTool.ServiceProvider.GetService<IHttpContextAccessor>();
            }

            [ValidationAspect(typeof(CreateCustomerValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]


            public async Task<IResult> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
            {
                var userId = int.Parse(_httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type.EndsWith("nameidentifier"))?.Value);


                var addedCustomer = new Customer
                {
                    UserId = userId,
                    CustomerScaleId = request.CustomerScaleId,
                    IndustryId = request.IndustryId,
                    DashboardKey = SecurityKeyHelper.GetRandomStringNumber(64)
                };

                _customerRepository.Add(addedCustomer);
                await _customerRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Added);
            }
        }
    }
}