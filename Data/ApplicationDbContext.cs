using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BeFitUniMvc.Features.Exercises;
using BeFitUniMvc.Features.Sessions;

namespace BeFitUniMvc.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<ExerciseType> ExerciseTypes { get; set; }
    public DbSet<TrainingSession> TrainingSessions { get; set; }
    public DbSet<PerformedExercise> PerformedExercises { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<PerformedExercise>()
            .HasOne(p => p.TrainingSession)
            .WithMany(s => s.PerformedExercises)
            .HasForeignKey(p => p.TrainingSessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<PerformedExercise>()
            .HasOne(p => p.ExerciseType)
            .WithMany(t => t.PerformedExercises)
            .HasForeignKey(p => p.ExerciseTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
