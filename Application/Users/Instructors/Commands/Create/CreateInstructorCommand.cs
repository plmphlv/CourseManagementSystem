using Application.Users.Commands.Register;
using FluentValidation.Results;

namespace Application.Users.Instructors.Commands.Create
{
    public class CreateInstructorCommand : IRequest<int>
    {
        public string UserId { get; set; } = null!;
    }

    public class CreateInstructorCommandHandler : IRequestHandler<CreateInstructorCommand, int>
    {
        private readonly IIdentityService identityService;

        private readonly IApplicationDbContext context;

        public CreateInstructorCommandHandler(IIdentityService identityService, IApplicationDbContext context)
        {
            this.identityService = identityService;
            this.context = context;
        }
        public async Task<int> Handle(CreateInstructorCommand request, CancellationToken cancellationToken)
        {
            string userId = request.UserId;

            User? user = await identityService.FindUserByIdAsync(userId);

            if (user is null)
            {
                throw new NotFoundException(nameof(User), userId);
            }

            Instructor instructor = new Instructor
            {
                UserId = userId
            };

            context.Instructors.Add(instructor);
            await context.SaveChangesAsync(cancellationToken);

            string role = Role.Instructor.ToString();
            Result result = await identityService.AddRoleAsync(userId, role, cancellationToken);

            if (!result.Succeeded)
            {
                throw new ValidationException(result.Errors.Select(e => new ValidationFailure(nameof(RegisterCommand), e)));
            }

            return instructor.Id;
        }
    }
}
