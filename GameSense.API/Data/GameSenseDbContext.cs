using GameSense.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GameSense.API.Data
{
    //class representing database and tables in it

    public class GameSenseDbContext: DbContext
    {
        public DbSet<Session> Sessions { get; set; }
        public DbSet<DeathEvent> DeathEvents { get; set; }
        public GameSenseDbContext(DbContextOptions<GameSenseDbContext> options) : base(options) { 
        }
    }
}
