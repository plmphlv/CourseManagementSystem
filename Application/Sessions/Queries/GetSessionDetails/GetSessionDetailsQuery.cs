
namespace Application.Sessions.Queries.GetSessionDetails
{
    public class GetSessionDetailsQuery : IRequest<SessionOutputModel>
    {
        public int Id { get; set; }
    }

   public class GetSessionDetailsQueryHandler : IRequestHandler<GetSessionDetailsQuery, SessionOutputModel>
    {
        private readonly IUnitOfWork unitOfWork;

        public GetSessionDetailsQueryHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<SessionOutputModel> Handle(GetSessionDetailsQuery request, CancellationToken cancellationToken)
        {
            int id = request.Id;

            SessionOutputModel? session = await unitOfWork.Sessions.Query()
                
                .Where(s => s.Id == id)
                .Select(s => new SessionOutputModel
                {
                    Id = s.Id,
                    ScheduledTime = s.ScheduledTime,
                    DurationMinutes = s.DurationMinutes,
                    IsConfirmed = s.IsConfirmed,
                    Notes = s.Notes,
                    CourseId = s.CourseId,
                    CourseName = s.Course.Name,
                    InstructorId = s.InstructorId,
                    InstructorName = $"{s.Instructor.User.FirstName} {s.Instructor.User.LastName}",
                    SubstituteInstructorId = s.SubstituteInstructorId,
                    SubstituteInstructorName = s.SubstituteInstructor != null ? $"{s.Instructor.User.FirstName} {s.Instructor.User.LastName}" : null
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (session == null)
            {
                throw new NotFoundException(nameof(Session), id);
            }

            return session;
        }
    }
}
