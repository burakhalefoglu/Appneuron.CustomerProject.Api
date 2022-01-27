using Core.Entities;

#nullable disable

namespace Entities.Concrete
{
    public class CustomerScale : DocumentDbEntity
    {
        public bool Status = true;
        public string Name { get; set; }
        public string Description { get; set; }
    }
}