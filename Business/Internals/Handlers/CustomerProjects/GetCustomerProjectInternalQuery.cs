using System.Threading;
using System.Threading.Tasks;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;

namespace Business.Internals.Handlers.CustomerProjects
{
    public class GetCustomerProjectInternalQuery : IRequest<IDataResult<CustomerProject>>
    {
        public long ProjectId { get; set; }

        public class
            GetCustomerProjectQueryHandler : IRequestHandler<GetCustomerProjectInternalQuery,
                IDataResult<CustomerProject>>
        {
            private readonly ICustomerProjectRepository _customerProjectRepository;

            public GetCustomerProjectQueryHandler(ICustomerProjectRepository customerProjectRepository
            )
            {
                _customerProjectRepository = customerProjectRepository;
            }

            public async Task<IDataResult<CustomerProject>> Handle(GetCustomerProjectInternalQuery request,
                CancellationToken cancellationToken)
            {
                var customerProject =
                    await _customerProjectRepository.GetAsync(p => p.Id == request.ProjectId);
                return new SuccessDataResult<CustomerProject>(customerProject);
            }
        }
    }
}