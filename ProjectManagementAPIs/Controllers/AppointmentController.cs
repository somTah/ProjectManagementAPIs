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
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IMapper _mapper;
        private readonly IAppointmentReserveRepository _appointmentReserveRepository;

        public AppointmentController(IAppointmentRepository appointmentRepository, IMapper mapper, IAppointmentReserveRepository appointmentReserveRepository)
        {
            _appointmentRepository = appointmentRepository;
            _mapper = mapper;
            _appointmentReserveRepository = appointmentReserveRepository;
        }

        [HttpGet] //PM03,PM01,PM02
        [Authorize(Roles = "PM01, PM02, PM03")]
        public IActionResult GetAppointments()
        {
            //var appointments = _mapper.Map<List<AppointmentModelDto>>(_appointmentRepository.GetAppointments());
            var appointments = _appointmentRepository.GetAppointments();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(appointments);
        }

        [HttpGet("{appointmentId}")] //PM01, PM02
        [Authorize(Roles = "PM01, PM02")]
        public IActionResult GetAppointment(string appointmentId)
        {
            if (!_appointmentRepository.AppointmentExists(appointmentId))
                return NotFound();

            //var appointment = _mapper.Map<AppointmentModelDto>(_appointmentRepository.GetAppointment(appointmentId));
            var appointment = _appointmentRepository.GetAppointment(appointmentId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(appointment);
        }

        [HttpGet("project/{appointmentId}")]
        public IActionResult GetProjectByAppointment(string appointmentId)
        {
            if (!_appointmentRepository.AppointmentExists(appointmentId))
                return NotFound();

            var projects = _mapper.Map<List<ProjectModelDto>>(_appointmentRepository.GetProjectByAppointment(appointmentId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(projects);
        }

        [HttpPost] //PM01
        [Authorize(Roles = "PM01")]
        public IActionResult CreateAppointment([FromBody] AppointmentModelDto appointmentCreate)
        {
            if (appointmentCreate == null)
                return BadRequest(ModelState);

            var appointments = _appointmentRepository.GetAppointments().Where(a => a.AppointmentTitle.Trim().ToUpper() == appointmentCreate.AppointmentTitle.TrimEnd().ToUpper()).FirstOrDefault();

            if (appointments != null)
            {
                ModelState.AddModelError("", "Appointment already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var appointmentMap = _mapper.Map<AppointmentModel>(appointmentCreate);

            if (!_appointmentRepository.CreateAppointment(appointmentMap))
            {
                ModelState.AddModelError("", "Something went wrong while savin");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpPut("{appointmentId}")]
        public IActionResult UpdateAppointment(string appointmentId, [FromBody] AppointmentModelDto updatedAppointment)
        {
            if (updatedAppointment == null)
                return BadRequest(ModelState);

            if (appointmentId != updatedAppointment.AppointmentId)
                return BadRequest(ModelState);

            if (!_appointmentRepository.AppointmentExists(appointmentId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            var appointmentMap = _mapper.Map<AppointmentModel>(updatedAppointment);

            if (!_appointmentRepository.UpdateAppointment(appointmentMap))
            {
                ModelState.AddModelError("", "Something went wrong updating appointment");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{appointmentId}")] //PM01
        [Authorize(Roles = "PM01")]
        public IActionResult DeleteAppointment(string appointmentId)
        {
            if (!_appointmentRepository.AppointmentExists(appointmentId))
                return NotFound();

            var appointmentToDelete = _appointmentRepository.GetAppointment(appointmentId);

            if (!ModelState.IsValid) return BadRequest();

            if (_appointmentReserveRepository.AppointmentReserveExistsByAppointment(appointmentId))
            {
                if (!_appointmentReserveRepository.DeleteAppointmentReserveByAppointment(appointmentId))
                {
                    ModelState.AddModelError("", "Somthing went wrong deleting appointmentReserve");
                    return BadRequest(ModelState);
                }

            }

            if (!_appointmentRepository.DeleteAppointment(appointmentToDelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting appointment");
            }

            return NoContent();
        }

    }
}
