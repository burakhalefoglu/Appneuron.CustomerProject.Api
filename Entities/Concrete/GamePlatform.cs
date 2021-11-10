using System.Collections.Generic;
using Core.Entities;

#nullable disable

namespace Entities.Concrete
{
    public class GamePlatform : IEntity
    {
        public short Id { get; set; }
        public string PlatformName { get; set; }
        public string PlatformDescription { get; set; }

        public virtual ICollection<ProjectPlatform> ProjectPlatforms { get; set; }
    }
}