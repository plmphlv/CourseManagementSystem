
namespace Application.Courses.Queries.GetCourses
{
    public class GetCoursesQuery : IRequest<List<CourseDto>>
    {
        public string? SearchKeyword { get; set; }

        public int? InstructorId { get; set; }

        public bool? AtCapacity { get; set; }

        public bool? MyCourses { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }

    public class GetCoursesQueryHandler : IRequestHandler<GetCoursesQuery, List<CourseDto>>
    {
        private readonly IApplicationDbContext context;
        private readonly ICurrentUserService currentUserService;

        public GetCoursesQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            this.context = context;
            this.currentUserService = currentUserService;
        }

        public async Task<List<CourseDto>> Handle(GetCoursesQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Course> query = context.Courses;

            string? searchKeyword = request.SearchKeyword;

            if (!string.IsNullOrWhiteSpace(searchKeyword))
            {
                query = query.Where(c => c.Name.Contains(searchKeyword) ||
                c.Description != null &&
                c.Description.Contains(searchKeyword));
            }

            int? instructorId = request.InstructorId;

            if (instructorId.HasValue)
            {
                query = query.Where(c => c.InstructorId == instructorId);
            }

            bool? atCapacity = request.AtCapacity;

            if (atCapacity.HasValue)
            {
                if (atCapacity.Value)
                {
                    query = query.Where(c => c.Members.Count == c.MemberLimit);
                }
                else
                {
                    query = query.Where(c => c.Members.Count < c.MemberLimit);
                }
            }

            bool? myCourses = request.MyCourses;

            if (myCourses.HasValue)
            {
                string userId = currentUserService.UserId!;

                if (myCourses.Value)
                {
                    query = query.Where(c => c.Members.Any(cm => cm.MemberId == userId));
                }
                else
                {
                    query = query.Where(c => !c.Members.Any(cm => cm.MemberId == userId));
                }
            }

            DateTime? startDate = request.StartDate;

            if (startDate.HasValue)
            {
                query = query.Where(c => c.StartDate >= startDate);
            }

            DateTime? endDate = request.EndDate;

            if (endDate.HasValue)
            {
                query = query.Where(c => c.EndDate <= endDate);
            }

            List<CourseDto> courseDtos = await query
                .Select(c => new CourseDto
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync(cancellationToken);

            return courseDtos;
        }
    }
}
