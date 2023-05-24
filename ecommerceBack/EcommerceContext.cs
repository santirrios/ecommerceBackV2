using ecommerceBack.models;
using Microsoft.EntityFrameworkCore;
namespace ecommerceBack;

public class EcommerceContext : DbContext
{
    public DbSet<Product> Products {get;set;}
    public DbSet<Category> Categories {get; set;}
    public DbSet<Image> Images {get; set;}
    public DbSet<UserWithId> Users {get; set;}
    public EcommerceContext(DbContextOptions<EcommerceContext> options):base(options){}
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserWithId>(user =>
        {
            user.ToTable("users");
            user.HasKey(u => u.UserId);

            user.Property(u => u.FirstName).IsRequired().HasMaxLength(50);
            user.Property(u => u.LastName).IsRequired().HasMaxLength(50);
            user.Property(u => u.Rol).IsRequired().HasMaxLength(15);
            user.Property(u => u.Email).IsRequired().HasMaxLength(60);
            user.Property(u => u.Password).IsRequired().HasMaxLength(60);

        });

        modelBuilder.Entity<Category>(category =>
        {
            category.ToTable("categories");
            category.HasKey(c => c.CategoryId);

            category.Property(c => c.Name).IsRequired().HasMaxLength(50);
            category.Property(c => c.Description).IsRequired().HasMaxLength(120);

        });

        modelBuilder.Entity<Product>(product =>
        {
            product.ToTable("products");
            product.HasKey(p => p.ProductId);
            product.HasOne(P => P.Category).WithMany(c => c.Products).HasForeignKey(p => p.CategoryId).OnDelete(DeleteBehavior.Cascade);

            product.Property(p => p.Name).IsRequired().HasMaxLength(50);
            product.Property(p => p.Description).IsRequired().HasMaxLength(150);
            product.Property(p => p.Price).IsRequired();

        });
        modelBuilder.Entity<Image>(image =>
        {
            image.ToTable("images");
            image.HasKey(i => i.ImageId);
            image.HasOne(i => i.Product).WithMany(p => p.Images).HasForeignKey(i => i.ProductId).OnDelete(DeleteBehavior.Cascade);

            image.Property(i => i.URL).IsRequired().HasMaxLength(500);

        });
    }
}