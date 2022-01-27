using Core.Entities;

#nullable disable

namespace Entities.Concrete
{
    public class CustomerProjectHasProduct : DocumentDbEntity
    {
        public string ProductId { get; set; }
        public string ProjectId { get; set; }
    }
}