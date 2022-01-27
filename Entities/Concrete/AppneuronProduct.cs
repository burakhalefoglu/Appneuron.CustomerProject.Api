using Core.Entities;

#nullable disable

namespace Entities.Concrete
{
    public class AppneuronProduct : DocumentDbEntity
    {
        public bool Status = true;
        public string ProductName { get; set; }
    }
}