namespace Application.Schedules.Command.Confirm
{
    public class ConfirmScheduleCommand : IRequest
    {
        public int Id { get; set; }
    }

    public class ConfirmScheduleCommandHandler : IRequestHandler<ConfirmScheduleCommand>
    {
        private readonly IUnitOfWork unitOfWork;

        public ConfirmScheduleCommandHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task Handle(ConfirmScheduleCommand request, CancellationToken cancellationToken)
        {
            int id = request.Id;

            Schedule? schedule = await unitOfWork.Schedules
                .GetByIdAsync(id, cancellationToken);

            if (schedule is null)
            {
                throw new NotFoundException(nameof(Schedule), id);
            }

            schedule.IsActive = true;

            await unitOfWork.Schedules.UpdateAsync(schedule, cancellationToken);
        }
    }
}
