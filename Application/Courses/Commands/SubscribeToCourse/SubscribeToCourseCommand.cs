namespace Application.Courses.Commands.SubscribeToCourse
{
    public class SubscribeToCourseCommand : IRequest
    {
        public int CourseId { get; set; }
    }

    public class SubscribeToCourseCommandHandler : IRequestHandler<SubscribeToCourseCommand>
    {
        private readonly IApplicationDbContext context;

        private readonly ICurrentUserService currentUserService;

        public SubscribeToCourseCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            this.context = context;
            this.currentUserService = currentUserService;
        }

        public async Task Handle(SubscribeToCourseCommand request, CancellationToken cancellationToken)
        {
            int courseId = request.CourseId;

            bool courseExists = await context.Courses
                .AnyAsync(c => c.Id == courseId, cancellationToken);

            if (!courseExists)
            {
                throw new NotFoundException(nameof(Course), courseId);
            }

            string userId = currentUserService.UserId!;

            CourseMember subscription = new CourseMember
            {
                CourseId = request.CourseId,
                MemberId = userId
            };

            context.CourseMembers.Add(subscription);
            await context.SaveChangesAsync(cancellationToken);

            List<Session> sessions = await context.Sessions
                .Where(s => s.CourseId == courseId).ToListAsync(cancellationToken);

            List<Schedule> schedules = new List<Schedule>();

            foreach (Session session in sessions)
            {
                schedules.Add(new Schedule
                {
                    AccountId = userId,
                    SessionId = session.Id,
                    ScheduleDate = session.ScheduledTime,
                    IsActive = true
                });
            }

            context.Schedules.AddRange(schedules);

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
