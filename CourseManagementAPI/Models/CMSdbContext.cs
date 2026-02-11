using Microsoft.EntityFrameworkCore;

namespace CourseManagementAPI.Models
{
    public class CMSdbContext: DbContext    
    {
        public CMSdbContext(DbContextOptions<CMSdbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

    }
}
