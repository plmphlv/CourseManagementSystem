namespace Application.Sessions.Commands.Create
{
    public class CreateSessionCommandValidator : AbstractValidator<CreateSessionCommand>
    {
        public CreateSessionCommandValidator(IDateTime dateTime)
        {
            RuleFor(c => c.ScheduledTime)
                .NotNull()
                .WithMessage("Sessions must have a scheduled time")
                .GreaterThan(dateTime.Now)
                .WithMessage("Scheduled time must be in the future");

            RuleFor(c => c.DurationMinutes)
                .GreaterThanOrEqualTo(5)
                .WithMessage("Sessions cannot be shorter than 5 minutes");

            RuleFor(c => c.CourseId)
                .GreaterThan(0)
                .WithMessage("Sessions must be part of a Course");

            RuleFor(c => c.InstructorId)
                .GreaterThan(0)
                .WithMessage("Sessions must have an instuctor");

            RuleFor(c => c.SubstituteInstructorId)
                .GreaterThan(0)
                .When(c => c.SubstituteInstructorId.HasValue)
                .WithMessage("Sessions must have a valid substitute instuctor");
        }
    }
}
