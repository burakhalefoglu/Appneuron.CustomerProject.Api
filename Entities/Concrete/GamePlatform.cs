using Core.Entities;

#nullable disable

namespace Entities.Concrete
{
    public class GamePlatform : IEntity
    {
        public bool Status = true;
        public string PlatformName { get; set; }
        public string PlatformDescription { get; set; }
        public long Id { get; set; }
    }
}