using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework.Contexts;
using Entities.Concrete;

namespace DataAccess.Concrete.EntityFramework
{
    public class ProjectPlatformRepository : EfEntityRepositoryBase<ProjectPlatform, ProjectDbContext>, IProjectPlatformRepository
    {
        public ProjectPlatformRepository(ProjectDbContext context) : base(context)
        {
        }
    }
}