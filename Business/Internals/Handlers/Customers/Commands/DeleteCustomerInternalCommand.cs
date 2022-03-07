using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using Microsoft.AspNetCore.Http;
using IResult = Core.Utilities.Results.IResult;

namespace Business.Internals.Handlers.Customers.Commands
{
    /// <summary>
    /// </summary>
    public class DeleteCustomerInternalCommand : IRequest<IResult>
    {
        public class DeleteCustomerInternalCommandHandler : IRequestHandler<DeleteCustomerInternalCommand, IResult>
        {
            private readonly ICustomerRepository _customerRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public DeleteCustomerInternalCommandHandler(ICustomerRepository customerRepository,
                IHttpContextAccessor httpContextAccessor)
            {
                _customerRepository = customerRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ConsoleLogger))]
            public async Task<IResult> Handle(DeleteCustomerInternalCommand request, CancellationToken cancellationToken)
            {
                var userId = _httpContextAccessor.HttpContext?.User.Claims
                    .FirstOrDefault(x => x.Type.EndsWith("nameidentifier"))?.Value;

                var customerToDelete = await _customerRepository.GetAsync(p => p.Id == Convert.ToInt64(userId) && p.Status == true);
                if (customerToDelete == null) return new ErrorResult(Messages.UserNotFound);
                customerToDelete.Status = false;
                await _customerRepository.UpdateAsync(customerToDelete);
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}