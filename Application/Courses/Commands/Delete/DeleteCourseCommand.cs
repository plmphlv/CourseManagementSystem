namespace Application.Courses.Commands.Delete
{
    public class DeleteCourseCommand : IRequest
    {
        public int Id { get; set; }
    }

    public class DeleteCourseCommandHandler : IRequestHandler<DeleteCourseCommand>
    {
        private readonly IUnitOfWork unitOfWork;

        public DeleteCourseCommandHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
        {
            int id = request.Id;

            Course? course = await unitOfWork.Courses.GetByIdAsync(id, cancellationToken);

            if (course is null)
            {
                throw new NotFoundException(nameof(Course), id);
            }

            List<Session> sessions = await unitOfWork.Sessions
                .Query()
                .Where(s => s.CourseId == id)
                .ToListAsync(cancellationToken);

            HashSet<int> sessionsIds = sessions.Select(s => s.Id)
                .ToHashSet();

            List<Schedule> schedules = await unitOfWork.Schedules
                .Query()
                .Where(s => sessionsIds.Contains(s.SessionId))
                .ToListAsync(cancellationToken);

            unitOfWork.Sessions.DeleteRange(sessions);
            unitOfWork.Schedules.DeleteRange(schedules);

            await unitOfWork.SaveChangesAsync(cancellationToken);
            await unitOfWork.Courses.DeleteAsync(course, cancellationToken);
        }
    }
}
