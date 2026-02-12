namespace CourseManagementAPI.DTOs
{
    public class CreateEnrollmentDto
    {
        public int UserId { get; set; }
        public int CourseId { get; set; }
    }

    public class EnrollmentDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrollDate { get; set; }
        public UserDto User { get; set; }
        public CourseDto Course { get; set; }
    }
}
