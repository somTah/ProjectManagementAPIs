using Microsoft.EntityFrameworkCore;
using ProjectManagementAPIs.Models;

namespace ProjectManagementAPIs.Data
{
    public class DataContext : DbContext
    {
        public DbSet<AdviseeModel> Advisees { get; set; }
        public DbSet<MemberUserModel> MemberUsers { get; set; }
        public DbSet<ProjectModel> Projects { get; set; }
        public DbSet<AdviserModel> Advisers { get; set; }
        public DbSet<RoleModel> Roles { get; set; }
        public DbSet<ProjectProgressModel> ProjectProgresses { get; set; }
        public DbSet<AppointmentModel> Appointments { get; set; }
        public DbSet<AppointmentReserveModel> AppointmentReserves { get; set; }
        public DbSet<RefreshTokenModel> RefreshTokens { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AdviserModel>().ToTable("Adviser");
            modelBuilder.Entity<ProjectModel>().ToTable("Projects");
            modelBuilder.Entity<RoleModel>().ToTable("Role");
            modelBuilder.Entity<MemberUserModel>().ToTable("MemberUser");
            modelBuilder.Entity<AdviseeModel>().ToTable("Advisee");
            modelBuilder.Entity<ProjectProgressModel>().ToTable("ProjectProgress");
            modelBuilder.Entity<AppointmentModel>().ToTable("Appointment");
            modelBuilder.Entity<AppointmentReserveModel>().ToTable("AppointmentReserve");
            modelBuilder.Entity<RefreshTokenModel>().ToTable("RefreshToken");

            modelBuilder.Entity<AdviseeModel>().HasKey(up => new { up.MemberUserId, up.ProjectId });
            modelBuilder.Entity<AdviseeModel>().HasOne(u => u.MemberUser).WithMany(a => a.Advisees).HasForeignKey(u => u.MemberUserId);
            modelBuilder.Entity<AdviseeModel>().HasOne(p => p.Project).WithMany(a => a.Advisees).HasForeignKey(p => p.ProjectId);

            modelBuilder.Entity<AdviserModel>().HasKey(up => new { up.MemberUserId, up.ProjectId });
            modelBuilder.Entity<AdviserModel>().HasOne(u => u.MemberUser).WithMany(a => a.Advisers).HasForeignKey(u => u.MemberUserId);
            modelBuilder.Entity<AdviserModel>().HasOne(p => p.Project).WithMany(a => a.Advisers).HasForeignKey(p => p.ProjectId);

            modelBuilder.Entity<AppointmentReserveModel>().HasKey(ap => new { ap.AppointmentId, ap.ProjectId });
            modelBuilder.Entity<AppointmentReserveModel>().HasOne(a => a.Appointment).WithMany(ar => ar.AppointmentReserves).HasForeignKey(a => a.AppointmentId);
            modelBuilder.Entity<AppointmentReserveModel>().HasOne(p => p.Project).WithMany(ar => ar.AppointmentReserves).HasForeignKey(p => p.ProjectId);

        }

    }
}
