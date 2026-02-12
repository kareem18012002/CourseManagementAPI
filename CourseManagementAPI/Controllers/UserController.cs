using CourseManagementAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly CMSdbContext _context;

        public UserController(CMSdbContext context)
        {
            _context = context;
        }

        // POST: api/user (Admin only)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<User>> CreateUser([FromBody] User user)
        {
            if (user == null)
                return BadRequest("User data is required");

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

        // GET: api/user (Admin only)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            var users = await _context.Users.Where(u => !u.IsDeleted).ToListAsync();
            return Ok(users);
        }

        // GET: api/user/{id} (Admin or Self)
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
            var userRole = User.FindFirst("role")?.Value;

            // Admin can view any user, others can only view themselves
            if (userRole != "Admin" && userId != id)
                return Forbid("You can only view your own profile");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);

            if (user == null)
                return NotFound($"User with id {id} not found");

            return Ok(user);
        }

        // PUT: api/user/{id} (Admin or Self)
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {
            var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
            var userRole = User.FindFirst("role")?.Value;

            // Admin can update any user, others can only update themselves
            if (userRole != "Admin" && userId != id)
                return Forbid("You can only update your own profile");

            if (id != user.Id)
                return BadRequest("Id mismatch");

            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null || existingUser.IsDeleted)
                return NotFound($"User with id {id} not found");

            existingUser.Username = user.Username;
            existingUser.Password = user.Password;

            if (userRole == "Admin")
                existingUser.Role = user.Role;

            _context.Users.Update(existingUser);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/user/{id} (Admin only)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null || user.IsDeleted)
                return NotFound($"User with id {id} not found");

            user.IsDeleted = true;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
