using Microsoft.EntityFrameworkCore;
using onlinelearningEF03.Models;
using OnlineLearningEF03.Data;
using OnlineLearningEF03.Models;
namespace OnlineLearningEF03
{
    internal class Program
    {
        static void Main()
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    context.Database.Migrate();   // apply pending migrations
                }

                SeedStudentsAndEnrollments();

                Header("CREATE");
                AddInstructor();
                AddCourse(1, 3);        // valid
                AddCourse(99, 3);       // instructor does not exist
                AddCourse(1, 99);       // category does not exist

                AddStudents();
                EnrollStudent(5, 1);    // valid (first run)
                EnrollStudent(5, 1);    // already enrolled
                EnrollStudent(1, 6);    // course not published
                EnrollStudent(99, 1);   // student does not exist
                EnrollStudent(1, 99);   // course does not exist

                Header("READ");
                FindCourse(2, "Advanced EF Core");
                FindCourse(999, "No Such Course");
                FilterCourses(40m, 100m);
                ShowSubCategories(1);
                ShowSubCategories(99);
                CourseRoster(1);
                CourseRoster(99);

                Header("UPDATE");
                ChangePrice(1, 54.99m);
                ChangePrice(99, 10m);
                ChangePrice(1, -5m);
                UpdateProgress(1, 1, 75);
                UpdateProgress(2, 3, 100);    // becomes Completed
                UpdateProgress(1, 1, 150);    // rejected
                UpdateProgress(99, 99, 50);   // enrollment not found
                PublishCourse(6);
                PublishCourse(6);             // already published
                PublishCourse(99);

                Header("DELETE");
                Unenroll(4, 1);
                Unenroll(4, 1);               // already gone
                DeleteCategoryUnsafe(2);      // has sub-categories -> DbUpdateException
                DeleteCategorySafe(2);        // refused politely
                DeleteCategorySafe(7);        // empty category -> deleted
                DeleteCourse(3);              // has enrollments
                DeleteCourse(99);

                Header("BULK OPERATIONS");
                BulkPriceRaise(CourseLevel.Intermediate);
                BulkDeleteDropped();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        }
        static void Header(string title)
        {
            Console.WriteLine();
            Console.WriteLine($"--- {title} ---");
        }
        // SEEDING FROM CODE (only when the table is empty)
        static void SeedStudentsAndEnrollments()
        {
            using var context = new AppDbContext();

            if (!context.Students.Any())
            {
                context.Students.AddRange(
                    new Student { FullName = "Omar Hassan", Email = "omar.hassan@mail.com", DateOfBirth = new DateTime(2000, 5, 14) },
                    new Student { FullName = "Laila Samir", Email = "laila.samir@mail.com", DateOfBirth = new DateTime(1999, 11, 2) },
                    new Student { FullName = "Karim Adel", Email = "karim.adel@mail.com", DateOfBirth = new DateTime(2001, 8, 23) },
                    new Student { FullName = "Nour Ali", Email = "nour.ali@mail.com", DateOfBirth = new DateTime(1998, 1, 30) },
                    new Student { FullName = "Hana Mostafa", Email = "hana.mostafa@mail.com", DateOfBirth = new DateTime(2002, 3, 9) });
                context.SaveChanges();
                Console.WriteLine("Seeded 5 students.");
            }

            if (!context.Enrollments.Any())
            {
                var ids = context.Students.OrderBy(s => s.Id).Select(s => s.Id).Take(5).ToList();
                var existingCourseIds = context.Courses.Select(c => c.Id).ToList();

                if (ids.Count == 5)
                {
                    var enrollments = new List<Enrollment>
                    {
                        new() { StudentId = ids[0], CourseId = 1, EnrollmentDate = new DateTime(2025, 1, 10), ProgressPercentage = 40,  Status = EnrollmentStatus.Active },
                        new() { StudentId = ids[0], CourseId = 2, EnrollmentDate = new DateTime(2025, 1, 12), ProgressPercentage = 100, Status = EnrollmentStatus.Completed, FinalGrade = 88.50m },
                        new() { StudentId = ids[1], CourseId = 1, EnrollmentDate = new DateTime(2025, 2, 1),  ProgressPercentage = 100, Status = EnrollmentStatus.Completed, FinalGrade = 92.00m },
                        new() { StudentId = ids[1], CourseId = 3, EnrollmentDate = new DateTime(2025, 2, 5),  ProgressPercentage = 25,  Status = EnrollmentStatus.Active },
                        new() { StudentId = ids[2], CourseId = 2, EnrollmentDate = new DateTime(2025, 2, 10), ProgressPercentage = 10,  Status = EnrollmentStatus.Dropped },
                        new() { StudentId = ids[2], CourseId = 5, EnrollmentDate = new DateTime(2025, 2, 15), ProgressPercentage = 60,  Status = EnrollmentStatus.Active },
                        new() { StudentId = ids[3], CourseId = 1, EnrollmentDate = new DateTime(2025, 3, 1),  ProgressPercentage = 5,   Status = EnrollmentStatus.Dropped },
                        new() { StudentId = ids[4], CourseId = 3, EnrollmentDate = new DateTime(2025, 3, 5),  ProgressPercentage = 100, Status = EnrollmentStatus.Completed, FinalGrade = 79.25m }
                    };

                    context.Enrollments.AddRange(enrollments.Where(e => existingCourseIds.Contains(e.CourseId)));
                    context.SaveChanges();
                    Console.WriteLine("Seeded enrollments.");
                }
            }
        }
        // CREATE
        static void AddInstructor()
        {
            using var context = new AppDbContext();

            var instructor = new Instructor
            {
                FullName = "Nadia Salem",
                Email = "nadia.salem@learnhub.com",
                Bio = "Cloud and DevOps instructor.",
                JoinedDate = DateTime.Today
            };

            context.Instructors.Add(instructor);
            context.SaveChanges();
            Console.WriteLine($"Instructor added. Generated Id = {instructor.Id}");
        }

        static void AddCourse(int instructorId, int categoryId)
        {
            using var context = new AppDbContext();

            if (!context.Instructors.Any(i => i.Id == instructorId))
            {
                Console.WriteLine($"Cannot add course: instructor {instructorId} does not exist.");
                return;
            }

            if (!context.Categories.Any(c => c.Id == categoryId))
            {
                Console.WriteLine($"Cannot add course: category {categoryId} does not exist.");
                return;
            }

            var course = new Course
            {
                Title = "Docker for .NET Developers",
                Description = "Containerize your .NET apps.",
                Price = 79.00m,
                Level = CourseLevel.Intermediate,
                IsPublished = false,
                InstructorId = instructorId,
                CategoryId = categoryId
            };

            context.Courses.Add(course);
            context.SaveChanges();
            Console.WriteLine($"Course added. Generated Id = {course.Id}");
        }

        static void AddStudents()
        {
            using var context = new AppDbContext();

            var students = new List<Student>
            {
                new() { FullName = "Salma Tarek", Email = "salma.tarek@mail.com", DateOfBirth = new DateTime(2000, 7, 7) },
                new() { FullName = "Ali Mahmoud", Email = "ali.mahmoud@mail.com", DateOfBirth = new DateTime(1997, 12, 19) },
                new() { FullName = "Dina Wael",   Email = "dina.wael@mail.com",   DateOfBirth = new DateTime(2003, 4, 25) }
            };

            context.Students.AddRange(students);   // one AddRange
            context.SaveChanges();                 // one SaveChanges

            foreach (var s in students)
                Console.WriteLine($"Student added: {s.FullName} (Id = {s.Id})");
        }

        static void EnrollStudent(int studentId, int courseId)
        {
            using var context = new AppDbContext();

            var student = context.Students.Find(studentId);
            if (student == null)
            {
                Console.WriteLine($"Enrollment refused: student {studentId} does not exist.");
                return;
            }

            var course = context.Courses.Find(courseId);
            if (course == null)
            {
                Console.WriteLine($"Enrollment refused: course {courseId} does not exist.");
                return;
            }

            if (!course.IsPublished)
            {
                Console.WriteLine($"Enrollment refused: course '{course.Title}' is not published.");
                return;
            }

            if (context.Enrollments.Any(e => e.StudentId == studentId && e.CourseId == courseId))
            {
                Console.WriteLine($"Enrollment refused: {student.FullName} is already enrolled in '{course.Title}'.");
                return;
            }

            context.Enrollments.Add(new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId,
                EnrollmentDate = DateTime.Now,
                ProgressPercentage = 0,
                Status = EnrollmentStatus.Active
            });
            context.SaveChanges();
            Console.WriteLine($"{student.FullName} enrolled in '{course.Title}'.");
        }
        // READ
        static void FindCourse(int id, string title)
        {
            using var context = new AppDbContext();

            var byId = context.Courses.Find(id);
            Console.WriteLine(byId == null
                ? $"Find({id}): course not found."
                : $"Find({id}): {byId.Title} ({byId.Level})");

            var byTitle = context.Courses.FirstOrDefault(c => c.Title == title);
            Console.WriteLine(byTitle == null
                ? $"FirstOrDefault('{title}'): course not found."
                : $"FirstOrDefault('{title}'): Id = {byTitle.Id}, Price = {byTitle.Price:F2}");
        }
        static void FilterCourses(decimal min, decimal max)
        {
            using var context = new AppDbContext();

            var courses = context.Courses
                .AsNoTracking()
                .Where(c => c.Price >= min && c.Price <= max)
                .OrderByDescending(c => c.Price)
                .Select(c => new { c.Title, c.Level, c.Price })
                .ToList();

            Console.WriteLine($"Courses priced between {min:F2} and {max:F2}:");
            if (courses.Count == 0)
                Console.WriteLine("  (none)");
            foreach (var c in courses)
                Console.WriteLine($"  {c.Title} | {c.Level} | {c.Price:F2}");
        }
        static void ShowSubCategories(int parentId)
        {
            using var context = new AppDbContext();

            var parent = context.Categories.Find(parentId);
            if (parent == null)
            {
                Console.WriteLine($"Category {parentId} not found.");
            }
            else
            {
                var subs = context.Categories
                    .AsNoTracking()
                    .Where(c => c.ParentCategoryId == parentId)
                    .ToList();

                Console.WriteLine($"Sub-categories of '{parent.Name}':");
                if (subs.Count == 0)
                    Console.WriteLine("  (none)");
                foreach (var s in subs)
                    Console.WriteLine($"  {s.Id} - {s.Name}");
            }

            var topLevel = context.Categories
                .AsNoTracking()
                .Where(c => c.ParentCategoryId == null)
                .ToList();

            Console.WriteLine("Top-level categories:");
            foreach (var t in topLevel)
                Console.WriteLine($"  {t.Id} - {t.Name}");
        }
        static void CourseRoster(int courseId)
        {
            using var context = new AppDbContext();

            var course = context.Courses.Find(courseId);
            if (course == null)
            {
                Console.WriteLine($"Roster: course {courseId} not found.");
                return;
            }

            // Step 1: student Ids from the enrollments
            var studentIds = context.Enrollments
                .Where(e => e.CourseId == courseId)
                .Select(e => e.StudentId)
                .ToList();

            // Step 2: students with Contains (no joins)
            var names = context.Students
                .Where(s => studentIds.Contains(s.Id))
                .Select(s => s.FullName)
                .ToList();

            Console.WriteLine($"'{course.Title}' has {studentIds.Count} enrolled student(s):");
            foreach (var n in names)
                Console.WriteLine($"  {n}");
        }
        // UPDATE
        static void ChangePrice(int courseId, decimal newPrice)
        {
            if (newPrice < 0)
            {
                Console.WriteLine($"Price {newPrice} is invalid: it cannot be negative.");
                return;
            }

            using var context = new AppDbContext();

            var course = context.Courses.Find(courseId);
            if (course == null)
            {
                Console.WriteLine($"Change price: course {courseId} not found.");
                return;
            }

            var oldPrice = course.Price;
            course.Price = newPrice;
            context.SaveChanges();
            Console.WriteLine($"'{course.Title}' price changed from {oldPrice:F2} to {newPrice:F2}.");
        }

        static void UpdateProgress(int studentId, int courseId, int progress)
        {
            if (progress < 0 || progress > 100)
            {
                Console.WriteLine($"Progress {progress} rejected: it must be between 0 and 100.");
                return;
            }

            using var context = new AppDbContext();

            // Find with the composite key (order: StudentId, CourseId)
            var enrollment = context.Enrollments.Find(studentId, courseId);
            if (enrollment == null)
            {
                Console.WriteLine($"Enrollment (student {studentId}, course {courseId}) not found.");
                return;
            }

            enrollment.ProgressPercentage = progress;

            if (progress == 100)
            {
                enrollment.Status = EnrollmentStatus.Completed;
            }
            else if (enrollment.Status == EnrollmentStatus.Completed)
            {
                // No longer at 100: it is not completed anymore, so no final grade
                enrollment.Status = EnrollmentStatus.Active;
                enrollment.FinalGrade = null;
            }

            context.SaveChanges();
            Console.WriteLine($"Enrollment (student {studentId}, course {courseId}): progress = {progress}%, status = {enrollment.Status}.");
        }

        static void PublishCourse(int courseId)
        {
            using var context = new AppDbContext();

            var course = context.Courses.Find(courseId);
            if (course == null)
            {
                Console.WriteLine($"Publish: course {courseId} not found.");
                return;
            }

            if (course.IsPublished)
            {
                Console.WriteLine($"Course '{course.Title}' is already published.");
                return;
            }

            course.IsPublished = true;
            course.PublishedDate = DateTime.Now;
            context.SaveChanges();
            Console.WriteLine($"Course '{course.Title}' published on {course.PublishedDate:d}.");
        }
        // DELETE
        static void Unenroll(int studentId, int courseId)
        {
            using var context = new AppDbContext();

            var enrollment = context.Enrollments.Find(studentId, courseId);
            if (enrollment == null)
            {
                Console.WriteLine($"Unenroll: enrollment (student {studentId}, course {courseId}) not found.");
                return;
            }

            context.Enrollments.Remove(enrollment);
            context.SaveChanges();
            Console.WriteLine($"Enrollment (student {studentId}, course {courseId}) deleted.");
        }

        // Unsafe version: just try to delete, and catch the database error
        static void DeleteCategoryUnsafe(int categoryId)
        {
            using var context = new AppDbContext();

            var category = context.Categories.Find(categoryId);
            if (category == null)
            {
                Console.WriteLine($"Category {categoryId} not found.");
                return;
            }

            try
            {
                context.Categories.Remove(category);
                context.SaveChanges();
                Console.WriteLine($"Category '{category.Name}' deleted.");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("DbUpdateException: " + ex.Message);
                Console.WriteLine("Inner exception: " + ex.InnerException?.Message);
            }
        }

        // Safe version: check first, then delete
        static void DeleteCategorySafe(int categoryId)
        {
            using var context = new AppDbContext();

            var category = context.Categories.Find(categoryId);
            if (category == null)
            {
                Console.WriteLine($"Category {categoryId} not found.");
                return;
            }

            if (context.Categories.Any(c => c.ParentCategoryId == categoryId))
            {
                Console.WriteLine($"Cannot delete '{category.Name}': it still has sub-categories.");
                return;
            }

            if (context.Courses.Any(c => c.CategoryId == categoryId))
            {
                Console.WriteLine($"Cannot delete '{category.Name}': it still has courses.");
                return;
            }

            context.Categories.Remove(category);
            context.SaveChanges();
            Console.WriteLine($"Category '{category.Name}' deleted safely.");
        }

        static void DeleteCourse(int courseId)
        {
            using var context = new AppDbContext();

            var course = context.Courses.Find(courseId);
            if (course == null)
            {
                Console.WriteLine($"Delete course: course {courseId} not found.");
                return;
            }

            var before = context.Enrollments.Count(e => e.CourseId == courseId);
            Console.WriteLine($"Enrollments of '{course.Title}' before delete: {before}");

            context.Courses.Remove(course);
            context.SaveChanges();

            var after = context.Enrollments.Count(e => e.CourseId == courseId);
            Console.WriteLine($"Enrollments of '{course.Title}' after delete: {after}");

            // Why were they removed? Enrollment -> Course is a REQUIRED relationship,
            // configured with DeleteBehavior.Cascade. The foreign key in the database is
            // created with ON DELETE CASCADE, so SQL Server itself deletes every enrollment
            // of the course when the course row is deleted.
        }
        // BULK OPERATIONS 
        static void BulkPriceRaise(CourseLevel level)
        {
            using var context = new AppDbContext();

            int rows = context.Courses
                .Where(c => c.IsPublished && c.Level == level)
                .ExecuteUpdate(s => s.SetProperty(c => c.Price, c => c.Price * 1.10m));

            Console.WriteLine($"Price raised by 10% for published {level} courses. Rows affected: {rows}");
        }

        static void BulkDeleteDropped()
        {
            using var context = new AppDbContext();

            int rows = context.Enrollments
                .Where(e => e.Status == EnrollmentStatus.Dropped)
                .ExecuteDelete();

            Console.WriteLine($"Dropped enrollments deleted. Rows affected: {rows}");
        }
    }
}