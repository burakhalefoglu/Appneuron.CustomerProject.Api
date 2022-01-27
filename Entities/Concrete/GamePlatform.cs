using Core.Entities;

#nullable disable

namespace Entities.Concrete
{
    public class GamePlatform : DocumentDbEntity
    {
        public bool Status = true;
        public string PlatformName { get; set; }
        public string PlatformDescription { get; set; }
    }
}