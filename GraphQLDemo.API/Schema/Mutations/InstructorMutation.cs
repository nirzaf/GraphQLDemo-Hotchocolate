using AppAny.HotChocolate.FluentValidation;
using GraphQLDemo.API.DTOs;
using GraphQLDemo.API.Schema.Subscriptions;
using GraphQLDemo.API.Services;
using GraphQLDemo.API.Validators;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Data;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using System;
using System.Threading.Tasks;

namespace GraphQLDemo.API.Schema.Mutations
{
    [ExtendObjectType(typeof(Mutation))]
    public class InstructorMutation
    {
        [Authorize]
        [UseDbContext(typeof(SchoolDbContext))]
        public async Task<InstructorResult> CreateInstructor(
            [UseFluentValidation, UseValidator<InstructorTypeInputValidator>] InstructorTypeInput instructorInput,
            [ScopedService] SchoolDbContext context,
            [Service] ITopicEventSender topicEventSender)
        {
            InstructorDto instructorDto = new InstructorDto()
            {
                FirstName = instructorInput.FirstName,
                LastName = instructorInput.LastName,
                Salary = instructorInput.Salary,
            };

            context.Add(instructorDto);
            await context.SaveChangesAsync();

            InstructorResult instructorResult = new InstructorResult()
            {
                Id = instructorDto.Id,
                FirstName = instructorDto.FirstName,
                LastName = instructorDto.LastName,
                Salary = instructorDto.Salary,
            };

            await topicEventSender.SendAsync(nameof(Subscription.InstructorCreated), instructorResult);

            return instructorResult;
        }

        [Authorize]
        [UseDbContext(typeof(SchoolDbContext))]
        public async Task<InstructorResult> UpdateInstructor(
            Guid id,
            [UseFluentValidation, UseValidator<InstructorTypeInputValidator>] InstructorTypeInput instructorInput,
            [ScopedService] SchoolDbContext context)
        {
            InstructorDto instructorDto = await context.Instructors.FindAsync(id);

            if(instructorDto == null)
            {
                throw new GraphQLException(new Error("Instructor not found.", "INSTRUCTOR_NOT_FOUND"));
            }

            instructorDto.FirstName = instructorInput.FirstName;
            instructorDto.LastName = instructorInput.LastName;
            instructorDto.Salary = instructorInput.Salary;

            context.Update(instructorDto);
            await context.SaveChangesAsync();

            InstructorResult instructorResult = new InstructorResult()
            {
                Id = instructorDto.Id,
                FirstName = instructorDto.FirstName,
                LastName = instructorDto.LastName,
                Salary = instructorDto.Salary,
            };

            return instructorResult;
        }

        [Authorize(Policy = "IsAdmin")]
        [UseDbContext(typeof(SchoolDbContext))]
        public async Task<bool> DeleteInstructor(Guid id, [ScopedService] SchoolDbContext context)
        {
            InstructorDto instructorDto = new InstructorDto()
            {
                Id = id
            };

            context.Remove(instructorDto);

            try
            {
                await context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
