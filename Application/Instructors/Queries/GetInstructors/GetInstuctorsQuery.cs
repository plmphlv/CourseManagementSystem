using Application.Instructors.Queries.GetInstructorDetails;

namespace Application.Instructors.Queries.GetInstructors
{
    public class GetInstuctorsQuery : IRequest<List<InstructorOutputModel>>;

    public class GetInstuctorsQueryHandler : IRequestHandler<GetInstuctorsQuery, List<InstructorOutputModel>>
    {
        private readonly IApplicationDbContext context;
        public GetInstuctorsQueryHandler(IApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<List<InstructorOutputModel>> Handle(GetInstuctorsQuery request, CancellationToken cancellationToken)
        {
            List<InstructorOutputModel> instructors = await context.Instructors
                .Select(i => new InstructorOutputModel
                {
                    Id = i.Id,
                    FullName = i.User.FirstName + " " + i.User.LastName
                })
                .ToListAsync(cancellationToken);

            return instructors;
        }
    }
}