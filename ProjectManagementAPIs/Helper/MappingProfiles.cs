using AutoMapper;
using ProjectManagementAPIs.Dto;
using ProjectManagementAPIs.Models;

namespace ProjectManagementAPIs.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<ProjectModel, ProjectModelDto>();
            CreateMap<ProjectModelDto, ProjectModel>();
            CreateMap<RoleModel, RoleModelDto>();
            CreateMap<MemberUserModel, MemberUserModelDto>();
            CreateMap<MemberUserModelDto, MemberUserModel>();
            CreateMap<ProjectProgressModel, ProjectProgressModelDto>();
            CreateMap<ProjectProgressModelDto, ProjectProgressModel>();
            CreateMap<AppointmentModel, AppointmentModelDto>();
            CreateMap<AppointmentModelDto, AppointmentModel>();

            CreateMap<AdviseeModel, AdviseeModelDto>();
            CreateMap<AdviseeModelDto, AdviseeModel>();

            CreateMap<AdviserModel, AdviserModelDto>();
            CreateMap<AdviserModelDto, AdviserModel>();

            CreateMap<AppointmentReserveModel, AppointmentReserveModelDto>();
            CreateMap<AppointmentReserveModelDto, AppointmentReserveModel>();

            CreateMap<MemberUserModel, LoginModelDto>();






        }
    }
}
