using CourseManagementAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LessonController : ControllerBase
    {
        private readonly CMSdbContext _context;

        public LessonController(CMSdbContext context)
        {
            _context = context;
        }

        // POST: api/lesson (Admin, Instructor only)
        [HttpPost]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<ActionResult<Lesson>> CreateLesson([FromBody] Lesson lesson)
        {
            if (lesson == null)
                return BadRequest("Lesson data is required");

            // Check if course exists
            var courseExists = await _context.Courses.AnyAsync(c => c.Id == lesson.CourseId && !c.IsDeleted);
            if (!courseExists)
                return BadRequest($"Course with id {lesson.CourseId} not found");

            _context.Lessons.Add(lesson);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLessonById), new { id = lesson.Id }, lesson);
        }

        // GET: api/lesson (All authenticated users)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Lesson>>> GetAllLessons()
        {
            var lessons = await _context.Lessons
                .Include(l => l.Course)
                .ToListAsync();
            return Ok(lessons);
        }

        // GET: api/lesson/{id} (All authenticated users)
        [HttpGet("{id}")]
        public async Task<ActionResult<Lesson>> GetLessonById(int id)
        {
            var lesson = await _context.Lessons
                .Include(l => l.Course)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lesson == null)
                return NotFound($"Lesson with id {id} not found");

            return Ok(lesson);
        }

        // GET: api/lesson/course/{courseId} (All authenticated users)
        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<IEnumerable<Lesson>>> GetLessonsByCourse(int courseId)
        {
            var lessons = await _context.Lessons
                .Where(l => l.CourseId == courseId)
                .Include(l => l.Course)
                .ToListAsync();

            if (!lessons.Any())
                return NotFound($"No lessons found for course with id {courseId}");

            return Ok(lessons);
        }

        // PUT: api/lesson/{id} (Admin, Instructor only)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> UpdateLesson(int id, [FromBody] Lesson lesson)
        {
            if (id != lesson.Id)
                return BadRequest("Id mismatch");

            var existingLesson = await _context.Lessons.FindAsync(id);
            if (existingLesson == null)
                return NotFound($"Lesson with id {id} not found");

            // Check if course exists
            var courseExists = await _context.Courses.AnyAsync(c => c.Id == lesson.CourseId && !c.IsDeleted);
            if (!courseExists)
                return BadRequest($"Course with id {lesson.CourseId} not found");

            existingLesson.Title = lesson.Title;
            existingLesson.Content = lesson.Content;
            existingLesson.CourseId = lesson.CourseId;

            _context.Lessons.Update(existingLesson);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/lesson/{id} (Admin only)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteLesson(int id)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson == null)
                return NotFound($"Lesson with id {id} not found");

            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
