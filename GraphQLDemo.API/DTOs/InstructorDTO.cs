using System;
using System.Collections.Generic;

namespace GraphQLDemo.API.DTOs
{
    public class InstructorDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public double Salary { get; set; }
        public IEnumerable<CourseDto> Courses { get; set; }
    }
}
