namespace Application.Sessions.Commands.Confirm
{
    public class ConfirmSessionsCommand : IRequest
    {
        public int Id { get; set; }
    }

    public class ConfirmSessionsCommandHandler : IRequestHandler<ConfirmSessionsCommand>
    {
        private readonly IUnitOfWork unitOfWork;

        public ConfirmSessionsCommandHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task Handle(ConfirmSessionsCommand request, CancellationToken cancellationToken)
        {
            int id = request.Id;

            Session? session = await unitOfWork.Sessions
                .GetByIdAsync(id, cancellationToken);

            if (session is null)
            {
                throw new NotFoundException(nameof(Session), id);
            }

            session.IsConfirmed = true;

            await unitOfWork.Sessions.UpdateAsync(session, cancellationToken);

            List<Schedule> schedules = await unitOfWork.Schedules
               .Query()
               .Where(s => s.SessionId == id)
               .ToListAsync(cancellationToken);

            foreach (Schedule schedule in schedules)
            {
                schedule.IsActive = true;
            }

            await unitOfWork.Schedules.UpdateAsync(schedules, cancellationToken);
        }
    }
}
