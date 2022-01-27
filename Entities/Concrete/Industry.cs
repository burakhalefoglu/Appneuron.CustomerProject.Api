using Core.Entities;

#nullable disable

namespace Entities.Concrete
{
    public class Industry : DocumentDbEntity
    {
        public bool Status = true;
        public string Name { get; set; }
    }
}