using CourseManagementAPI.DTOs;
using CourseManagementAPI.Models;

namespace CourseManagementAPI.Services
{
    public interface IMappingService
    {
        UserDto MapToUserDto(User user);
        CourseDto MapToCourseDto(Course course);
        LessonDto MapToLessonDto(Lesson lesson);
        EnrollmentDto MapToEnrollmentDto(Enrollment enrollment);
    }

    public class MappingService : IMappingService
    {
        public UserDto MapToUserDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role
            };
        }

        public CourseDto MapToCourseDto(Course course)
        {
            return new CourseDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                Price = course.Price,
                Lessons = course.Lessons?.Select(MapToLessonDto).ToList() ?? new List<LessonDto>()
            };
        }

        public LessonDto MapToLessonDto(Lesson lesson)
        {
            return new LessonDto
            {
                Id = lesson.Id,
                Title = lesson.Title,
                Content = lesson.Content,
                CourseId = lesson.CourseId
            };
        }

        public EnrollmentDto MapToEnrollmentDto(Enrollment enrollment)
        {
            return new EnrollmentDto
            {
                Id = enrollment.Id,
                UserId = enrollment.UserId,
                CourseId = enrollment.CourseId,
                EnrollDate = enrollment.EnrollDate,
                User = enrollment.User != null ? MapToUserDto(enrollment.User) : null,
                Course = enrollment.Course != null ? MapToCourseDto(enrollment.Course) : null
            };
        }
    }
}
