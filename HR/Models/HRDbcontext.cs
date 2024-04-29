using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace HR.Models
{
    public class HRDbcontext:IdentityDbContext<ApplictionUsers>
    {
        public HRDbcontext(DbContextOptions options):base(options)

        {
            
        }
        public virtual DbSet<informationAttendencperMonth> Attendencpermonth { get; set; }
        public virtual DbSet<ApplictionUsers> Users { get; set; }
        public virtual DbSet<Employee>Employees { get; set; }
        public virtual DbSet<HolidaySetting> Holidays { get; set; }
        public virtual DbSet<department> Departments { get; set; }
        public virtual DbSet<PublicSetting> PublicSettings { get; set; }
        public virtual DbSet<AttendenceEmployee> AttendenceEmployees { get; set; }
    }
}
