using Application.Common.Interfaces;
using Domain.Entities;

namespace Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext context;

        public UnitOfWork(ApplicationDbContext context)
        {
            this.context = context;

            Courses = new Repository<Course>(context);
            CourseMembers = new Repository<CourseMember>(context);
            Instructors = new Repository<Instructor>(context);
            Schedules = new Repository<Schedule>(context);
            Sessions = new Repository<Session>(context);
        }

        public IRepository<Course> Courses { get; }

        public IRepository<CourseMember> CourseMembers { get; }

        public IRepository<Instructor> Instructors { get; }

        public IRepository<Schedule> Schedules { get; }

        public IRepository<Session> Sessions { get; }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
