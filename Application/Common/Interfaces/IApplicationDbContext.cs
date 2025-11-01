using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

        DatabaseFacade Database { get; }

        DbSet<Course> Courses { get; }

        DbSet<CourseMember> CourseMembers { get; }

        DbSet<Instructor> Instructors { get; }

        DbSet<Schedule> Schedules { get; }

        DbSet<Session> Sessions { get; }
    }
}
