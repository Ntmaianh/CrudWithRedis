using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Redis.Model.Models;

public partial class DemoRedisContext : DbContext
{
    public DemoRedisContext()
    {
    }

    public DemoRedisContext(DbContextOptions<DemoRedisContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-9A9P2Q7\\SQLEXPRESS;Database=demoRedis;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
    }
    public DbSet<Product> Products { get; set; }
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
