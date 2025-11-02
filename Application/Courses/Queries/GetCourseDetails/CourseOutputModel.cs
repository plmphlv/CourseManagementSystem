using Application.Courses.Common;

namespace Application.Courses.Queries.GetCourseDetails
{
    public class CourseOutputModel : CourseModel
    {
        public int Id { get; set; }

        public string CompletionPercentage { get; set; } = null!;

        public string InstructorName { get; set; } = null!;
    }
}