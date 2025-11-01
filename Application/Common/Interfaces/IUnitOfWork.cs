using Domain.Entities;

namespace Application.Common.Interfaces
{
    public interface IUnitOfWork
    {
        IRepository<Course> Courses { get; }

        IRepository<CourseMember> CourseMembers { get; }

        IRepository<Instructor> Instructors { get; }

        IRepository<Schedule> Schedules { get; }

        IRepository<Session> Sessions { get; }

        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
