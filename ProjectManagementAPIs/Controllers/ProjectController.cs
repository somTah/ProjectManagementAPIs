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
    public class ProjectController : ControllerBase
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IMapper _mapper;
        private readonly IAppointmentReserveRepository _appointmentReserveRepository;
        private readonly IAdviseeRepository _adviseeRepository;
        private readonly IAdviserRepository _adviserRepository;
        private readonly IProjectProgressRepository _projectProgressRepository;

        public ProjectController(IProjectRepository projectRepository, IMapper mapper, IAppointmentReserveRepository appointmentReserveRepository, IAdviseeRepository adviseeRepository, IAdviserRepository adviserRepository, IProjectProgressRepository projectProgressRepository)
        {
            _projectRepository = projectRepository;
            _mapper = mapper;
            _appointmentReserveRepository = appointmentReserveRepository;
            _adviseeRepository = adviseeRepository;
            _adviserRepository = adviserRepository;
            _projectProgressRepository = projectProgressRepository;
        }

        [HttpGet]
        public IActionResult GetProjects()
        {
            //var projects = _mapper.Map<List<ProjectModelDto>>(_projectRepository.GetProjects());
            var projects = _projectRepository.GetProjects();


            if (!ModelState.IsValid) return BadRequest(ModelState);

            return Ok(projects);
        }

        [HttpGet("{projectId}")]
        public IActionResult GetProject(string projectId)
        {
            if (!_projectRepository.ProjectExists(projectId))
                return NotFound();

            //var project = _mapper.Map<ProjectModelDto>(_projectRepository.GetProject(projectId));
            var project = _projectRepository.GetProject(projectId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(project);
        }

        [HttpGet("AdviseeId/{projectId}")]
        public IActionResult GetAdviseeByProject(string projectId)
        {
            if (!_projectRepository.ProjectExists(projectId))
                return NotFound();

            var advisee = _mapper.Map<List<MemberUserModelDto>>(_projectRepository.GetAdviseeByProject(projectId));

            if (!ModelState.IsValid)
                return BadRequest();

            return Ok(advisee);
        }

        [HttpGet("adviserId/{projectId}")]
        public IActionResult GetAdviserByProject(string projectId)
        {
            if (!_projectRepository.ProjectExists(projectId))
                return NotFound();

            var adviser = _mapper.Map<List<MemberUserModelDto>>(_projectRepository.GetAdviserByProject(projectId));

            if (!ModelState.IsValid)
                return BadRequest();

            return Ok(adviser);
        }

        [HttpPost]
        [Authorize(Roles = "PM01, PM02")] //PM01, PM02
        public IActionResult CreateProject([FromQuery] string adviserId, [FromBody] ProjectModelDto projectCreate)
        {
            if (projectCreate == null)
                return BadRequest(ModelState);

            var projects = _projectRepository.GetProjects().Where(c => c.ProjectName.Trim().ToUpper() == projectCreate.ProjectName.TrimEnd().ToUpper()).FirstOrDefault();

            if (projects != null)
            {
                ModelState.AddModelError("", "Project already exists");
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var projectMap = _mapper.Map<ProjectModel>(projectCreate);

            if (!_projectRepository.CreateProject(adviserId, projectMap))
            {
                ModelState.AddModelError("", "Somthing went wrong while saving");
                return BadRequest(ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpPut("{projectId}")] //PM00, PM01, PM02
        [Authorize(Roles = "PM00, PM01, PM02")]
        public IActionResult UpdateProject(string projectId, [FromBody] ProjectModelDto updateProject)
        {
            if (updateProject == null)
                return BadRequest(ModelState);

            if (projectId != updateProject.ProjectId)
                return BadRequest(ModelState);

            if (!_projectRepository.ProjectExists(projectId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            if (updateProject.ProjectYear != (DateTime.Now.Year + 543).ToString()) return Conflict("Must be current year");

            var projectMap = _mapper.Map<ProjectModel>(updateProject);

            if (!_projectRepository.UpdateProject(projectMap))
            {
                ModelState.AddModelError("", "Something went wrong updating");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{projectId}")] //PM00, PM01, PM02
        [Authorize(Roles = "PM00, PM01, PM02")]
        public IActionResult DeleteProject(string projectId)
        {
            if (!_projectRepository.ProjectExists(projectId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_appointmentReserveRepository.AppointmentReserveExists(projectId))
            {
                if (!_appointmentReserveRepository.DeleteAppointmentReserve(projectId))
                {
                    ModelState.AddModelError("", "Somthing went wrong deleting appointmentReserve");
                    return BadRequest(ModelState);
                }

            }

            if (_adviseeRepository.AdviseeExists(projectId))
            {
                if (!_adviseeRepository.DeleteAdvisee(projectId))
                {
                    ModelState.AddModelError("", "Somthing went wrong deleting advisee");
                    return BadRequest(ModelState);
                }

            }

            if (_adviserRepository.AdviserExists(projectId))
            {
                if (!_adviserRepository.DeleteAdviser(projectId))
                {
                    ModelState.AddModelError("", "Somthing went wrong deleting adviser");
                    return BadRequest(ModelState);
                }

            }

            var getProjectProgress = _projectProgressRepository.GetProjectProgressByProject(projectId);

            if (getProjectProgress.Count > 0)
            {
                if (!_projectProgressRepository.DeleteProjectProgressByProject(projectId))
                {
                    ModelState.AddModelError("", "Somthing went wrong deleting projectProgress");
                    return BadRequest(ModelState);
                }

            }

            var projectToDelete = _projectRepository.GetProject(projectId);

            if (!_projectRepository.DeleteProject(projectToDelete))
            {
                ModelState.AddModelError("", "Somthing went wrong deleting advisee");
                return BadRequest(ModelState);
            }
            return NoContent();
        }

    }
}
