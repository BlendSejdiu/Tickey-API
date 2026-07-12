namespace Tickey.Endpoints;
public static class TicketEndpoints
{
    public static void MapTicketEndpoints(this WebApplication app)
    {
        app.MapGet("/tickets", async (AppDbContext db, CancellationToken cancellationToken) =>
        {
            var tickets = await db.Tickets
                                 .Select(t => new TicketDTO(
                                     t.Id,
                                     t.EventId,
                                     t.TicketType,
                                     t.Price,
                                     t.QuantityAvailable,
                                     t.QuantitySold)).AsNoTracking().ToListAsync(cancellationToken);

            return Results.Ok(tickets);

        }).WithName("GetAllTickets").WithTags("Tickets").WithSummary("Get all tickets");

        app.MapGet("/tickets/{id:guid}", async (Guid id, AppDbContext db, CancellationToken cancellationToken) =>
        {
            var ticket = await db.Tickets
                .Where(x => x.Id == id)
                .Select(t => new TicketDTO(
                    t.Id,
                    t.EventId,
                    t.TicketType,
                    t.Price,
                    t.QuantityAvailable,
                    t.QuantitySold)).FirstOrDefaultAsync(cancellationToken);

            return ticket is not null? Results.Ok(ticket): Results.NotFound();

        }).WithName("GetTicketById").WithTags("Tickets").WithSummary("Get a ticket by ID");


        app.MapPost("/tickets", async (CreateTicketDTO dto, AppDbContext db) =>
        {
            var @event = await db.Events.FirstOrDefaultAsync(e => e.Id == dto.EventId);

            if (@event is null)
                return Results.NotFound("EventId not found.");

            var allocatedTickets = await db.Tickets.Where(x => x.EventId == dto.EventId).SumAsync(x => x.QuantityAvailable);

            var remainingTickets = @event.TotalTickets - allocatedTickets;

            if(dto.QuantityAvailable > remainingTickets)
                return Results.BadRequest(new{ Message = $"Only {remainingTickets} tickets are available for this event."});
            
            var ticket = new Ticket
            {
                EventId = dto.EventId,
                TicketType = dto.TicketType,
                Price = dto.Price,
                QuantityAvailable = dto.QuantityAvailable,
                QuantitySold = 0,
                CreatedBy = "admin",
                CreatedAt = DateTime.UtcNow,
            };

            await db.Tickets.AddAsync(ticket);
            await db.SaveChangesAsync();

            var response = new TicketDTO(
                ticket.Id,
                ticket.EventId,
                ticket.TicketType,
                ticket.Price,
                ticket.QuantityAvailable,
                ticket.QuantitySold);

            return Results.Created($"/tickets/{ticket.Id}", response);

        }).WithName("CreateTicket").WithTags("Tickets").WithSummary("Create a new ticket");


        app.MapPut("/tickets/{id:guid}", async (Guid id, TicketDTO dto, AppDbContext db) =>
        {
            var ticket = await db.Tickets.FindAsync(id);

            if (ticket is null)
                return Results.NotFound();

            ticket.EventId = dto.EventId;
            ticket.TicketType = dto.TicketType;
            ticket.Price = dto.Price;
            ticket.QuantityAvailable = dto.QuantityAvailable;
            ticket.QuantitySold = dto.QuantitySold;

            ticket.UpdatedBy = "admin";
            ticket.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();

            var response = new TicketDTO(
                ticket.Id,
                ticket.EventId,
                ticket.TicketType,
                ticket.Price,
                ticket.QuantityAvailable,
                ticket.QuantitySold);

            return Results.Ok(response);

        }).WithName("UpdateTicket").WithTags("Tickets").WithSummary("Update a ticket by ID");


        app.MapDelete("/tickets/{id:guid}", async (Guid id, AppDbContext db) =>
        {
            var ticket = await db.Tickets.FindAsync(id);

            if (ticket is null)
                return Results.NotFound();

            db.Tickets.Remove(ticket);
            await db.SaveChangesAsync();

            return Results.NoContent();

        }).WithName("DeleteTicket").WithTags("Tickets").WithSummary("Delete a ticket by ID");
    }
}
