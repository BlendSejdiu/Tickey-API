using Tickey.Models.Enum;

namespace Tickey.Models.DTO.EventDTOs;

public record UpdateEventDTO(string? Name,
                            string? Description,
                            DateTime? EventDateTime,
                            string? Location,
                            int? TotalTickets,
                            Category? EventCategory);