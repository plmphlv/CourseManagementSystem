namespace Application.Schedules.Command.Confirm
{
    public class ConfirmScheduleCommand : IRequest
    {
        public int Id { get; set; }
    }

    public class ConfirmScheduleCommandHandler : IRequestHandler<ConfirmScheduleCommand>
    {
        private readonly IApplicationDbContext context;

        public ConfirmScheduleCommandHandler(IApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task Handle(ConfirmScheduleCommand request, CancellationToken cancellationToken)
        {
            int id = request.Id;

            Schedule? schedule = await context.Schedules
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

            if (schedule is null)
            {
                throw new NotFoundException(nameof(Schedule), id);
            }

            schedule.IsActive = true;

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
