namespace Tickey.Models.Domain;

public class Ticket
{
    //General
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public string TicketType { get; set; }
    public decimal Price { get; set; }
    public int QuantityAvailable { get; set; }
    public int? QuantitySold { get; set; }

    //Audit
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation Properties
    public Event Event { get; set; } = null!;
}
