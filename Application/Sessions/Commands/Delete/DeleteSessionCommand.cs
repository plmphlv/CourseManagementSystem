namespace Application.Sessions.Commands.Delete
{
    public class DeleteSessionCommand : IRequest
    {
        public int Id { get; set; }
    }

    public class DeleteSessionCommandHandler : IRequestHandler<DeleteSessionCommand>
    {
        private readonly IApplicationDbContext context;

        public DeleteSessionCommandHandler(IApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task Handle(DeleteSessionCommand request, CancellationToken cancellationToken)
        {
            int id = request.Id;

            Session? session = await context.Sessions
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

            if (session is null)
            {
                throw new NotFoundException(nameof(Session), id);
            }

            context.Sessions.Remove(session);
            await context.SaveChangesAsync(cancellationToken);

            List<Schedule> schedules = await context.Schedules
                .Where(s => s.SessionId == id)
                .ToListAsync(cancellationToken);

            context.Schedules.RemoveRange(schedules);
            await context.SaveChangesAsync(cancellationToken);

        }
    }
}
