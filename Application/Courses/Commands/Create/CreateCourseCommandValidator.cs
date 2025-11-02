namespace Application.Courses.Commands.Create
{
    public class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
    {
        public CreateCourseCommandValidator()
        {
            RuleFor(c => c.Name)
                .NotEmpty()
                .WithMessage("Course name is requires");

            RuleFor(c => c.InstructorId)
                .GreaterThan(0)
                .WithMessage("Valid instructor id is required");

            RuleFor(c => c.InstructorId)
                .GreaterThan(0)
                .When(c => c.MemberLimit.HasValue)
                .WithMessage("Invalid Member Limit");

            RuleFor(c => c.StartDate)
                .LessThan(c => c.EndDate)
                .WithMessage("The course should start earlier than it ends");

            RuleForEach(c => c.Sessions)
               .Must((course, session) => course.StartDate <= session.ScheduledDate)
               .WithMessage("Session cannot be scheduled before the course's starting date");

            RuleForEach(c => c.Sessions)
                .Must((course, session) => course.EndDate >= session.ScheduledDate)
                .WithMessage("Session cannot be scheduled after the course's ending date");

            RuleForEach(c => c.Sessions)
                .Must(session => session.DurationMinutes > 0)
                .WithMessage("Session cannot be scheduled after the course's ending date");
        }
    }
}
