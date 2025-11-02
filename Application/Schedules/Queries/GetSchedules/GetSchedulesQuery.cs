namespace Application.Schedules.Queries.GetSchedules
{
    public class GetSchedulesQuery : IRequest<List<ScheduleDto>>
    {
        public int? CourseId { get; set; }

        public int? SessionId { get; set; }

        public bool? IsConfirmed { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }

    public class GetSchedulesQueryHandler : IRequestHandler<GetSchedulesQuery, List<ScheduleDto>>
    {
        private readonly IApplicationDbContext _context;
        public GetSchedulesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<ScheduleDto>> Handle(GetSchedulesQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Schedule> query = _context.Schedules.AsQueryable();

            int? courseId = request.CourseId;

            if (courseId.HasValue)
            {
                query = query.Where(s => s.Session.CourseId == courseId);
            }

            bool? isConfirmed = request.IsConfirmed;

            if (isConfirmed.HasValue)
            {
                query = query.Where(s => s.Session.IsConfirmed == isConfirmed);
            }

            int? sessionId = request.SessionId;

            if (sessionId.HasValue)
            {
                query = query.Where(s => s.SessionId == sessionId);
            }

            DateTime? startDate = request.StartDate;

            if (startDate.HasValue)
            {
                query = query.Where(s => s.ScheduleDate >= startDate);
            }

            DateTime? endDate = request.EndDate;

            if (endDate.HasValue)
            {
                query = query.Where(s => s.ScheduleDate <= endDate);
            }

            List<ScheduleDto> schedules = await query.Select(s =>
                new ScheduleDto
                {
                    Id = s.Id,
                    IsConfirmed = s.IsActive,
                    ScheduleDate = s.ScheduleDate,
                    SessionId = s.SessionId
                }
            ).ToListAsync(cancellationToken);

            return schedules;
        }
    }
}
