using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DoeSangue.Infrastructure.Data
{
    public class DoeSangueContextFactory : IDesignTimeDbContextFactory<DoeSangueContext>
    {
        public DoeSangueContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DoeSangueContext>();
            var connectionString = "Server=.;Database=DoeSangue;Integrated Security=true;TrustServerCertificate=true";
            optionsBuilder.UseSqlServer(connectionString);

            return new DoeSangueContext(optionsBuilder.Options);
        }
    }
}
