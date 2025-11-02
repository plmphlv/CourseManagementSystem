namespace Application.Sessions.Commands.Confirm
{
    public class ConfirmSessionsCommand : IRequest
    {
        public int Id { get; set; }
    }

    public class ConfirmSessionsCommandHandler : IRequestHandler<ConfirmSessionsCommand>
    {
        private readonly IApplicationDbContext context;

        public ConfirmSessionsCommandHandler(IApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task Handle(ConfirmSessionsCommand request, CancellationToken cancellationToken)
        {
            int id = request.Id;

            Session? session = await context.Sessions
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

            if (session is null)
            {
                throw new NotFoundException(nameof(Session), id);
            }

            session.IsConfirmed = true;

            await context.SaveChangesAsync(cancellationToken);

            List<Schedule> schedules = await context.Schedules
               .Where(s => s.SessionId == id)
               .ToListAsync(cancellationToken);

            foreach (Schedule schedule in schedules)
            {
                schedule.IsActive = true;
            }

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
