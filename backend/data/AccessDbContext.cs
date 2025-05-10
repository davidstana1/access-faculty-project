using backend.entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace backend.data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Division> Divisions { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<AccessSchedule> AccessSchedules { get; set; }
        public DbSet<AccessLog> AccessLogs { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<GateStatus> GateStatuses { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.Entity<Employee>()
                .HasOne(e => e.Division)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DivisionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Employee>()
                .HasOne(e => e.ApprovedBy)
                .WithMany()
                .HasForeignKey(e => e.ApprovedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AccessSchedule>()
                .HasOne(s => s.Employee)
                .WithMany(e => e.AccessSchedules)
                .HasForeignKey(s => s.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AccessLog>()
                .HasOne(l => l.Employee)
                .WithMany(e => e.AccessLogs)
                .HasForeignKey(l => l.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<User>()
                .HasOne(u => u.Division)
                .WithMany(d => d.Users)
                .HasForeignKey(u => u.DivisionId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.Entity<Employee>()
                .HasIndex(e => e.CNP)
                .IsUnique();

            builder.Entity<Employee>()
                .HasIndex(e => e.BadgeNumber)
                .IsUnique();

            builder.Entity<Employee>()
                .HasIndex(e => e.BluetoothSecurityCode)
                .IsUnique();
        }
    }
}