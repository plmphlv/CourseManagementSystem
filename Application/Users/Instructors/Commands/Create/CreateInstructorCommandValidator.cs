namespace Application.Users.Instructors.Commands.Create
{
    public class CreateInstructorCommandValidator:AbstractValidator<CreateInstructorCommand>
    {
        public CreateInstructorCommandValidator() 
        {
            RuleFor(c => c.UserId)
            .NotEmpty()
            .WithMessage("UserId is required");
        }
    }
}
