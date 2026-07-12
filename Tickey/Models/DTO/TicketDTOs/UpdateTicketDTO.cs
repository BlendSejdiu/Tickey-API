namespace Tickey.Models.DTO.TicketDTOs;

public record UpdateTicketDTO(
    string TicketType,
    decimal Price,
    int QuantityAvailable
);