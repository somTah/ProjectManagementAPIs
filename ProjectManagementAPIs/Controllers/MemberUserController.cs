using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementAPIs.Dto;
using ProjectManagementAPIs.Interfaces;
using ProjectManagementAPIs.Models;
using System.Text.RegularExpressions;

namespace ProjectManagementAPIs.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MemberUserController : ControllerBase
    {
        private readonly IMemberUserRepository _memberUserRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;

        public MemberUserController(IMemberUserRepository memberUserRepository, IRoleRepository roleRepository, IMapper mapper, IPasswordHasher passwordHasher)
        {
            _memberUserRepository = memberUserRepository;
            _roleRepository = roleRepository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }

        [HttpGet] //PM00
        [Authorize(Roles = "PM00")]
        public IActionResult GetMemberUsers()
        {
            //var memberUsers = _mapper.Map<List<MemberUserModelDto>>(_memberUserRepository.GetMemberUsers());
            var memberUsers = _memberUserRepository.GetMemberUsers();
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(memberUsers);
        }

        [HttpGet("{memberUserId}")]
        public IActionResult GetMemberUser(string memberUserId)
        {
            if (!_memberUserRepository.MemberUserExists(memberUserId))
                return NotFound();

            //var memberUser = _mapper.Map<MemberUserModelDto>(_memberUserRepository.GetMemberUser(memberUserId));
            var memberUser = _memberUserRepository.GetMemberUser(memberUserId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(memberUser);
        }

        [HttpGet("memberUserEmail/{memberUserEmail}")]
        public IActionResult GetMemberUserByEmail(string memberUserEmail)
        {

            var memberUser = _memberUserRepository.GetMemberUserByEmail(memberUserEmail);

            if (memberUser == null)
            {
                return NotFound(); // Return 404 Not Found if the member user is not found
            }

            return Ok(memberUser);
        }

        [HttpGet("firstname/{firstname}")]
        public IActionResult GetMemberUserByFirstname(string firstname)
        {

            var memberUser = _memberUserRepository.GetMemberUserByFirstname(firstname);

            if (memberUser == null)
            {
                return NotFound(); // Return 404 Not Found if the member user is not found
            }

            return Ok(memberUser);
        }

        [HttpGet("lastname/{lastname}")]
        public IActionResult GetMemberUserByLastname(string lastname)
        {

            var memberUser = _memberUserRepository.GetMemberUserByLastname(lastname);

            if (memberUser == null)
            {
                return NotFound(); // Return 404 Not Found if the member user is not found
            }

            return Ok(memberUser);
        }

        [HttpGet("project/{memberUserId}")] //PM01, PM02, PM03
        [Authorize(Roles = "PM01, PM02, PM03")]
        public IActionResult GetProjectByMemberUser(string memberUserId)
        {
            if (!_memberUserRepository.MemberUserExists(memberUserId))
                return NotFound();

            var getMemberUser = _mapper.Map<MemberUserModelDto>(_memberUserRepository.GetMemberUser(memberUserId));

            var memberUserMap = _mapper.Map<MemberUserModel>(getMemberUser);

            memberUserMap.Role = _roleRepository.GetRoleByMemberUser(memberUserId);


            var memberUser = _memberUserRepository.GetProjectByMemberUser(memberUserId, memberUserMap.Role.RoleId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(memberUser);
        }

        [HttpPost] //PM00
        [Authorize(Roles = "PM00")]
        public IActionResult CreateMemberUser([FromBody] IEnumerable<MemberUserModelDto> memberUsersCreate)
        {
            if (!memberUsersCreate.Any()) return NotFound();

            if (!ModelState.IsValid) return BadRequest(ModelState);

            foreach (var memberUserCreate in memberUsersCreate)
            {
                var existingMemberUser = _memberUserRepository.GetMemberUsers().FirstOrDefault(m => m.MemberUserEmail.Trim().ToUpper() == memberUserCreate.MemberUserEmail.Trim().ToUpper());

                if (existingMemberUser != null) return Conflict("Email already exists");

                var memberUserMap = _mapper.Map<MemberUserModel>(memberUserCreate);

                string strRegexStudent = @"(^\d+\d{11}@dpu.ac.th$)";
                string strRegexAdviser = @"(^\w+@dpu.ac.th$)";
                Regex regexStudent = new Regex(strRegexStudent);
                Regex regexAdviser = new Regex(strRegexAdviser);

                if (regexStudent.IsMatch(memberUserMap.MemberUserEmail))
                {
                    memberUserMap.Role = _roleRepository.GetRole("PM03");
                }
                else if (regexAdviser.IsMatch(memberUserMap.MemberUserEmail))
                {
                    memberUserMap.Role = _roleRepository.GetRole("PM02");
                }
                else
                {
                    // Skip saving the email and move on to the next one
                    continue;
                }

                // Hash the password
                memberUserMap.passwordHash = _passwordHasher.Hash(memberUserCreate.MemberUserPassword);

                // Split the password hash and salt and store them separately
                string[] parts = memberUserMap.passwordHash.Split(':');
                if (parts.Length == 2)
                {
                    memberUserMap.passwordSalt = parts[0];
                    memberUserMap.passwordHash = parts[1];
                }
                else
                {
                    // Handle the case where the password hash and salt are not in the expected format
                    // You can return an error response or take appropriate action
                    return BadRequest("Invalid password format");
                }


                if (!_memberUserRepository.CreateMemberUser(memberUserMap))
                {
                    ModelState.AddModelError("", "Something went wrong while saving");
                    return StatusCode(500, ModelState);
                }
            }

            return Ok("Successfully created");
        }

        [HttpPatch("{memberUserId}")] //all role
        public IActionResult UpdateMemberUser(string memberUserId, string currentPassword, string newPassword)
        {
            if (!_memberUserRepository.MemberUserExists(memberUserId))
                return NotFound();

            var memberUser = _memberUserRepository.GetMemberUser(memberUserId);

            if (memberUser == null || !_passwordHasher.Verify((memberUser?.passwordSalt + ':' + memberUser?.passwordHash), currentPassword))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid) return BadRequest();

            if (newPassword != null)
            {
                memberUser.passwordHash = _passwordHasher.Hash(newPassword);
                string[] parts = memberUser.passwordHash.Split(':');
                if (parts.Length == 2)
                {
                    memberUser.passwordSalt = parts[0];
                    memberUser.passwordHash = parts[1];
                }
                else
                {
                    // Handle the case where the password hash and salt are not in the expected format
                    // You can return an error response or take appropriate action
                    return BadRequest("Invalid password format");
                }
            }

            if (!_memberUserRepository.UpdateMemberUser(memberUser))
            {
                ModelState.AddModelError("", "Something went wrong updating appointment");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpPut("{memberUserId}")] //PM00
        [Authorize(Roles = "PM00")]
        public IActionResult UpdateRoleMemberUser(string memberUserId, [FromQuery] string roleId)
        {

            if (!_roleRepository.RoleExists(roleId))
                return NotFound();

            if (!_memberUserRepository.MemberUserExists(memberUserId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            var memberUser = _memberUserRepository.GetMemberUser(memberUserId);
            if (memberUser.MemberUserId == memberUserId && memberUser.RoleId == roleId)
            {
                return BadRequest("This is your current role!");
            }

            var memberUserMap = _mapper.Map<MemberUserModel>(memberUser);

            memberUserMap.Role = _roleRepository.GetRole(roleId);

            if (!_memberUserRepository.UpdateRoleMemberUser(roleId, memberUserMap))
            {
                ModelState.AddModelError("", "Something went wrong updating memberUser");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{memberUserId}")]
        public IActionResult DeleteMemberUser(string memberUserId)
        {
            if (!_memberUserRepository.MemberUserExists(memberUserId))
                return NotFound();

            var memberUserToDelete = _memberUserRepository.GetMemberUser(memberUserId);

            if (!ModelState.IsValid) return BadRequest();

            if (!_memberUserRepository.DeleteMemberUser(memberUserToDelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting memberUser");
            }

            return NoContent();
        }

    }
}
