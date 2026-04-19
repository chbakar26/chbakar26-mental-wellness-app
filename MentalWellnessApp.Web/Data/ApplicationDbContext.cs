using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MentalWellnessApp.Web.Models;

namespace MentalWellnessApp.Web.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<WellnessProgram> WellnessPrograms => Set<WellnessProgram>();
    public DbSet<WellnessSession> WellnessSessions => Set<WellnessSession>();
    public DbSet<WellnessResource> WellnessResources => Set<WellnessResource>();
    public DbSet<MoodCheckIn> MoodCheckIns => Set<MoodCheckIn>();
    public DbSet<SessionRequest> SessionRequests => Set<SessionRequest>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>()
            .HasOne(x => x.PreferredCounsellor)
            .WithMany()
            .HasForeignKey(x => x.PreferredCounsellorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<WellnessProgram>()
            .HasOne(x => x.Counsellor)
            .WithMany()
            .HasForeignKey(x => x.CounsellorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<WellnessSession>()
            .HasOne(x => x.Counsellor)
            .WithMany()
            .HasForeignKey(x => x.CounsellorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<WellnessSession>()
            .HasOne(x => x.Participant)
            .WithMany()
            .HasForeignKey(x => x.ParticipantId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<MoodCheckIn>()
            .HasOne(x => x.Participant)
            .WithMany()
            .HasForeignKey(x => x.ParticipantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<SessionRequest>()
            .HasOne(x => x.Participant)
            .WithMany()
            .HasForeignKey(x => x.ParticipantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<SessionRequest>()
            .HasOne(x => x.Counsellor)
            .WithMany()
            .HasForeignKey(x => x.CounsellorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
