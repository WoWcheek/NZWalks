using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;
using NZWalks.API.Models.Domain;
using NZWalks.API.CustomActionFilters;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IRegionRepository regionRepository;

        public RegionsController(IMapper mapper, IRegionRepository regionRepository)
        {
            this.mapper = mapper;
            this.regionRepository = regionRepository;
        }

        [HttpGet]
        [Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetAll()
        {
            var regionsDomain = await regionRepository.GetAllAsync();

            var regionsDto = mapper.Map<List<RegionDto>>(regionsDomain);
            return Ok(regionsDto);
        }

        [HttpGet]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var regionDomain = await regionRepository.GetByIdAsync(id);
            if (regionDomain is null)
            {
                return NotFound();
            }

            var regionDto = mapper.Map<RegionDto>(regionDomain);
            return Ok(regionDto);
        }

        [HttpPost]
        [ValidateModel]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create([FromBody] AddRegionRequestDto addRegionRequestDto)
        {
            var regionDomain = mapper.Map<Region>(addRegionRequestDto);
            regionDomain = await regionRepository.CreateAsync(regionDomain);

            var regionDto = mapper.Map<RegionDto>(regionDomain);
            return CreatedAtAction(nameof(GetById), new { id = regionDto.Id }, regionDto);
        }

        [HttpPut]
        [ValidateModel]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto)
        {
            var regionDomain = mapper.Map<Region>(updateRegionRequestDto);
            regionDomain = await regionRepository.UpdateAsync(id, regionDomain);

            if (regionDomain is null)
            {
                return NotFound();
            }

            var regionDto = mapper.Map<RegionDto>(regionDomain);
            return Ok(regionDto);
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var regionDomain = await regionRepository.DeleteAsync(id);

            if (regionDomain is null)
            {
                return NotFound();
            }

            var regionDto = mapper.Map<RegionDto>(regionDomain);
            return Ok(regionDto);
        }
    }
}
