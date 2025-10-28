using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BeFitUniMvc.Features.Exercises;
using BeFitUniMvc.Models;

//using BeFitUniMvc.Features.Sessions;

namespace BeFitUniMvc.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options){}

    public DbSet<ExerciseType> ExerciseTypes { get; set; } = default!;
    public DbSet<TrainingSession> TrainingSessions { get; set; } = default!;
    public DbSet<PerformedExercise> PerformedExercises { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PerformedExercise>()
            .HasOne(pe => pe.TrainingSession)
            .WithMany(ts => ts.PerformedExercises)
            .HasForeignKey(pe => pe.TrainingSessionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PerformedExercise>()
            .HasOne(pe => pe.ExerciseType)
            .WithMany(et => et.PerformedExercises)
            .HasForeignKey(pe => pe.ExerciseTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
