
namespace Application.Courses.Queries.GetCourses
{
    public class GetCoursesQuery : IRequest<List<CourseDto>>
    {
        public string? SearchKeyword { get; set; }

        public int? InstructorId { get; set; }

        public bool? AtCapacity { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }

    public class GetCoursesQueryHandler : IRequestHandler<GetCoursesQuery, List<CourseDto>>
    {
        private readonly IUnitOfWork unitOfWork;

        public GetCoursesQueryHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<List<CourseDto>> Handle(GetCoursesQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Course> query = unitOfWork.Courses.Query();

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
