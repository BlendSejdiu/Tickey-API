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
                    e.EventDate,
                    e.EventTime,
                    e.Location,
                    e.TotalTickets,
                    e.EventCategory)).AsNoTracking().ToListAsync(cancellationToken);

            return Results.Ok(events);

        }).WithName("GetAllEvents").WithTags("Events").WithSummary("Get all events");


        app.MapGet("/events/{id:guid}", async (Guid id, AppDbContext db, CancellationToken cancellationToken) =>
        {
            var eventItem = await db.Events
                .Where(e => e.Id == id)
                .Select(e => new EventDTO(
                    e.Id,
                    e.Name,
                    e.Description,
                    e.EventDate,
                    e.EventTime,
                    e.Location,
                    e.TotalTickets,
                    e.EventCategory)).FirstOrDefaultAsync(cancellationToken);

            return eventItem is not null? Results.Ok(eventItem): Results.NotFound();

        }).WithName("GetEventById").WithTags("Events").WithSummary("Get an event by ID");


        app.MapPost("/events", async (CreateEventDTO dto, AppDbContext db) =>
        {
            var eventItem = new Event
            {
                Name = dto.Name,
                Description = dto.Description,
                EventDate = DateTime.SpecifyKind(dto.EventDate, DateTimeKind.Utc),
                EventTime = DateTime.SpecifyKind(dto.EventTime, DateTimeKind.Utc),
                Location = dto.Location,
                TotalTickets = dto.TotalTickets,
                EventCategory = dto.EventCategory,

                CreatedBy = "admin",
                CreatedAt = DateTimeOffset.UtcNow.UtcDateTime,
            };

            await db.Events.AddAsync(eventItem);
            await db.SaveChangesAsync();


            return Results.Created($"/events/{eventItem.Id}", eventItem.Id);

        }).WithName("CreateEvent").WithTags("Events").WithSummary("Create a new event");


        app.MapPut("/events/{id:guid}", async (Guid id, UpdateEventDTO dto, AppDbContext db) =>
        {
            var eventItem = await db.Events.FindAsync(id);

            if (eventItem is null)
                return Results.NotFound();

            eventItem.Name = dto.Name ?? eventItem.Name;
            eventItem.Description = dto.Description ?? eventItem.Description;
            eventItem.EventDate = dto.EventDate ?? eventItem.EventDate;
            eventItem.EventTime = dto.EventTime ?? eventItem.EventTime;
            eventItem.Location = dto.Location ?? eventItem.Location;
            eventItem.TotalTickets = dto.TotalTickets ?? eventItem.TotalTickets;
            eventItem.EventCategory = dto.EventCategory ?? eventItem.EventCategory;

            eventItem.UpdatedBy = "admin";
            eventItem.UpdatedAt = DateTimeOffset.UtcNow.UtcDateTime;

            await db.SaveChangesAsync();

            var response = new EventDTO(
                eventItem.Id,
                eventItem.Name,
                eventItem.Description,
                eventItem.EventDate,
                eventItem.EventTime,
                eventItem.Location,
                eventItem.TotalTickets,
                eventItem.EventCategory);

            return Results.Ok(response);

        }).WithName("UpdateEvent").WithTags("Events").WithSummary("Update an event by ID");


        app.MapDelete("/events/{id:guid}", async (Guid id, AppDbContext db) =>
        {
            var eventItem = await db.Events.FindAsync(id);

            if (eventItem is null)
                return Results.NotFound();

            db.Events.Remove(eventItem);
            await db.SaveChangesAsync();

            return Results.NoContent();

        }).WithName("DeleteEvent").WithTags("Events").WithSummary("Delete an event by ID");

    }
}