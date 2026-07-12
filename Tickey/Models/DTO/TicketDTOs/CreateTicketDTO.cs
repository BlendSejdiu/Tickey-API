namespace Tickey.Models.DTO.TicketDTOs;

public record CreateTicketDTO(
    Guid EventId,
    string TicketType,
    decimal Price,
    int QuantityAvailable
);
