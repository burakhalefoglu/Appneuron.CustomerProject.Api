using Core.Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace Tests.Helpers.MockInterfaces
{
    public interface IDbSets
    {
        DbSet<Log> Logs { get; set; }
    }
}