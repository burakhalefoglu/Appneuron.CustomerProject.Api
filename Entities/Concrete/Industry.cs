using Core.Entities;

#nullable disable

namespace Entities.Concrete
{
    public class Industry : IEntity
    {
        public bool Status = true;
        public string Name { get; set; }
        public long Id { get; set; }
    }
}