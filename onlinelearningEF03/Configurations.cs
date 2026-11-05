using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using onlinelearningEF03.Models;
using OnlineLearningEF03.Models;
namespace OnlineLearningEF03.Configurations;
public class InstructorConfiguration : IEntityTypeConfiguration<Instructor>
{
    public void Configure(EntityTypeBuilder<Instructor> builder)
    {
        builder.ToTable("Instructors");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.FullName).IsRequired().HasMaxLength(150);
        builder.Property(i => i.Email).IsRequired().HasMaxLength(200);
        builder.Property(i => i.Bio).HasMaxLength(1000);
        builder.Property(i => i.JoinedDate).IsRequired();
       builder.HasData(GetSeed());   // SEED
    }
    private static Instructor[] GetSeed() =>
    [
        new Instructor { Id = 1, FullName = "Ahmed Kamal", Email = "ahmed.kamal@learnhub.com",
                         Bio = "Senior .NET developer and trainer.", JoinedDate = new DateTime(2022, 3, 1) },
        new Instructor { Id = 2, FullName = "Mona Fathy", Email = "mona.fathy@learnhub.com",
                         Bio = "Database architect with 10+ years of experience.", JoinedDate = new DateTime(2022, 9, 15) },
        new Instructor { Id = 3, FullName = "Youssef Nabil", Email = "youssef.nabil@learnhub.com",
                         Bio = null, JoinedDate = new DateTime(2023, 1, 10) }
    ];
}
public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Description).HasMaxLength(500);

        // Self-reference: a parent that still has sub-categories cannot be deleted
        builder.HasOne(c => c.ParentCategory)
               .WithMany(c => c.SubCategories)
               .HasForeignKey(c => c.ParentCategoryId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(GetSeed());   // SEED
    }
    // 3 levels: Programming -> Backend -> ASP.NET
    private static Category[] GetSeed() =>
    [
        new Category { Id = 1, Name = "Programming", Description = "All programming topics", ParentCategoryId = null },
        new Category { Id = 2, Name = "Backend", Description = "Server-side development", ParentCategoryId = 1 },
        new Category { Id = 3, Name = "ASP.NET", Description = "ASP.NET and .NET web", ParentCategoryId = 2 },
        new Category { Id = 4, Name = "Design", Description = null, ParentCategoryId = null },
        new Category { Id = 5, Name = "UI/UX", Description = "User interface and experience", ParentCategoryId = 4 },
        new Category { Id = 6, Name = "Frontend", Description = "Client-side development", ParentCategoryId = 1 },
        new Category { Id = 7, Name = "Archive", Description = "Empty category (safe to delete)", ParentCategoryId = null }
    ];
}
public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("Courses");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Title).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Description).HasMaxLength(2000);
        builder.Property(c => c.Price).IsRequired().HasPrecision(10, 2);
        builder.Property(c => c.Level).IsRequired().HasConversion<string>().HasMaxLength(20);
        builder.Property(c => c.IsPublished).IsRequired();
        builder.Property(c => c.PublishedDate);   // nullable

        // Instructor 1 --- * Course
        builder.HasOne(c => c.Instructor)
               .WithMany(i => i.Courses)
               .HasForeignKey(c => c.InstructorId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

        // Category 1 --- * Course
        builder.HasOne(c => c.Category)
               .WithMany(cat => cat.Courses)
               .HasForeignKey(c => c.CategoryId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(GetSeed());   // SEED
    }

    private static Course[] GetSeed() =>
    [
        new Course { Id = 1, Title = "C# Fundamentals", Description = "Learn C# from scratch.",
                     Price = 49.99m, Level = CourseLevel.Beginner, IsPublished = true,
                     PublishedDate = new DateTime(2024, 1, 15), InstructorId = 1, CategoryId = 1 },
        new Course { Id = 2, Title = "ASP.NET Core Web API", Description = "Build REST APIs.",
                     Price = 89.99m, Level = CourseLevel.Intermediate, IsPublished = true,
                     PublishedDate = new DateTime(2024, 3, 10), InstructorId = 1, CategoryId = 3 },
        new Course { Id = 3, Title = "Advanced EF Core", Description = "Deep dive into EF Core.",
                     Price = 119.99m, Level = CourseLevel.Advanced, IsPublished = true,
                     PublishedDate = new DateTime(2024, 5, 20), InstructorId = 2, CategoryId = 2 },
        new Course { Id = 4, Title = "SQL Server Essentials", Description = null,
                     Price = 59.00m, Level = CourseLevel.Beginner, IsPublished = false,
                     PublishedDate = null, InstructorId = 2, CategoryId = 2 },
        new Course { Id = 5, Title = "Modern JavaScript", Description = "ES6 and beyond.",
                     Price = 74.50m, Level = CourseLevel.Intermediate, IsPublished = true,
                     PublishedDate = new DateTime(2024, 7, 1), InstructorId = 3, CategoryId = 6 },
        new Course { Id = 6, Title = "UI/UX Design Basics", Description = "Design principles.",
                     Price = 39.00m, Level = CourseLevel.Beginner, IsPublished = false,
                     PublishedDate = null, InstructorId = 3, CategoryId = 5 }
    ];
}
public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("Students");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.FullName).IsRequired().HasMaxLength(150);
        builder.Property(s => s.Email).IsRequired().HasMaxLength(200);
        builder.Property(s => s.DateOfBirth).IsRequired();
    }
}
public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        // Progress must be between 0 and 100 (also enforced in the database)
        builder.ToTable("Enrollments", t =>
            t.HasCheckConstraint("CK_Enrollments_Progress",
                "[ProgressPercentage] >= 0 AND [ProgressPercentage] <= 100"));

        // Composite key: a student can enroll in the same course only once
        builder.HasKey(e => new { e.StudentId, e.CourseId });

        builder.Property(e => e.EnrollmentDate).IsRequired();
        builder.Property(e => e.ProgressPercentage).IsRequired();
        builder.Property(e => e.Status).IsRequired().HasConversion<string>().HasMaxLength(20);
        builder.Property(e => e.FinalGrade).HasPrecision(5, 2);   // nullable

        // Student 1 --- * Enrollment
        builder.HasOne(e => e.Student)
               .WithMany(s => s.Enrollments)
               .HasForeignKey(e => e.StudentId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        // Course 1 --- * Enrollment
        builder.HasOne(e => e.Course)
               .WithMany(c => c.Enrollments)
               .HasForeignKey(e => e.CourseId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}
