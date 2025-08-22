using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SimulacaoCredito.Infrastructure.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        
        // Connection string para design-time
        var connectionString = "Server=localhost,1433;Database=SimulacaoCredito;User Id=hackathon;Password=TimeBECID@123;Encrypt=False;TrustServerCertificate=True;";
        
        optionsBuilder.UseSqlServer(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}
