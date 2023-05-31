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
    public class AdviseeController : ControllerBase
    {
        private readonly IAdviseeRepository _adviseeRepository;
        private readonly IMapper _mapper;
        private readonly IProjectRepository _projectRepository;
        private readonly IMemberUserRepository _memberUserRepository;
        private readonly IRoleRepository _roleRepository;

        public AdviseeController(IAdviseeRepository adviseeRepository, IMapper mapper, IProjectRepository projectRepository, IMemberUserRepository memberUserRepository, IRoleRepository roleRepository)
        {
            _adviseeRepository = adviseeRepository;
            _mapper = mapper;
            _projectRepository = projectRepository;
            _memberUserRepository = memberUserRepository;
            _roleRepository = roleRepository;
        }

        [HttpGet]
        public IActionResult GetAdvisees()
        {
            var advisees = _mapper.Map<List<AdviseeModelDto>>(_adviseeRepository.GetAdvisees());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(advisees);
        }

        [HttpGet("{projectId}")]
        public IActionResult GetAdvisee(string projectId)
        {
            if (!_adviseeRepository.AdviseeExists(projectId))
                return NotFound();

            //var advisee = _mapper.Map<AdviseeModelDto>(_adviseeRepository.GetAdvisee(projectId));
            var advisee = _adviseeRepository.GetAdvisee(projectId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(advisee);
        }

        [HttpPost] //PM03
        [Authorize(Roles = "PM03")]
        public IActionResult CreateAdvisee([FromBody] AdviseeModelDto adviseeCreate)
        {
            if (adviseeCreate == null)
                return NotFound();

            if (!_projectRepository.ProjectExists(adviseeCreate.ProjectId))
                return NotFound();

            if (!_memberUserRepository.MemberUserExists(adviseeCreate.MemberUserId))
                return NotFound();
            var advisee = _adviseeRepository.GetAdvisee(adviseeCreate.ProjectId);
            var sameAdvisee = advisee.Select(x => x.MemberUserId).ToList();

            if (sameAdvisee.Any(x => x == adviseeCreate.MemberUserId))
                return BadRequest(ModelState);

            if (sameAdvisee.Count() >= 3)
                return BadRequest(ModelState);

            var newProject = _projectRepository.GetProject(adviseeCreate.ProjectId);
            var memberUser = _memberUserRepository.GetMemberUser(adviseeCreate.MemberUserId);
            var oldProject = _memberUserRepository.GetProjectByMemberUser(memberUser.MemberUserId, memberUser.RoleId);

            if (oldProject.Any(x => x.ProjectYear == newProject.ProjectYear))
            {
                return StatusCode(422, ModelState);
            }

            var oldprojectId = oldProject.Select(x => x.ProjectId).ToList();
            if (oldprojectId.Count > 0)
            {
                if (!_adviseeRepository.DeleteAdvisee(oldprojectId[0]))
                {
                    ModelState.AddModelError("", "Somthing went wrong deleting advisee");
                    return BadRequest(ModelState);
                }

            }

            if (_roleRepository.GetRoleByMemberUser(adviseeCreate.MemberUserId).RoleId != "PM03")
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var adviseeMap = _mapper.Map<AdviseeModel>(adviseeCreate);

            if (!_adviseeRepository.CreateAdvisee(adviseeMap))
            {
                ModelState.AddModelError("", "Somthing went wrong while saving");
                return BadRequest(ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpPut]
        public IActionResult UpdateAdvisee([FromBody] AdviseeModelDto updateAdvisee)
        {
            if (updateAdvisee == null)
                return BadRequest(ModelState);

            if (!_adviseeRepository.AdviseeExists(updateAdvisee.ProjectId))
                return NotFound();

            if (!_memberUserRepository.MemberUserExists(updateAdvisee.MemberUserId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            var adviseeMap = _mapper.Map<AdviseeModel>(updateAdvisee);

            if (!_adviseeRepository.DeleteAdvisee(adviseeMap.ProjectId))
            {
                ModelState.AddModelError("", "Somthing went wrong deleting advisee");
                return BadRequest(ModelState);
            }

            var advisee = _adviseeRepository.GetAdvisee(updateAdvisee.ProjectId);
            var sameAdvisee = advisee.Select(x => x.MemberUserId).ToList();

            if (sameAdvisee.Any(x => x == updateAdvisee.MemberUserId))
                return BadRequest(ModelState);

            if (sameAdvisee.Count() >= 3)
                return BadRequest(ModelState);

            if (!_adviseeRepository.CreateAdvisee(adviseeMap))
            {
                ModelState.AddModelError("", "Something went wrong updating");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{projectId}")]
        public IActionResult DeleteAdvisee(string projectId)
        {
            if (!_projectRepository.ProjectExists(projectId))
                return NotFound();

            if (!_adviseeRepository.AdviseeExists(projectId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_adviseeRepository.DeleteAdvisee(projectId))
            {
                ModelState.AddModelError("", "Somthing went wrong deleting advisee");
                return BadRequest(ModelState);
            }
            return NoContent();
        }
    }
}
