namespace Business.MessageBrokers.Models;

public class CustomerProjectMetadata
{
    public long Id { get; set; }
    public long ProjectId { get; set; }
    public short IndustryId { get; set; }
    public short ProductId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public bool Status { get; set; }
}