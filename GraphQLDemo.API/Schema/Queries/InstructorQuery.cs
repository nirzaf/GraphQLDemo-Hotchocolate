using GraphQLDemo.API.DTOs;
using GraphQLDemo.API.Services;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GraphQLDemo.API.Schema.Queries
{
    [ExtendObjectType(typeof(Query))]
    public class InstructorQuery
    {
        [UseDbContext(typeof(SchoolDbContext))]
        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<InstructorType> GetInstructors([ScopedService] SchoolDbContext context)
        {
            return context.Instructors.Select(i => new InstructorType
            {
                Id = i.Id,
                FirstName = i.FirstName,
                LastName = i.LastName,
                Salary = i.Salary,
            });
        }

        [UseDbContext(typeof(SchoolDbContext))]
        public async Task<InstructorType> GetInstructorById(Guid id, [ScopedService] SchoolDbContext context)
        {
            InstructorDto instructorDto = await context.Instructors.FindAsync(id);

            if(instructorDto == null)
            {
                return null;
            }

            return new InstructorType
            {
                Id = instructorDto.Id,
                FirstName = instructorDto.FirstName,
                LastName = instructorDto.LastName,
                Salary = instructorDto.Salary,
            };
        }
    }
}
