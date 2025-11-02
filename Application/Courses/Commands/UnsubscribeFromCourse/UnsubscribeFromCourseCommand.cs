namespace Application.Courses.Commands.UnsubscribeFromCourse
{
    public class UnsubscribeFromCourseCommand : IRequest
    {
        public int CourseId { get; set; }
    }

    public class UnsubscribeFromCourseCommandHandler : IRequestHandler<UnsubscribeFromCourseCommand>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ICurrentUserService currentUserService;
        public UnsubscribeFromCourseCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            this.unitOfWork = unitOfWork;
            this.currentUserService = currentUserService;
        }
        public async Task Handle(UnsubscribeFromCourseCommand request, CancellationToken cancellationToken)
        {
            int courseId = request.CourseId;
            string userId = currentUserService.UserId!;

            CourseMember? subscription = await unitOfWork.CourseMembers
                .Query()
                .Where(cm => cm.CourseId == courseId && cm.MemberId == userId)
                .FirstOrDefaultAsync(cancellationToken);

            if (subscription is null)
            {
                throw new NotFoundException(nameof(CourseMember), $"CourseId: {courseId}, MemberId: {userId}");
            }

            await unitOfWork.CourseMembers.DeleteAsync(subscription, cancellationToken);

            IEnumerable<Schedule> schedules = await unitOfWork.Schedules
                .GetAllAsync(s => s.AccountId == userId && s.Session.CourseId == courseId, cancellationToken);

            unitOfWork.Schedules.DeleteRange(schedules);

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
