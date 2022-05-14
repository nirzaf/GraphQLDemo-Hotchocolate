using GraphQLDemo.API.Models;
using System;
using System.Collections.Generic;

namespace GraphQLDemo.API.DTOs
{
    public class CourseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Subject Subject { get; set; }
        public string CreatorId { get; set; }
        public Guid InstructorId { get; set; }
        public InstructorDto Instructor { get; set; }
        public IEnumerable<StudentDto> Students { get; set; }
    }
}
