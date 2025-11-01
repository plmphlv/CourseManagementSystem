using Domain.Entities;
using Infrastructure.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class ScheduleConfiguration : AuditableEntityConfiguration<Schedule>
    {
        public override void Configure(EntityTypeBuilder<Schedule> builder)
        {
            base.Configure(builder);

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id)
                .UseIdentityColumn(1, 1);

            builder.HasOne(s => s.Session)
                .WithMany(e => e.Schedules)
                    .HasForeignKey(s => s.SessionId)
                    .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(s => s.Account)
                    .WithMany(a => a.Schedules)
                    .HasForeignKey(s => s.AccountId)
                    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
