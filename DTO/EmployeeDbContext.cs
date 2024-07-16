using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MinimalApi.DTO;

public class EmployeeDbContext:IdentityDbContext<ApplicationUser,ApplicationUserRole,string>
{

    public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options)
               : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<Religion> Religions { get; set; }
}
