using MiPruebaTecnica.Domain;

namespace MiPruebaTecnica.Infrastructure.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Currency> Currencies => Set<Currency>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Email).IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Password).IsRequired();
        });

        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Street).IsRequired();
            entity.Property(e => e.City).IsRequired();
            entity.Property(e => e.Country).IsRequired();
            entity.HasOne(e => e.User)
                .WithMany(u => u.Addresses)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Currency>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).IsRequired();
            entity.HasIndex(e => e.Code).IsUnique();
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.RateToBase).IsRequired();

            entity.HasData(
                new Currency
                {
                    Id = 1,
                    Code = "PYG",
                    Name = "Guaraní paraguayo",
                    RateToBase = 1.0m
                },
                new Currency
                {
                    Id = 2,
                    Code = "USD",
                    Name = "Dólar estadounidense",
                    RateToBase = 0.00014m
                }
            );
        });
    }
}
