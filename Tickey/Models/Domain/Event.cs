using Tickey.Models.Enum;

namespace Tickey.Models.Domain;

public class Event
{
    //General
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime EventDateTime { get; set; }
    public string Location { get; set; }
    public int TotalTickets { get; set; }
    public Category EventCategory { get; set; }

    //Audit
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation Properties
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
