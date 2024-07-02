using AutoMapper;
using Microsoft.AspNetCore.Mvc;

using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;
using NZWalks.API.Models.Domain;
using NZWalks.API.CustomActionFilters;

namespace NZWalks.API.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class WalksController : Controller
    {
        private readonly IMapper mapper;
        private readonly IWalkRepository walkRepository;

        public WalksController(IMapper mapper, IWalkRepository walkRepository)
        {
            this.mapper = mapper;
            this.walkRepository = walkRepository;
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] AddWalkRequestDto addWalkRequestDto)
        {
            var walkDomain = mapper.Map<Walk>(addWalkRequestDto);
            await walkRepository.CreateAsync(walkDomain);

            var walkDto = mapper.Map<WalkDto>(walkDomain);
            return CreatedAtAction(nameof(GetById), new { id = walkDto.Id }, walkDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? filterOn,
            [FromQuery] string? filterQuery,
            [FromQuery] string? sortBy,
            [FromQuery] bool isAscending = true,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 100)
        {
            var walksDomain = await walkRepository.GetAllAsync(
                filterOn,
                filterQuery,
                sortBy,
                isAscending,
                pageNumber,
                pageSize);

            var walksDto = mapper.Map<List<WalkDto>>(walksDomain);
            return Ok(walksDto);
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var walkDomain = await walkRepository.GetByIdAsync(id);

            if (walkDomain is null)
            {
                return NotFound();
            }

            var walkDto = mapper.Map<WalkDto>(walkDomain);
            return Ok(walkDto);
        }

        [HttpPut]
        [Route("{id:Guid}")]
        [ValidateModel]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateWalkRequestDto updateWalkRequestDto)
        {
            var walkDomain = mapper.Map<Walk>(updateWalkRequestDto);

            walkDomain = await walkRepository.UpdateAsync(id, walkDomain);

            if (walkDomain is null)
            {
                return NotFound();
            }

            var walkDto = mapper.Map<WalkDto>(walkDomain);
            return Ok(walkDto);
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var deletedWalkDomain = await walkRepository.DeleteAsync(id);

            if (deletedWalkDomain is null)
            {
                return NotFound();
            }

            var walkDto = mapper.Map<WalkDto>(deletedWalkDomain);
            return Ok(walkDto);
        }
    }
}
