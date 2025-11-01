using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class CourseMemberConfiguration : IEntityTypeConfiguration<CourseMember>
    {
        public void Configure(EntityTypeBuilder<CourseMember> builder)
        {
            builder.HasKey(cm => new { cm.CourseId, cm.MemberId });

            builder.HasOne(cm => cm.Course)
                .WithMany(c => c.Members)
                .HasForeignKey(cm => cm.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cm => cm.Member)
                .WithMany(u => u.Courses)
                .HasForeignKey(cm => cm.MemberId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
