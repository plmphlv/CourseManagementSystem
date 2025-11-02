namespace Application.Sessions.Commands.Confirm
{
    public class ConfirmSessionsCommandValidator : AbstractValidator<ConfirmSessionsCommand>
    {
        public ConfirmSessionsCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Session IDs must be a positive number.");
        }
    }
}
