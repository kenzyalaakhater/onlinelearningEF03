using OnlineLearningEF03.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace onlinelearningEF03.Models;

public class Enrollment
{
    public int StudentId { get; set; }
    public Student Student { get; set; } = null!;
    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;
    public DateTime EnrollmentDate { get; set; }
    public int ProgressPercentage { get; set; }
    public EnrollmentStatus Status { get; set; }
    public decimal? FinalGrade { get; set; }
}
