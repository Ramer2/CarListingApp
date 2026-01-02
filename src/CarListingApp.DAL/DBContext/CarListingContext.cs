using CarListingApp.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace CarListingApp.DAL.DBContext;

public partial class CarListingContext : DbContext
{
    public CarListingContext()
    {
    }

    public CarListingContext(DbContextOptions<CarListingContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Car> Cars { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserFavorite> UserFavorites { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Car>(entity =>
        {
            entity.ToTable("Car");

            entity.HasIndex(e => e.Vin, "IX_Car_VIN").IsUnique();

            entity.Property(e => e.Vin).HasColumnName("VIN");

            entity.HasOne(d => d.SellerNavigation).WithMany(p => p.Cars)
                .HasForeignKey(d => d.Seller)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.StatusNavigation).WithMany(p => p.Cars)
                .HasForeignKey(d => d.Status)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.ToTable("Status");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "IX_User_Email").IsUnique();

            entity.HasIndex(e => e.Username, "IX_User_Username").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");

            entity.HasOne(d => d.RoleNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.Role)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<UserFavorite>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.CarId });

            entity.Property(e => e.UserId).HasColumnName("User_Id");
            entity.Property(e => e.CarId).HasColumnName("Car_Id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Car).WithMany(p => p.UserFavorites)
                .HasForeignKey(d => d.CarId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.User).WithMany(p => p.UserFavorites)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
