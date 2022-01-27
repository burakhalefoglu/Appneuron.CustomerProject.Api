using Core.DataAccess;
using Entities.Concrete;

namespace DataAccess.Abstract
{
    public interface IIndustryRepository : IDocumentDbRepository<Industry>
    {
    }
}