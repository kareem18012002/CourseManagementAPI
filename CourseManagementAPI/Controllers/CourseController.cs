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
    public class CourseController : ControllerBase
    {
        private readonly CMSdbContext _context;
        private readonly IMappingService _mappingService;

        public CourseController(CMSdbContext context, IMappingService mappingService)
        {
            _context = context;
            _mappingService = mappingService;
        }

        // POST: api/course (Admin, Instructor only)
        [HttpPost]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<ActionResult<CourseDto>> CreateCourse([FromBody] CreateCourseDto createCourseDto)
        {
            if (createCourseDto == null)
                return BadRequest("Course data is required");

            var course = new Course
            {
                Title = createCourseDto.Title,
                Description = createCourseDto.Description,
                Price = createCourseDto.Price,
                IsDeleted = false
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCourseById), new { id = course.Id }, _mappingService.MapToCourseDto(course));
        }

        // GET: api/course (All authenticated users)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetAllCourses()
        {
            var courses = await _context.Courses
                .Where(c => !c.IsDeleted)
                .Include(c => c.Lessons)
                .ToListAsync();
            return Ok(courses.Select(_mappingService.MapToCourseDto).ToList());
        }

        // GET: api/course/{id} (All authenticated users)
        [HttpGet("{id}")]   
        public async Task<ActionResult<CourseDto>> GetCourseById(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Lessons)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (course == null)
                return NotFound($"Course with id {id} not found");

            return Ok(_mappingService.MapToCourseDto(course));
        }

        // PUT: api/course/{id} (Admin, Instructor only)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] UpdateCourseDto updateCourseDto)
        {
            if (id != updateCourseDto.Id)
                return BadRequest("Id mismatch");

            var existingCourse = await _context.Courses.FindAsync(id);
            if (existingCourse == null || existingCourse.IsDeleted)
                return NotFound($"Course with id {id} not found");

            existingCourse.Title = updateCourseDto.Title;
            existingCourse.Description = updateCourseDto.Description;
            existingCourse.Price = updateCourseDto.Price;

            _context.Courses.Update(existingCourse);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/course/{id} (Admin only)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null || course.IsDeleted)
                return NotFound($"Course with id {id} not found");

            course.IsDeleted = true;
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
