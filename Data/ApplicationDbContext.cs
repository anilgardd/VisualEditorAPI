using Microsoft.EntityFrameworkCore;
using VisualEditorAPI.Models;

namespace VisualEditorAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Design> Designs { get; set; }
        public DbSet<Component> Components { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Design>()
                .HasOne(d => d.User)
                .WithMany(u => u.Designs)
                .HasForeignKey(d => d.UserId);

            base.OnModelCreating(modelBuilder);
        }
    }
}