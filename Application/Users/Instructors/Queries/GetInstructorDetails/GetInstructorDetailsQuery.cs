namespace Application.Users.Instructors.Queries.GetInstructorDetails
{
    public class GetInstructorDetailsQuery : IRequest<InstructorOutputModel>
    {
        public int Id { get; set; }
    }

    public class GetInstructorDetailsQueryHandler : IRequestHandler<GetInstructorDetailsQuery, InstructorOutputModel>
    {
        private readonly IApplicationDbContext context;

        public GetInstructorDetailsQueryHandler(IApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<InstructorOutputModel> Handle(GetInstructorDetailsQuery request, CancellationToken cancellationToken)
        {
            int id = request.Id;

            InstructorOutputModel? instructor = await context.Instructors
                .Where(i => i.Id == id)
                .Select(i => new InstructorOutputModel
                {
                    Id = i.Id,
                    FullName = i.User.FirstName + " " + i.User.LastName
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (instructor is null)
            {
                throw new NotFoundException(nameof(Instructor), id);
            }

            return instructor;
        }
    }
}
