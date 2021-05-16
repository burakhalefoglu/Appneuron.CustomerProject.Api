using Core.Entities;

#nullable disable

namespace Entities.Concrete
{
    public class ProjectPlatform : IEntity
    {
        public long Id { get; set; }
        public long ProjectId { get; set; }
        public short GamePlatformId { get; set; }

        public virtual GamePlatform GamePlatform { get; set; }
        public virtual CustomerProject Project { get; set; }
    }
}