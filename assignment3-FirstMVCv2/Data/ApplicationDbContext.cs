using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using assignment3_FirstMVCv2.Models;

namespace assignment3_FirstMVCv2.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<assignment3_FirstMVCv2.Models.Movie> Movie { get; set; }
        public DbSet<assignment3_FirstMVCv2.Models.Actor> Actor { get; set; }
        public DbSet<assignment3_FirstMVCv2.Models.ActorMovie> ActorMovie { get; set; }
    }
}