namespace Application.Sessions.Commands.Cancel
{
    public class CancelSessionsCommand : IRequest
    {
        public int Id { get; set; }
    }

    public class CancelSessionsCommandHandler : IRequestHandler<CancelSessionsCommand>
    {
        private readonly IApplicationDbContext context;
        public CancelSessionsCommandHandler(IApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task Handle(CancelSessionsCommand request, CancellationToken cancellationToken)
        {
            int id = request.Id;

            Session? session = await context.Sessions
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

            if (session is null)
            {
                throw new NotFoundException(nameof(Session), id);
            }

            session.IsConfirmed = false;

            await context.SaveChangesAsync(cancellationToken);

            List<Schedule> schedules = await context.Schedules
               .Where(s => s.SessionId == id)
               .ToListAsync(cancellationToken);

            foreach (Schedule schedule in schedules)
            {
                schedule.IsActive = false;
            }

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
