using System;
using System.Collections.Generic;

namespace GraphQLDemo.API.DTOs
{
    public class StudentDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public double Gpa { get; set; }
        public IEnumerable<CourseDto> Courses { get; set; }
    }
}
