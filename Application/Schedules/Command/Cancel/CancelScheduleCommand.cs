namespace Application.Schedules.Command.Cancel
{
    public class CancelScheduleCommand : IRequest
    {
        public int Id { get; set; }
    }

    public class CancelScheduleCommandHandler : IRequestHandler<CancelScheduleCommand>
    {
        private readonly IApplicationDbContext context;

        public CancelScheduleCommandHandler(IApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task Handle(CancelScheduleCommand request, CancellationToken cancellationToken)
        {
            int id = request.Id;

            Schedule? schedule = await context.Schedules
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

            if (schedule is null)
            {
                throw new NotFoundException(nameof(Schedule), id);
            }

            schedule.IsActive = false;

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
