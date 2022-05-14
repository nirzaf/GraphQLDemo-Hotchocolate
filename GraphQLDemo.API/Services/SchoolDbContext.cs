using GraphQLDemo.API.DTOs;
using Microsoft.EntityFrameworkCore;

namespace GraphQLDemo.API.Services
{
    public class SchoolDbContext : DbContext
    {
        public SchoolDbContext(DbContextOptions<SchoolDbContext> options) 
            : base(options) { }

        public DbSet<CourseDto> Courses { get; set; }
        public DbSet<InstructorDto> Instructors { get; set; }
        public DbSet<StudentDto> Students { get; set; }
    }
}
