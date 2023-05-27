using Microsoft.EntityFrameworkCore;
using RefereeApp.Entities;

namespace RefereeApp.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    public DbSet<Club> Clubs { get; set; }
    public DbSet<Fixture> Fixtures { get; set; }
    public DbSet<Referee> Referees { get; set; }
    public DbSet<RefereeLevel> RefereeLevels { get; set; }
    public DbSet<RefereeRegion> RefereeRegions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Referee>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasOne<RefereeLevel>(x => x.RefereeLevel)
                .WithOne(x => x.Referee)
                .HasForeignKey<RefereeLevel>(x => x.Id);

            entity.HasOne<RefereeRegion>(x => x.RefereeRegion)
                .WithOne(x => x.Referee)
                .HasForeignKey<RefereeRegion>(x => x.Id);

        });

        modelBuilder.Entity<Fixture>(entity =>
        {
            entity.HasOne<Referee>(x => x.Referee)
                .WithMany(x => x.Fixtures)
                .HasForeignKey(x => x.Id);
        });

    }
}