using AppAny.HotChocolate.FluentValidation;
using GraphQLDemo.API.DTOs;
using GraphQLDemo.API.Middlewares.UseUser;
using GraphQLDemo.API.Models;
using GraphQLDemo.API.Schema.Subscriptions;
using GraphQLDemo.API.Services.Courses;
using GraphQLDemo.API.Validators;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using System;
using System.Threading.Tasks;

namespace GraphQLDemo.API.Schema.Mutations
{
    [ExtendObjectType(typeof(Mutation))]
    public class CourseMutation
    {
        private readonly CoursesRepository _coursesRepository;

        public CourseMutation(CoursesRepository coursesRepository)
        {
            _coursesRepository = coursesRepository;
        }

        [Authorize]
        [UseUser]
        public async Task<CourseResult> CreateCourse(
            [UseFluentValidation, UseValidator<CourseTypeInputValidator>] CourseTypeInput courseInput,
            [Service] ITopicEventSender topicEventSender,
            [User] User user)
        {
            string userId = user.Id;

            CourseDto courseDto = new CourseDto()
            {
                Name = courseInput.Name,
                Subject = courseInput.Subject,
                InstructorId = courseInput.InstructorId,
                CreatorId = userId
            };

            courseDto = await _coursesRepository.Create(courseDto);

            CourseResult course = new CourseResult()
            {
                Id = courseDto.Id,
                Name = courseDto.Name,
                Subject = courseDto.Subject,
                InstructorId = courseDto.InstructorId
            };

            await topicEventSender.SendAsync(nameof(Subscription.CourseCreated), course);

            return course;
        }

        [Authorize]
        [UseUser]
        public async Task<CourseResult> UpdateCourse(Guid id,
            [UseFluentValidation, UseValidator<CourseTypeInputValidator>] CourseTypeInput courseInput,
            [Service] ITopicEventSender topicEventSender,
            [User] User user)
        {
            string userId = user.Id;

            CourseDto courseDto = await _coursesRepository.GetById(id);

            if(courseDto == null)
            {
                throw new GraphQLException(new Error("Course not found.", "COURSE_NOT_FOUND"));
            }

            if (courseDto.CreatorId != userId)
            {
                throw new GraphQLException(new Error("You do not have permission to update this course.", "INVALID_PERMISSION"));
            }

            courseDto.Name = courseInput.Name;
            courseDto.Subject = courseInput.Subject;
            courseDto.InstructorId = courseInput.InstructorId;

            courseDto = await _coursesRepository.Update(courseDto);

            CourseResult course = new CourseResult()
            {
                Id = courseDto.Id,
                Name = courseDto.Name,
                Subject = courseDto.Subject,
                InstructorId = courseDto.InstructorId
            };

            string updateCourseTopic = $"{course.Id}_{nameof(Subscription.CourseUpdated)}";
            await topicEventSender.SendAsync(updateCourseTopic, course);

            return course;
        }

        [Authorize(Policy = "IsAdmin")]
        public async Task<bool> DeleteCourse(Guid id)
        {
            try
            {
                return await _coursesRepository.Delete(id);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
