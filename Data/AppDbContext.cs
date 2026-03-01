using Microsoft.EntityFrameworkCore;
using StudentApi.Models;

namespace StudentApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Student> Students => Set<Student>();
    public DbSet<UserInfo> Users => Set<UserInfo>();
    public DbSet<Class> Classes => Set<Class>();
    public DbSet<StudentClassXref> StudentClassXrefs => Set<StudentClassXref>();
    public DbSet<Subject> Subjects => Set<Subject>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Composite PK for the join table
        modelBuilder.Entity<StudentClassXref>()
            .HasKey(x => new { x.StudentId, x.ClassId });

        modelBuilder.Entity<StudentClassXref>()
            .HasOne<Student>()
            .WithMany()
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<StudentClassXref>()
            .HasOne<Class>()
            .WithMany()
            .HasForeignKey(x => x.ClassId)
            .OnDelete(DeleteBehavior.Cascade);

        // One-to-one: Class has one Subject, Subject belongs to one Class
        modelBuilder.Entity<Class>()
            .HasOne(c => c.Subject)
            .WithOne(s => s.Class)
            .HasForeignKey<Subject>(s => s.ClassId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
