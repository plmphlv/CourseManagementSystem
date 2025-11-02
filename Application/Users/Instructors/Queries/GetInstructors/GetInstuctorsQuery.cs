using Application.Users.Instructors.Queries.GetInstructorDetails;

namespace Application.Users.Instructors.Queries.GetInstructors
{
    public class GetInstuctorsQuery : IRequest<List<InstructorOutputModel>>;

    public class GetInstuctorsQueryHandler : IRequestHandler<GetInstuctorsQuery, List<InstructorOutputModel>>
    {
        private readonly IUnitOfWork unitOfWork;
        public GetInstuctorsQueryHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<List<InstructorOutputModel>> Handle(GetInstuctorsQuery request, CancellationToken cancellationToken)
        {
            List<InstructorOutputModel> instructors = await unitOfWork.Instructors
                .Query()
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