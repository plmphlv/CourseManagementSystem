namespace Application.Sessions.Commands.Delete
{
    public class DeleteSessionCommand : IRequest
    {
        public int Id { get; set; }
    }

    public class DeleteSessionCommandHandler : IRequestHandler<DeleteSessionCommand>
    {
        private readonly IUnitOfWork unitOfWork;

        public DeleteSessionCommandHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteSessionCommand request, CancellationToken cancellationToken)
        {
            int id = request.Id;

            Session? session = await unitOfWork.Sessions
                .GetByIdAsync(id, cancellationToken);

            if (session is null)
            {
                throw new NotFoundException(nameof(Session), id);
            }

            List<Schedule> schedules = await unitOfWork.Schedules
                .Query()
                .Where(s => s.SessionId == id)
                .ToListAsync(cancellationToken);

            unitOfWork.Schedules.DeleteRange(schedules);
            await unitOfWork.Sessions.DeleteAsync(session, cancellationToken);

        }
    }
}
