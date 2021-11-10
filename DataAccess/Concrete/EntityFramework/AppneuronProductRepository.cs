using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework.Contexts;
using Entities.Concrete;

namespace DataAccess.Concrete.EntityFramework
{
    public class AppneuronProductRepository : EfEntityRepositoryBase<AppneuronProduct, ProjectDbContext>,
        IAppneuronProductRepository
    {
        public AppneuronProductRepository(ProjectDbContext context) : base(context)
        {
        }
    }
}