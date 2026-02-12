using CourseManagementAPI.DTOs;
using CourseManagementAPI.Models;
using CourseManagementAPI.Services;
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
        private readonly IMappingService _mappingService;

        public LessonController(CMSdbContext context, IMappingService mappingService)
        {
            _context = context;
            _mappingService = mappingService;
        }

        // POST: api/lesson (Admin, Instructor only)
        [HttpPost]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<ActionResult<LessonDto>> CreateLesson([FromBody] CreateLessonDto createLessonDto)
        {
            if (createLessonDto == null)
                return BadRequest("Lesson data is required");

            // Check if course exists
            var courseExists = await _context.Courses.AnyAsync(c => c.Id == createLessonDto.CourseId && !c.IsDeleted);
            if (!courseExists)
                return BadRequest($"Course with id {createLessonDto.CourseId} not found");

            var lesson = new Lesson
            {
                Title = createLessonDto.Title,
                Content = createLessonDto.Content,
                CourseId = createLessonDto.CourseId
            };

            _context.Lessons.Add(lesson);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLessonById), new { id = lesson.Id }, _mappingService.MapToLessonDto(lesson));
        }

        // GET: api/lesson (All authenticated users)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LessonDto>>> GetAllLessons()
        {
            var lessons = await _context.Lessons
                .Include(l => l.Course)
                .ToListAsync();
            return Ok(lessons.Select(_mappingService.MapToLessonDto).ToList());
        }

        // GET: api/lesson/{id} (All authenticated users)
        [HttpGet("{id}")]
        public async Task<ActionResult<LessonDto>> GetLessonById(int id)
        {
            var lesson = await _context.Lessons
                .Include(l => l.Course)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lesson == null)
                return NotFound($"Lesson with id {id} not found");

            return Ok(_mappingService.MapToLessonDto(lesson));
        }

        // GET: api/lesson/course/{courseId} (All authenticated users)
        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<IEnumerable<LessonDto>>> GetLessonsByCourse(int courseId)
        {
            var lessons = await _context.Lessons
                .Where(l => l.CourseId == courseId)
                .Include(l => l.Course)
                .ToListAsync();

            if (!lessons.Any())
                return NotFound($"No lessons found for course with id {courseId}");

            return Ok(lessons.Select(_mappingService.MapToLessonDto).ToList());
        }

        // PUT: api/lesson/{id} (Admin, Instructor only)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> UpdateLesson(int id, [FromBody] UpdateLessonDto updateLessonDto)
        {
            if (id != updateLessonDto.Id)
                return BadRequest("Id mismatch");

            var existingLesson = await _context.Lessons.FindAsync(id);
            if (existingLesson == null)
                return NotFound($"Lesson with id {id} not found");

            // Check if course exists
            var courseExists = await _context.Courses.AnyAsync(c => c.Id == updateLessonDto.CourseId && !c.IsDeleted);
            if (!courseExists)
                return BadRequest($"Course with id {updateLessonDto.CourseId} not found");

            existingLesson.Title = updateLessonDto.Title;
            existingLesson.Content = updateLessonDto.Content;
            existingLesson.CourseId = updateLessonDto.CourseId;

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
