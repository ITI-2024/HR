using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace HR.Models
{
    public class HRDbcontext : IdentityDbContext<ApplictionUsers, group, string>
    {
        public HRDbcontext(DbContextOptions<HRDbcontext> options) : base(options)
        {

        }


        public virtual DbSet<informationAttendencperMonth> Attendencpermonth { get; set; }
        public virtual DbSet<ApplictionUsers> Users { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<HolidaySetting> Holidays { get; set; }
        public virtual DbSet<department> Departments { get; set; }
        public virtual DbSet<PublicSetting> PublicSettings { get; set; }
        public DbSet<group> Roles { get; set; }
        public virtual DbSet<AttendenceEmployee> AttendenceEmployees { get; set; }


        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    modelBuilder.Entity<AttendenceEmployee>()
        //        .HasOne(a => a.Emp)                     // Defines a navigation property 'Emp' in AttendenceEmployee
        //        .WithMany(e => e.Attendence)             // Indicates that an Employee can have multiple AttendenceEmployee records
        //        .HasForeignKey(a => a.idemp)            // Specifies the foreign key property in AttendenceEmployee
        //        .OnDelete(DeleteBehavior.Cascade);       // Sets cascade delete behavior for AttendenceEmployee records related to an Employee

        //    base.OnModelCreating(modelBuilder);          // Calls the base implementation of OnModelCreating
        //}
    }

}
