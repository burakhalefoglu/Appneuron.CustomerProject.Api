using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework.Contexts;
using Entities.Concrete;

namespace DataAccess.Concrete.EntityFramework
{
    public class GamePlatformRepository : EfEntityRepositoryBase<GamePlatform, ProjectDbContext>,
        IGamePlatformRepository
    {
        public GamePlatformRepository(ProjectDbContext context) : base(context)
        {
        }
    }
}