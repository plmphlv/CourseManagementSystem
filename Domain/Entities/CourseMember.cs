namespace Domain.Entities
{
    public class CourseMember
    {
        public int CourseId { get; set; }

        public Course Course { get; set; } = null!;

        public string MemberId { get; set; } = null!;

        public User Member { get; set; } = null!;
    }
}
