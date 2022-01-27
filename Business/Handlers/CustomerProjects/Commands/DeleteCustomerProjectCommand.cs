using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Business.Handlers.CustomerProjects.Commands
{
    /// <summary>
    /// </summary>
    public class DeleteCustomerProjectCommand : IRequest<IResult>
    {
        public string Id { get; set; }

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
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteCustomerProjectCommand request, CancellationToken cancellationToken)
            {
                var userId = _httpContextAccessor.HttpContext?.User.Claims
                    .FirstOrDefault(x => x.Type.EndsWith("nameidentifier"))?.Value;

                var customerProjectToDelete =
                    await _customerProjectRepository.GetAsync(p =>
                        p.ProjectId == request.Id && p.CustomerId == userId);
                if (customerProjectToDelete == null) return new ErrorResult(Messages.ProjectNotFound);
                customerProjectToDelete.Status = false;
                await _customerProjectRepository.UpdateAsync(customerProjectToDelete,
                    x => x.ObjectId == customerProjectToDelete.ObjectId);
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}