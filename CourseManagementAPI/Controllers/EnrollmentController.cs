using CourseManagementAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EnrollmentController : ControllerBase
    {
        private readonly CMSdbContext _context;

        public EnrollmentController(CMSdbContext context)
        {
            _context = context;
        }

        // POST: api/enrollment (Student can enroll in courses)
        [HttpPost]
        [Authorize(Roles = "Student,Admin")]
        public async Task<ActionResult<Enrollment>> EnrollUserInCourse([FromBody] Enrollment enrollment)
        {
            if (enrollment == null)
                return BadRequest("Enrollment data is required");

            var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
            var userRole = User.FindFirst("role")?.Value;

            // Students can only enroll themselves, Admin can enroll anyone
            if (userRole != "Admin" && userId != enrollment.UserId)
                return Forbid("You can only enroll yourself");

            // Check if user exists
            var userExists = await _context.Users.AnyAsync(u => u.Id == enrollment.UserId && !u.IsDeleted);
            if (!userExists)
                return BadRequest($"User with id {enrollment.UserId} not found");

            // Check if course exists
            var courseExists = await _context.Courses.AnyAsync(c => c.Id == enrollment.CourseId && !c.IsDeleted);
            if (!courseExists)
                return BadRequest($"Course with id {enrollment.CourseId} not found");

            // Check if user is already enrolled
            var alreadyEnrolled = await _context.Enrollments
                .AnyAsync(e => e.UserId == enrollment.UserId && e.CourseId == enrollment.CourseId);
            if (alreadyEnrolled)
                return BadRequest("User is already enrolled in this course");

            enrollment.EnrollDate = DateTime.UtcNow;
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEnrollmentById), new { id = enrollment.Id }, enrollment);
        }

        // GET: api/enrollment (Admin only)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Enrollment>>> GetAllEnrollments()
        {
            var enrollments = await _context.Enrollments
                .Include(e => e.User)
                .Include(e => e.Course)
                .ToListAsync();
            return Ok(enrollments);
        }

        // GET: api/enrollment/{id} (Admin or Owner)
        [HttpGet("{id}")]
        public async Task<ActionResult<Enrollment>> GetEnrollmentById(int id)
        {
            var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
            var userRole = User.FindFirst("role")?.Value;

            var enrollment = await _context.Enrollments
                .Include(e => e.User)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (enrollment == null)
                return NotFound($"Enrollment with id {id} not found");

            // Admin can view any enrollment, others can only view their own
            if (userRole != "Admin" && userId != enrollment.UserId)
                return Forbid("You can only view your own enrollments");

            return Ok(enrollment);
        }

        // GET: api/enrollment/user/{userId} (User or Admin)
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Enrollment>>> GetUserEnrollments(int userId)
        {
            var currentUserId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
            var userRole = User.FindFirst("role")?.Value;

            // Admin can view any user's enrollments, others can only view their own
            if (userRole != "Admin" && currentUserId != userId)
                return Forbid("You can only view your own enrollments");

            var enrollments = await _context.Enrollments
                .Where(e => e.UserId == userId)
                .Include(e => e.User)
                .Include(e => e.Course)
                .ToListAsync();

            if (!enrollments.Any())
                return NotFound($"No enrollments found for user with id {userId}");

            return Ok(enrollments);
        }

        // DELETE: api/enrollment/{id} (Admin or Owner)
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelEnrollment(int id)
        {
            var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
            var userRole = User.FindFirst("role")?.Value;

            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
                return NotFound($"Enrollment with id {id} not found");

            // Admin can cancel any enrollment, students can only cancel their own
            if (userRole != "Admin" && userId != enrollment.UserId)
                return Forbid("You can only cancel your own enrollments");

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
