using Core.DataAccess.EntityFramework;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework.Contexts;
using Entities.Concrete;

namespace DataAccess.Concrete.EntityFramework
{
    public class IndustryRepository : EfEntityRepositoryBase<Industry, ProjectDbContext>, IIndustryRepository
    {
        public IndustryRepository(ProjectDbContext context) : base(context)
        {
        }
    }
}