namespace Application.Sessions.Commands.Cancel
{
    public class CancelSessionsCommand : IRequest
    {
        public int Id { get; set; }
    }

    public class CancelSessionsCommandHandler : IRequestHandler<CancelSessionsCommand>
    {
        private readonly IUnitOfWork unitOfWork;
        public CancelSessionsCommandHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task Handle(CancelSessionsCommand request, CancellationToken cancellationToken)
        {
            int id = request.Id;

            Session? session = await unitOfWork.Sessions
                .GetByIdAsync(id, cancellationToken);

            if (session is null)
            {
                throw new NotFoundException(nameof(Session), id);
            }

            session.IsConfirmed = false;
            await unitOfWork.Sessions.UpdateAsync(session, cancellationToken);

            List<Schedule> schedules = await unitOfWork.Schedules
               .Query()
               .Where(s => s.SessionId == id)
               .ToListAsync(cancellationToken);

            foreach (Schedule schedule in schedules)
            {
                schedule.IsActive = false;
            }

            await unitOfWork.Schedules.UpdateAsync(schedules, cancellationToken);
        }
    }
}
