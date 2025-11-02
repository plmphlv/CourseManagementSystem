using Application.Sessions.Common;
using FluentValidation.Results;

namespace Application.Sessions.Commands.Create
{
    public class CreateSessionCommand : SessionModel, IRequest<int>;

    public class CreateSessionCommandHandler : IRequestHandler<CreateSessionCommand, int>
    {
        private readonly IApplicationDbContext context;

        public CreateSessionCommandHandler(IApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<int> Handle(CreateSessionCommand request, CancellationToken cancellationToken)
        {
            List<string> missingEntityMessages = new List<string>();

            int courseId = request.CourseId;

            Course? course = await context.Courses
               .FirstOrDefaultAsync(c => c.Id == courseId, cancellationToken);

            if (course is null)
            {
                missingEntityMessages.Add($"{nameof(Course)} with Id:{courseId} was not found");
            }

            int instructorId = request.InstructorId;

            bool instructorExists = await context.Instructors
               .AnyAsync(i => i.Id == instructorId, cancellationToken);

            if (!instructorExists)
            {
                missingEntityMessages.Add($"{nameof(Instructor)} with Id:{courseId} was not found");
            }

            int? substituteInstructorId = request.SubstituteInstructorId;

            if (substituteInstructorId.HasValue)
            {
                bool substituteInstructorExists = await context.Instructors
                   .AnyAsync(i => i.Id == substituteInstructorId, cancellationToken);

                if (!instructorExists)
                {
                    missingEntityMessages.Add($"{nameof(Instructor)} with Id:{courseId} was not found");
                }
            }

            if (missingEntityMessages.Count > 0)
            {
                throw new NotFoundException(missingEntityMessages);
            }

            List<ValidationFailure> validationFailures = new List<ValidationFailure>(2);

            DateTime scheduledTime = request.ScheduledTime;

            if (scheduledTime > course.EndDate)
            {
                validationFailures.Add(new ValidationFailure(nameof(Session.ScheduledTime), $"Sessions cannot be scheduled after course end date"));
            }

            bool sessionExists = await context.Sessions
                .AnyAsync(s => s.CourseId == courseId &&
                s.ScheduledTime == scheduledTime, cancellationToken);

            if (sessionExists)
            {
                validationFailures.Add(new ValidationFailure(nameof(Session.ScheduledTime), $"Session with Scheduled Time:{scheduledTime.ToString()} already exists"));
            }

            if (validationFailures.Count > 0)
            {
                throw new ValidationException(validationFailures);
            }

            Session session = new Session
            {
                ScheduledTime = scheduledTime,
                DurationMinutes = request.DurationMinutes,
                IsConfirmed = request.IsConfirmed,
                Notes = request.Notes,
                CourseId = courseId,
                InstructorId = instructorId,
                SubstituteInstructorId = substituteInstructorId,
            };

            List<string> users = await context.CourseMembers
                .Distinct()
                .Where(cm => cm.CourseId == courseId)
                .Select(cm => cm.MemberId)
                .ToListAsync(cancellationToken);

            List<Schedule> schedules = new List<Schedule>();

            foreach (string userId in users)
            {
                Schedule schedule = new Schedule
                {
                    Session = session,
                    AccountId = userId,
                    IsActive = session.IsConfirmed,
                    ScheduleDate = session.ScheduledTime,
                };

                schedules.Add(schedule);
            }

            context.Schedules.AddRange(schedules);
            await context.SaveChangesAsync(cancellationToken);

            return session.Id;
        }
    }
}
