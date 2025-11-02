using Application.Sessions.Common;
using FluentValidation.Results;

namespace Application.Sessions.Commands.Update
{
    public class UpdateSessionCommand : SessionModel, IRequest
    {
        public int Id { get; set; }
    }

    public class UpdateSessionCommandHandler : IRequestHandler<UpdateSessionCommand>
    {
        private readonly IUnitOfWork unitOfWork;

        public UpdateSessionCommandHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateSessionCommand request, CancellationToken cancellationToken)
        {
            int id = request.Id;

            List<string> missingEntityMessages = new List<string>();

            Session? session = await unitOfWork.Sessions
                .GetByIdAsync(id, cancellationToken);

            if (session is null)
            {
                throw new NotFoundException(nameof(Session), id);
            }

            int courseId = request.CourseId;

            Course? course = await unitOfWork.Courses
               .GetByIdAsync(courseId, cancellationToken);

            if (course is null)
            {
                missingEntityMessages.Add($"{nameof(Course)} with Id:{courseId} was not found");
            }

            int instructorId = request.InstructorId;

            bool instructorExists = await unitOfWork.Instructors
               .EntityExists(i => i.Id == instructorId, cancellationToken);

            if (!instructorExists)
            {
                missingEntityMessages.Add($"{nameof(Instructor)} with Id:{courseId} was not found");
            }

            int? substituteInstructorId = request.SubstituteInstructorId;

            if (substituteInstructorId.HasValue)
            {
                bool substituteInstructorExists = await unitOfWork.Instructors
                   .EntityExists(i => i.Id == substituteInstructorId, cancellationToken);

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

            bool sessionExists = await unitOfWork.Sessions
                .EntityExists(s => s.CourseId == courseId &&
                s.ScheduledTime == scheduledTime, cancellationToken);

            if (sessionExists)
            {
                validationFailures.Add(new ValidationFailure(nameof(Session.ScheduledTime), $"Session with Scheduled Time:{scheduledTime.ToString()} already exists"));
            }

            if (validationFailures.Count > 0)
            {
                throw new ValidationException(validationFailures);
            }

            session.ScheduledTime = scheduledTime;
            session.DurationMinutes = request.DurationMinutes;
            session.IsConfirmed = request.IsConfirmed;
            session.Notes = request.Notes;
            session.CourseId = courseId;
            session.InstructorId = instructorId;
            session.SubstituteInstructorId = substituteInstructorId;

            await unitOfWork.Sessions.UpdateAsync(session, cancellationToken);

            List<Schedule> schedules = await unitOfWork.Schedules
                .Query()
                .Where(s=>s.SessionId==id)
                .ToListAsync(cancellationToken);

            foreach (Schedule schedule in schedules)
            {
                schedule.IsActive = request.IsConfirmed;
                schedule.ScheduleDate = scheduledTime;
            }

            await unitOfWork.Schedules.UpdateAsync(schedules, cancellationToken);
        }
    }
}
