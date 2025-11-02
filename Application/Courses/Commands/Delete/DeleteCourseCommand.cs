namespace Application.Courses.Commands.Delete
{
    public class DeleteCourseCommand : IRequest
    {
        public int Id { get; set; }
    }

    public class DeleteCourseCommandHandler : IRequestHandler<DeleteCourseCommand>
    {
        private readonly IApplicationDbContext context;

        public DeleteCourseCommandHandler(IApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
        {
            int id = request.Id;

            Course? course = await context.Courses.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

            if (course is null)
            {
                throw new NotFoundException(nameof(Course), id);
            }

            List<Session> sessions = await context.Sessions
                .Where(s => s.CourseId == id)
                .ToListAsync(cancellationToken);

            HashSet<int> sessionsIds = sessions.Select(s => s.Id)
                .ToHashSet();

            List<Schedule> schedules = await context.Schedules
                .Where(s => sessionsIds.Contains(s.SessionId))
                .ToListAsync(cancellationToken);

            context.Sessions.RemoveRange(sessions);
            context.Schedules.RemoveRange(schedules);

            context.Courses.Remove(course);

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
