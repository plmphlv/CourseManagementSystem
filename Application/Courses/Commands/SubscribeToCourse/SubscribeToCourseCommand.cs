namespace Application.Courses.Commands.SubscribeToCourse
{
    public class SubscribeToCourseCommand : IRequest
    {
        public int CourseId { get; set; }
    }

    public class SubscribeToCourseCommandHandler : IRequestHandler<SubscribeToCourseCommand>
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly ICurrentUserService currentUserService;

        public SubscribeToCourseCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            this.unitOfWork = unitOfWork;
            this.currentUserService = currentUserService;
        }

        public async Task Handle(SubscribeToCourseCommand request, CancellationToken cancellationToken)
        {
            int courseId = request.CourseId;

            bool courseExists = await unitOfWork.Courses
                .EntityExists(c => c.Id == courseId, cancellationToken);

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

            await unitOfWork.CourseMembers.AddAsync(subscription,cancellationToken);

            List<Session> sessions = await unitOfWork.Sessions
                .Query(s => s.CourseId == courseId).ToListAsync(cancellationToken);

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

            await unitOfWork.Schedules.AddRangeAsync(schedules, cancellationToken);
        }
    }
}
