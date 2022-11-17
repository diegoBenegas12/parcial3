using api.primerparcial.Models;
using Microsoft.EntityFrameworkCore;

namespace api.primerparcial.Data
{
    public class parcial1 : DbContext
    {
        public parcial1(DbContextOptions<parcial1> options) : base(options)
        {

        }

        public DbSet<Clientes> Clientes => Set<Clientes>();

        public DbSet<Ciudades> Ciudades => Set<Ciudades>();


    }
}
