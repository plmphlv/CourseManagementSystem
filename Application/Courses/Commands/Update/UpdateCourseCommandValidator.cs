using System.Transactions;

namespace Application.Courses.Commands.Update
{
    public class UpdateCourseCommandValidator : AbstractValidator<UpdateCourseCommand>
    {
        public UpdateCourseCommandValidator()
        {
            RuleFor(c => c.Id)
                .GreaterThan(0)
                .WithMessage("Invalid course id");

            RuleFor(c => c.Name)
                .NotEmpty()
                .WithMessage("Course name is requires");


            RuleFor(c => c.MemberLimit)
                .GreaterThan(0)
                .When(c => c.MemberLimit.HasValue)
                .WithMessage("Invalid Member Limit");

            RuleFor(c => c.StartDate)
                .LessThan(c => c.EndDate)
                .WithMessage("The course should start earlier than it ends");
        }
    }
}
