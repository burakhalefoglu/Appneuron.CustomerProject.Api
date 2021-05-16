using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework.Contexts;
using Entities.Concrete;

namespace DataAccess.Concrete.EntityFramework
{
    public class VoteRepository : EfEntityRepositoryBase<Vote, ProjectDbContext>, IVoteRepository
    {
        public VoteRepository(ProjectDbContext context) : base(context)
        {
        }
    }
}