using System;
using Microsoft.EntityFrameworkCore;

namespace productService.Models
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options)
        : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //取消DB名稱加s
            modelBuilder.Entity<Product>().ToTable("Product");
        }
    }
}
