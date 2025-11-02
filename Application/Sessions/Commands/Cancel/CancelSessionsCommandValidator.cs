namespace Application.Sessions.Commands.Cancel
{
    public class CancelSessionsCommandValidator : AbstractValidator<CancelSessionsCommand>
    {
        public CancelSessionsCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Session IDs must be a positive number.");
        }
    }
}
