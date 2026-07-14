using Tickey.Models.Enum;

namespace Tickey.Models.DTO.EventDTOs;

public record EventDTO(Guid Id,
                       string Name,
                       string Description,
                       DateTime EventDateTime,
                       string Location,
                       int TotalTickets,
                       Category EventCategory
                      );
