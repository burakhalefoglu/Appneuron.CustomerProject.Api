using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using Core.Utilities.Security.Models;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using Microsoft.AspNetCore.Http;
using IResult = Core.Utilities.Results.IResult;

namespace Business.Handlers.CustomerProjects.Commands
{
    /// <summary>
    /// </summary>
    public class DeleteCustomerProjectCommand : IRequest<IResult>
    {
        public string Name { get; set; }

        public class DeleteCustomerProjectCommandHandler : IRequestHandler<DeleteCustomerProjectCommand, IResult>
        {
            private readonly ICustomerProjectRepository _customerProjectRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public DeleteCustomerProjectCommandHandler(ICustomerProjectRepository customerProjectRepository,
                IHttpContextAccessor httpContextAccessor)
            {
                _customerProjectRepository = customerProjectRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(ConsoleLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteCustomerProjectCommand request, CancellationToken cancellationToken)
            {
                var userId = _httpContextAccessor.HttpContext?.User.Claims
                    .FirstOrDefault(x => x.Type.EndsWith("nameidentifier"))?.Value;

                var customerProjectToDelete =
                    await _customerProjectRepository.GetAsync(p =>
                        p.Name == request.Name && p.CustomerId == Convert.ToInt64(userId) && p.Status == true);
                if (customerProjectToDelete == null) return new ErrorDataResult<AccessToken>(Messages.ProjectNotFound);
                customerProjectToDelete.Status = false;
                
                await _customerProjectRepository.UpdateAsync(customerProjectToDelete);
                
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}