using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AdvancedProgrammingASPProject.Areas.Identity.Data;

public class AdvancedProgrammingASPProjectContext : IdentityDbContext<Users>
{
    public AdvancedProgrammingASPProjectContext(DbContextOptions<AdvancedProgrammingASPProjectContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Add any additional customizations if needed
    }
}
