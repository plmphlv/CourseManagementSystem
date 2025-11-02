namespace Application.Courses.Common
{
    public abstract class CourseModel
    {
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public int? MemberLimit { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
