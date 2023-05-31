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
    public class AppointmentReserveController : ControllerBase
    {
        private readonly IAppointmentReserveRepository _appointmentReserveRepository;
        private readonly IMapper _mapper;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IProjectRepository _projectRepository;

        public AppointmentReserveController(IAppointmentReserveRepository appointmentReserveRepository, IMapper mapper, IAppointmentRepository appointmentRepository, IProjectRepository projectRepository)
        {
            _appointmentReserveRepository = appointmentReserveRepository;
            _mapper = mapper;
            _appointmentRepository = appointmentRepository;
            _projectRepository = projectRepository;
        }

        [HttpGet]
        public IActionResult GetAppointmentReserves()
        {
            var appointmentReserves = _mapper.Map<List<AppointmentReserveModelDto>>(_appointmentReserveRepository.GetAppointmentReserves());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(appointmentReserves);
        }

        [HttpGet("{projectId}")]
        public IActionResult GetAppointmentReserve(string projectId)
        {
            if (!_appointmentReserveRepository.AppointmentReserveExists(projectId))
                return NotFound();

            //var appointmentReserve = _mapper.Map<AppointmentReserveModelDto>(_appointmentReserveRepository.GetAppointmentReserve(projectId));
            var appointmentReserve = _appointmentReserveRepository.GetAppointmentReserve(projectId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(appointmentReserve);
        }

        [HttpPost] //PM03
        [Authorize(Roles = "PM03")]
        public IActionResult CreateAppointmentReserve([FromBody] AppointmentReserveModelDto appointmentReserveCreate)
        {
            if (appointmentReserveCreate == null)
            {
                return NotFound();
            }

            if (!_appointmentRepository.AppointmentExists(appointmentReserveCreate.AppointmentId))
                return NotFound();

            if (!_projectRepository.ProjectExists(appointmentReserveCreate.ProjectId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var appointment = _appointmentRepository.GetAppointment(appointmentReserveCreate.AppointmentId);
            var project = appointment.AppointmentReserves.Select(x => x.ProjectId).ToList();
            if (project.Any(x => x == appointmentReserveCreate.ProjectId))
            {
                return Conflict("You already have a reservation.");
            }
            var reserveTime = appointment.AppointmentReserves.Select(x => x.ReserveTime).ToList();
            if (reserveTime.Any(x => x == appointmentReserveCreate.ReserveTime))
            {
                return StatusCode(422, "This time has been reserved.");
            }

            var appointmentReserveMap = _mapper.Map<AppointmentReserveModel>(appointmentReserveCreate);

            if (!_appointmentReserveRepository.CreateAppointmentReserve(appointmentReserveMap))
            {
                ModelState.AddModelError("", "Somthing went wrong while saving");
                return BadRequest(ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpPut]
        public IActionResult UpdateAppointmentReserve([FromBody] AppointmentReserveModelDto updateappointmentReserve)
        {
            if (updateappointmentReserve == null)
                return BadRequest(ModelState);

            if (!_appointmentReserveRepository.AppointmentReserveExists(updateappointmentReserve.ProjectId))
                return NotFound();

            if (!_appointmentRepository.AppointmentExists(updateappointmentReserve.AppointmentId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            var appointmentReserveMap = _mapper.Map<AppointmentReserveModel>(updateappointmentReserve);

            if (!_appointmentReserveRepository.DeleteAppointmentReserve(appointmentReserveMap.ProjectId))
            {
                ModelState.AddModelError("", "Somthing went wrong deleting advisee");
                return BadRequest(ModelState);
            }

            if (!_appointmentReserveRepository.CreateAppointmentReserve(appointmentReserveMap))
            {
                ModelState.AddModelError("", "Something went wrong updating");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{projectId}")]
        public IActionResult DeleteAppointmentReserve(string projectId)
        {

            if (!_projectRepository.ProjectExists(projectId))
                return NotFound();

            if (!_appointmentReserveRepository.AppointmentReserveExists(projectId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_appointmentReserveRepository.DeleteAppointmentReserve(projectId))
            {
                ModelState.AddModelError("", "Somthing went wrong deleting AppointmentReserve");
                return BadRequest(ModelState);
            }
            return NoContent();
        }
    }
}
