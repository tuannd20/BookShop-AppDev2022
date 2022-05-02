using BookShop.Areas.Identity.Data;
using BookShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Data;

public class BookShopContext : IdentityDbContext<BookShopUser>
{
    public BookShopContext(DbContextOptions<BookShopContext> options)
        : base(options)
    {
    }
    public DbSet<Store> Stores { get; set; }


    public DbSet<Book> Books { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    public DbSet<Cart> Carts { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<BookShopUser>()
            .HasOne<Store>(au => au.Store)
            .WithOne(st => st.User)
            .HasForeignKey<Store>(st => st.UserId);

        builder.Entity<Book>()
            .ToTable("Book")
            .HasOne<Store>(b => b.Store)
            .WithMany(st => st.Books)
            .HasForeignKey(b => b.StoreId);

        builder.Entity<Store>().ToTable("Store");
        builder.Entity<Order>()
            .ToTable("Order")
            .HasOne<BookShopUser>(o => o.User)
            .WithMany(ap => ap.Orders)
            .HasForeignKey(o => o.UserId);

        builder.Entity<OrderDetail>()
            .ToTable("OrderDetail")
            .HasKey(od => new { od.OrderId, od.BookIsbn });
        builder.Entity<OrderDetail>()
            .HasOne<Order>(od => od.Order)
            .WithMany(or => or.OrderDetails)
            .HasForeignKey(od => od.OrderId);
        builder.Entity<OrderDetail>()
            .HasOne<Book>(od => od.Book)
            .WithMany(b => b.OrderDetails)
            .HasForeignKey(od => od.BookIsbn);

        builder.Entity<Cart>().ToTable("Cart");

        builder.Entity<Cart>()
           .ToTable("Cart")
           .HasKey(c => new { c.UserId, c.BookIsbn });
        builder.Entity<BookShopUser>()
           .HasOne<Cart>(au => au.Carts)
           .WithOne(st => st.User)
           .HasForeignKey<Cart>(st => st.UserId);
        builder.Entity<Cart>()
            .HasOne<Book>(od => od.Book)
            .WithMany(b => b.Carts)
            .HasForeignKey(od => od.BookIsbn);
    }
}
