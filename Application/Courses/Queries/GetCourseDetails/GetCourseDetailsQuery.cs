
namespace Application.Courses.Queries.GetCourseDetails
{
    public class GetCourseDetailsQuery : IRequest<CourseOutputModel>
    {
        public int Id { get; set; }
    }

    public class GetCourseDetailsQueryHandler : IRequestHandler<GetCourseDetailsQuery, CourseOutputModel>
    {
        private readonly IDateTime dateTime;
        private readonly IApplicationDbContext context;
        private readonly ICurrentUserService currentUserService;

        public GetCourseDetailsQueryHandler(IDateTime dateTime, IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            this.dateTime = dateTime;
            this.context = context;
            this.currentUserService = currentUserService;
        }

        public async Task<CourseOutputModel> Handle(GetCourseDetailsQuery request, CancellationToken cancellationToken)
        {
            int id = request.Id;

            CourseOutputModel? course = await context.Courses
                .Where(c => c.Id == id)
                .Select(c => new CourseOutputModel
                {
                    Id = id,
                    Name = c.Name,
                    Description = c.Description,
                    MemberLimit = c.MemberLimit,
                    InstructorName = c.Instructor.User.FirstName + " " + c.Instructor.User.LastName,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (course is null)
            {
                throw new NotFoundException(nameof(Course), id);
            }

            string userId = currentUserService.UserId!;

            bool isSublscribedToCourse = await context.CourseMembers
                .AnyAsync(cm => cm.MemberId == userId && cm.CourseId == id, cancellationToken);

            if (!isSublscribedToCourse)
            {
                course.CompletionPercentage = "You aren't subscribed to this course.";

                return course;
            }

            HashSet<int> sessionIds = await context.Sessions
                .Where(s => s.CourseId == id)
                .Select(s => s.Id)
                .ToHashSetAsync(cancellationToken);

            int sessionCount = sessionIds.Count();

            if (sessionCount == 0)
            {
                course.CompletionPercentage = "0.00%";

                return course;
            }

            DateTime now = dateTime.Now;

            int completedSessions = await context.Schedules
                .Where(s =>
                s.AccountId == userId &&
                s.ScheduleDate < now &&
                sessionIds.Contains(s.SessionId) &&
                s.IsActive
                ).Distinct()
                .Select(s => s.SessionId)
                .CountAsync(cancellationToken);


            decimal completionPercentage = (decimal)completedSessions / sessionCount;

            course.CompletionPercentage = $"{completionPercentage * 100:F2}%";

            return course;
        }
    }
}
