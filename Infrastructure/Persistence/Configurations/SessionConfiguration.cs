using Domain.Entities;
using Infrastructure.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class SessionConfiguration : AuditableEntityConfiguration<Session>
    {
        public override void Configure(EntityTypeBuilder<Session> builder)
        {
            base.Configure(builder);

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id)
                .UseIdentityColumn(1, 1);

            builder.HasOne(s => s.Course)
                .WithMany(c => c.Sessions)
                .HasForeignKey(s => s.CourseId);

            builder.HasOne(s => s.Instructor)
                .WithMany(i => i.Sessions)
                .HasForeignKey(s => s.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.SubstituteInstructor)
                .WithMany(i => i.SubstituteSessions)
                .HasForeignKey(s => s.SubstituteInstructorId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
