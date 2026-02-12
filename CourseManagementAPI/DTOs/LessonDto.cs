namespace CourseManagementAPI.DTOs
{
    public class CreateLessonDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public int CourseId { get; set; }
    }

    public class UpdateLessonDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int CourseId { get; set; }
    }

    public class LessonDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int CourseId { get; set; }
    }
}
