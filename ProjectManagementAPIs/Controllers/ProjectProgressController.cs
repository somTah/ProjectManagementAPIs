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
    public class ProjectProgressController : ControllerBase
    {
        private readonly IProjectProgressRepository _projectProgressRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IMapper _mapper;

        public ProjectProgressController(IProjectProgressRepository projectProgressRepository, IProjectRepository projectRepository, IMapper mapper)
        {
            _projectProgressRepository = projectProgressRepository;
            _projectRepository = projectRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetProjectProgresses()
        {
            var projectProgresses = _mapper.Map<List<ProjectProgressModelDto>>(_projectProgressRepository.GetProjectProgresses());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(projectProgresses);
        }

        [HttpGet("{projectProgressId}")] //PM03, PM01, PM02
        [Authorize(Roles = "PM01, PM02, PM03")]
        public IActionResult GetProjectProgress(string projectProgressId)
        {
            //if (!_projectProgressRepository.ProjectProgressExists(projectProgressId))
            //    return NotFound();

            //var projectProgress = _mapper.Map<ProjectProgressModelDto>(_projectProgressRepository.GetProjectProgress(projectProgressId));
            var projectProgress = _projectProgressRepository.GetProjectProgress(projectProgressId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(projectProgress);
        }

        [HttpGet("project/{projectId}")] //PM03, PM01, PM02
        [Authorize(Roles = "PM01, PM02, PM03")]
        public IActionResult GetProjectProgressByProject(string projectId)
        {
            if (!_projectRepository.ProjectExists(projectId))
                return NotFound();

            //var projectProgresses = _mapper.Map<List<ProjectProgressModelDto>>(_projectProgressRepository.GetProjectProgressesOfProject(projectId));
            var projectProgresses = _projectProgressRepository.GetProjectProgressByProject(projectId);

            if (!ModelState.IsValid)
                return BadRequest();

            return Ok(projectProgresses);
        }

        [HttpPost] //PM03
        [Authorize(Roles = "PM03")]
        public IActionResult CreateProjectProgress([FromQuery] string projectId, [FromBody] ProjectProgressModelDto projectProgressCreate)
        {
            if (projectProgressCreate == null)
                return BadRequest(ModelState);

            if (!_projectRepository.ProjectExists(projectId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var projectProgress = _projectProgressRepository.GetProjectProgressByProject(projectId);
            projectProgressCreate.NumberProgress = projectProgress.Count + 1;
            var projectProgressMap = _mapper.Map<ProjectProgressModel>(projectProgressCreate);

            projectProgressMap.Project = _projectRepository.GetProject(projectId);

            if (!_projectProgressRepository.CreateProjectProgress(projectProgressMap))
            {
                ModelState.AddModelError("", "Somthing went wrong while savin");
                return BadRequest(ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpPut("{projectProgressId}")] //PM01, PM02
        [Authorize(Roles = "PM01, PM02")]
        public IActionResult UpdateProjectProgress(string projectProgressId, [FromQuery] string projectId, [FromBody] ProjectProgressModelDto updateProjectProgress)
        {
            if (updateProjectProgress == null)
                return BadRequest(ModelState);

            if (projectProgressId != updateProjectProgress.ProjectProgressId)
                return BadRequest(ModelState);

            if (!_projectProgressRepository.ProjectProgressExists(projectProgressId))
                return NotFound();

            if (!_projectRepository.ProjectExists(projectId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            var projectProgressMap = _mapper.Map<ProjectProgressModel>(updateProjectProgress);

            projectProgressMap.Project = _projectRepository.GetProject(projectId);

            if (!_projectProgressRepository.UpdateProjectProgress(projectId, projectProgressMap))
            {
                ModelState.AddModelError("", "Something went wrong updating projectProgress");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        //[HttpDelete("{projectProgressId}")]
        //public IActionResult DeleteProjectProgress(string projectProgressId)
        //{
        //    if (!_projectProgressRepository.ProjectProgressExists(projectProgressId))
        //        return NotFound();

        //    var projectProgressToDelete = _projectProgressRepository.GetProjectProgress(projectProgressId);

        //    if (!ModelState.IsValid) return BadRequest();

        //    if (!_projectProgressRepository.DeleteProjectProgress(projectProgressToDelete))
        //    {
        //        ModelState.AddModelError("", "Something went wrong deleting projectProgress");
        //    }

        //    return NoContent();
        //}

        [HttpDelete("{projectId}")]
        public IActionResult DeleteProjectProgressByProject(string projectId)
        {
            if (!_projectRepository.ProjectExists(projectId))
                return NotFound();

            if (!ModelState.IsValid) return BadRequest();

            if (!_projectProgressRepository.DeleteProjectProgressByProject(projectId))
            {
                ModelState.AddModelError("", "Somthing went wrong deleting projectProgress");
                return BadRequest(ModelState);
            }

            return NoContent();
        }

    }
}
