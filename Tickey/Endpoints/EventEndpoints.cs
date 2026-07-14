namespace Tickey.Endpoints;

public static class EventEndpoints
{
    public static void MapEventEndpoints(this WebApplication app)
    {
        app.MapGet("/events", async (AppDbContext db, CancellationToken cancellationToken) =>
        {
            var events = await db.Events
                .Select(e => new EventDTO(
                    e.Id,
                    e.Name,
                    e.Description,
                    e.EventDateTime,
                    e.Location,
                    e.TotalTickets,
                    e.EventCategory)).AsNoTracking().ToListAsync(cancellationToken);

            return Results.Ok(events);

        }).RequireRateLimiting("Fixed").WithName("GetAllEvents").WithTags("Events").WithSummary("Get all events");

        app.MapGet("/events/{id:guid}", async (Guid id, AppDbContext db, CancellationToken cancellationToken) =>
        {
            var eventItem = await db.Events
                .Where(e => e.Id == id)
                .Select(e => new EventDTO(
                    e.Id,
                    e.Name,
                    e.Description,
                    e.EventDateTime,
                    e.Location,
                    e.TotalTickets,
                    e.EventCategory)).FirstOrDefaultAsync(cancellationToken);

            return eventItem is not null? Results.Ok(eventItem): Results.NotFound();

        }).RequireRateLimiting("user-role").WithName("GetEventById").WithTags("Events").WithSummary("Get an event by ID");

        app.MapPost("/events", async (CreateEventDTO dto, AppDbContext db, ILogger<EventEndpointsLogger> logger) =>
        {
            var eventItem = new Event
            {
                Name = dto.Name,
                Description = dto.Description,
                EventDateTime = dto.EventDateTime,
                Location = dto.Location,
                TotalTickets = dto.TotalTickets,
                EventCategory = dto.EventCategory,

                CreatedBy = "admin",
                CreatedAt = DateTimeOffset.UtcNow.UtcDateTime,
            };

            await db.Events.AddAsync(eventItem);
            await db.SaveChangesAsync();

            logger.LogInformation("Event created with ID: {EventId}", eventItem.Id);

            return Results.Created($"/events/{eventItem.Id}", eventItem.Id);

        }).RequireRateLimiting("Fixed").RequireAuthorization("AdminOnly").WithName("CreateEvent").WithTags("Events").WithSummary("Create a new event");

        app.MapPut("/events/{id:guid}", async (Guid id, UpdateEventDTO dto, AppDbContext db, ILogger<EventEndpointsLogger> logger) =>
        {
            var eventItem = await db.Events.FindAsync(id);

            if (eventItem is null)
                return Results.NotFound();

            eventItem.Name = dto.Name ?? eventItem.Name;
            eventItem.Description = dto.Description ?? eventItem.Description;
            eventItem.EventDateTime = dto.EventDateTime ?? eventItem.EventDateTime;
            eventItem.Location = dto.Location ?? eventItem.Location;
            eventItem.TotalTickets = dto.TotalTickets ?? eventItem.TotalTickets;
            eventItem.EventCategory = dto.EventCategory ?? eventItem.EventCategory;

            eventItem.UpdatedBy = "admin";
            eventItem.UpdatedAt = DateTimeOffset.UtcNow.UtcDateTime;

            await db.SaveChangesAsync();

            logger.LogInformation("Event updated with ID: {EventId}", eventItem.Id);

            var response = new EventDTO(
                eventItem.Id,
                eventItem.Name,
                eventItem.Description,
                eventItem.EventDateTime,
                eventItem.Location,
                eventItem.TotalTickets,
                eventItem.EventCategory);

            return Results.Ok(response);

        }).RequireRateLimiting("Fixed").RequireAuthorization("AdminOnly").WithName("UpdateEvent").WithTags("Events").WithSummary("Update an event by ID");

        app.MapDelete("/events/{id:guid}", async (Guid id, AppDbContext db, ILogger<EventEndpointsLogger> logger) =>
        {
            var eventItem = await db.Events.FindAsync(id);

            if (eventItem is null)
                return Results.NotFound();

            db.Events.Remove(eventItem);
            await db.SaveChangesAsync();

            logger.LogInformation("Event deleted with ID: {EventId}", eventItem.Id);

            return Results.NoContent();

        }).RequireRateLimiting("Fixed").RequireAuthorization("AdminOnly").WithName("DeleteEvent").WithTags("Events").WithSummary("Delete an event by ID");

    }

    internal sealed class EventEndpointsLogger
    {
    }
}