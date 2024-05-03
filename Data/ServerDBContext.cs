using DogGrooming_Server.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DogGrooming_Server.Data
{
    public class ServerDBContext : DbContext
    {
        public ServerDBContext(DbContextOptions<ServerDBContext> options) : base(options)
        {
        }

        public DbSet<User> User { get; set; }
        public DbSet<Appointment> Appointment { get; set; }
    }
}
