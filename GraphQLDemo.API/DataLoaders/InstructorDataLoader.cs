using GraphQLDemo.API.DTOs;
using GraphQLDemo.API.Services.Instructors;
using GreenDonut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GraphQLDemo.API.DataLoaders
{
    public class InstructorDataLoader : BatchDataLoader<Guid, InstructorDto>
    {
        private readonly InstructorsRepository _instructorsRepository;

        public InstructorDataLoader(
            InstructorsRepository instructorsRepository,
            IBatchScheduler batchScheduler,
            DataLoaderOptions options = null)
            : base(batchScheduler, options)
        {
            _instructorsRepository = instructorsRepository;
        }

        protected override async Task<IReadOnlyDictionary<Guid, InstructorDto>> LoadBatchAsync(IReadOnlyList<Guid> keys, CancellationToken cancellationToken)
        {
            IEnumerable<InstructorDto> instructors = await _instructorsRepository.GetManyByIds(keys);

            return instructors.ToDictionary(i => i.Id);
        }
    }
}
