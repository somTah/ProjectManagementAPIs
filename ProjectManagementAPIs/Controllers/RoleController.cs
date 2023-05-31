using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementAPIs.Dto;
using ProjectManagementAPIs.Interfaces;

namespace ProjectManagementAPIs.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleRepository _rolesRepository;
        private readonly IMapper _mapper;

        public RoleController(IRoleRepository roleRepository, IMapper mapper)
        {
            _rolesRepository = roleRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetRoles()
        {
            var roles = _mapper.Map<List<RoleModelDto>>(_rolesRepository.GetRoles());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(roles);
        }

        [HttpGet("{roleId}")]
        public IActionResult GetRole(string roleId)
        {
            if (!_rolesRepository.RoleExists(roleId))
                return NotFound();

            var role = _mapper.Map<RoleModelDto>(_rolesRepository.GetRole(roleId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(role);
        }

        [HttpGet("/memberUser/{memberUserId}")]
        public IActionResult GetRoleByMemberUser(string memberUserId)
        {
            var role = _mapper.Map<RoleModelDto>(_rolesRepository.GetRoleByMemberUser(memberUserId));

            if (!ModelState.IsValid)
                return BadRequest();

            return Ok(role);
        }
    }
}
