namespace Application.Courses.Commands.UnsubscribeFromCourse
{
    public class UnsubscribeFromCourseCommand : IRequest
    {
        public int CourseId { get; set; }
    }

    public class UnsubscribeFromCourseCommandHandler : IRequestHandler<UnsubscribeFromCourseCommand>
    {
        private readonly IApplicationDbContext context;
        private readonly ICurrentUserService currentUserService;
        public UnsubscribeFromCourseCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            this.context = context;
            this.currentUserService = currentUserService;
        }
        public async Task Handle(UnsubscribeFromCourseCommand request, CancellationToken cancellationToken)
        {
            int courseId = request.CourseId;
            string userId = currentUserService.UserId!;

            CourseMember? subscription = await context.CourseMembers
                .Where(cm => cm.CourseId == courseId && cm.MemberId == userId)
                .FirstOrDefaultAsync(cancellationToken);

            if (subscription is null)
            {
                throw new NotFoundException(nameof(CourseMember), $"CourseId: {courseId}, MemberId: {userId}");
            }

            context.CourseMembers.Remove(subscription);

            List<Schedule> schedules = await context.Schedules
                .Where(s => s.AccountId == userId && s.Session.CourseId == courseId)
                .ToListAsync(cancellationToken);

            context.Schedules.RemoveRange(schedules);

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
