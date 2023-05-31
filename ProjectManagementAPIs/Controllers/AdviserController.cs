using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementAPIs.Dto;
using ProjectManagementAPIs.Interfaces;
using ProjectManagementAPIs.Models;

namespace ProjectManagementAPIs.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AdviserController : ControllerBase
    {
        private readonly IAdviserRepository _adviserRepository;
        private readonly IMapper _mapper;
        private readonly IProjectRepository _projectRepository;
        private readonly IMemberUserRepository _memberUserRepository;
        private readonly IRoleRepository _roleRepository;

        public AdviserController(IAdviserRepository adviserRepository, IMapper mapper, IProjectRepository projectRepository, IMemberUserRepository memberUserRepository, IRoleRepository roleRepository)
        {
            _adviserRepository = adviserRepository;
            _mapper = mapper;
            _projectRepository = projectRepository;
            _memberUserRepository = memberUserRepository;
            _roleRepository = roleRepository;
        }

        [HttpGet]
        public IActionResult GetAdvisers()
        {
            var advisers = _mapper.Map<List<AdviserModelDto>>(_adviserRepository.GetAdvisers());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(advisers);
        }

        [HttpGet("{projectId}")]
        public IActionResult GetAdviser(string projectId)
        {
            if (!_adviserRepository.AdviserExists(projectId))
                return NotFound();

            //var adviser = _mapper.Map<AdviserModelDto>(_adviserRepository.GetAdviser(projectId));
            var adviser = _adviserRepository.GetAdviser(projectId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(adviser);
        }

        [HttpPost]
        public IActionResult CreateAdviser([FromBody] AdviserModelDto adviserCreate)
        {
            if (adviserCreate == null)
                return NotFound();

            if (!_projectRepository.ProjectExists(adviserCreate.ProjectId))
                return NotFound();

            if (!_memberUserRepository.MemberUserExists(adviserCreate.MemberUserId))
                return NotFound();

            var adviser = _adviserRepository.GetAdviser(adviserCreate.ProjectId);
            var sameAdviser = adviser.Select(x => x.MemberUserId).ToList();

            if (sameAdviser.Any(x => x == adviserCreate.MemberUserId))
                return BadRequest(ModelState);

            if (sameAdviser.Count() >= 3)
                return BadRequest(ModelState);

            if (_roleRepository.GetRoleByMemberUser(adviserCreate.MemberUserId).RoleId != "PM02")
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var advisertMap = _mapper.Map<AdviserModel>(adviserCreate);

            if (!_adviserRepository.CreateAdviser(advisertMap))
            {
                ModelState.AddModelError("", "Somthing went wrong while saving");
                return BadRequest(ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpPut]
        public IActionResult UpdateAdviser([FromBody] AdviserModelDto updateAdviser)
        {
            if (updateAdviser == null)
                return BadRequest(ModelState);

            if (!_adviserRepository.AdviserExists(updateAdviser.ProjectId))
                return NotFound();

            if (!_memberUserRepository.MemberUserExists(updateAdviser.MemberUserId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            var adviserMap = _mapper.Map<AdviserModel>(updateAdviser);

            if (!_adviserRepository.DeleteAdviser(adviserMap.ProjectId))
            {
                ModelState.AddModelError("", "Somthing went wrong deleting advisee");
                return BadRequest(ModelState);
            }

            var adviser = _adviserRepository.GetAdviser(updateAdviser.ProjectId);
            var sameAdviser = adviser.Select(x => x.MemberUserId).ToList();

            if (sameAdviser.Any(x => x == updateAdviser.MemberUserId))
                return BadRequest(ModelState);

            if (sameAdviser.Count() >= 3)
                return BadRequest(ModelState);

            if (!_adviserRepository.CreateAdviser(adviserMap))
            {
                ModelState.AddModelError("", "Something went wrong updating");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{projectId}")]
        public IActionResult DeleteAdviser(string projectId)
        {
            if (!_projectRepository.ProjectExists(projectId))
                return NotFound();

            if (!_adviserRepository.AdviserExists(projectId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_adviserRepository.DeleteAdviser(projectId))
            {
                ModelState.AddModelError("", "Somthing went wrong deleting adviser");
                return BadRequest(ModelState);
            }
            return NoContent();
        }

    }
}
