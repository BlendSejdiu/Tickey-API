namespace Tickey.Models.DTO.TicketDTOs;

public record TicketDTO(Guid Id,
                        Guid EventId,
                        string TicketType,
                        decimal Price,
                        int QuantityAvailable,
                        int? QuantitySold
                       );

