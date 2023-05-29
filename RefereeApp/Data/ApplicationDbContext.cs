using Microsoft.EntityFrameworkCore;
using RefereeApp.Entities;
using RefereeApp.Entities.Enums;

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

        modelBuilder.Entity<Referee>()
            .HasOne(x => x.RefereeLevel)
            .WithOne(x => x.Referee)
            .HasForeignKey<Referee>(x => x.RefLevelId);

        modelBuilder.Entity<Referee>()
            .HasOne(x => x.RefereeRegion)
            .WithOne(x => x.Referee)
            .HasForeignKey<Referee>(x => x.RefRegionId);

        modelBuilder.Entity<FixtureClub>().HasKey(x => new {x.ClubId,x.FixtureId});

        modelBuilder.Entity<FixtureClub>()
            .HasOne(x => x.Fixture)
            .WithMany(x => x.FixtureClubs)
            .HasForeignKey(x => x.FixtureId);

        modelBuilder.Entity<FixtureClub>()
            .HasOne(x => x.Club)
            .WithMany(x => x.FixtureClubs)
            .HasForeignKey(x => x.ClubId);
        
    }
}