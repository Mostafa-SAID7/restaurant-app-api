using System;
using RestuarantAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace RestuarantAPI.Data;

public class AppDbContext :DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
    {

    }

    public DbSet<User> Users{get; set;}
    public DbSet<Item> Items{get; set;}
    public DbSet<Order> Orders{get; set;}
    public DbSet<Restaurant> Restaurants{get;set;}

    public DbSet<MasterOrder> MasterOrders{get;set;}

    public DbSet<Cart> Carts{get;set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
   

modelBuilder.Entity<User>()
        .HasIndex(mo => mo.Usercode)
        .IsUnique();
modelBuilder.Entity<User>()
        .HasKey(mo => mo.UserEmail);

    base.OnModelCreating(modelBuilder);
}


}


       





