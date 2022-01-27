using Core.Entities;

#nullable disable

namespace Entities.Concrete
{
    public class Vote : DocumentDbEntity
    {
        public bool Status = true;
        public string VoteName { get; set; }
        public short VoteValue { get; set; }
    }
}