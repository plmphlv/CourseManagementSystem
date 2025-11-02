namespace Application.Schedules.Command.Cancel
{
    public class CancelScheduleCommand : IRequest
    {
        public int Id { get; set; }
    }

    public class CancelScheduleCommandHandler : IRequestHandler<CancelScheduleCommand>
    {
        private readonly IUnitOfWork unitOfWork;

        public CancelScheduleCommandHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task Handle(CancelScheduleCommand request, CancellationToken cancellationToken)
        {
            int id = request.Id;

            Schedule? schedule = await unitOfWork.Schedules
                .GetByIdAsync(id, cancellationToken);

            if (schedule is null)
            {
                throw new NotFoundException(nameof(Schedule), id);
            }

            schedule.IsActive = false;

            await unitOfWork.Schedules.UpdateAsync(schedule, cancellationToken);
        }
    }
}
