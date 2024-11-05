using System;
using System.Collections.Generic;
using System.IO;
using Capybook.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Capybook;

public partial class Prn212ProjectCapybookContext : DbContext
{
    public Prn212ProjectCapybookContext()
    {
    }

    public Prn212ProjectCapybookContext(DbContextOptions<Prn212ProjectCapybookContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<Voucher> Vouchers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(GetConnectionString());
    }

    private string? GetConnectionString()
    {
        IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true).Build();
        return configuration["ConnectionStrings:DBDefault"];
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Username);

            entity.ToTable("Account");

            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("username");
            entity.Property(e => e.AccStatus).HasColumnName("accStatus");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.Dob).HasColumnName("dob");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(255)
                .HasColumnName("firstName");
            entity.Property(e => e.LastName)
                .HasMaxLength(255)
                .HasColumnName("lastName");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("phone");
            entity.Property(e => e.Role).HasColumnName("role");
            entity.Property(e => e.Sex).HasColumnName("sex");
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.ToTable("Book");

            entity.Property(e => e.BookId).HasColumnName("bookID");
            entity.Property(e => e.Author)
                .HasMaxLength(255)
                .HasColumnName("author");
            entity.Property(e => e.BookDescription)
                .HasColumnType("text")
                .HasColumnName("bookDescription");
            entity.Property(e => e.BookPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("bookPrice");
            entity.Property(e => e.BookQuantity).HasColumnName("bookQuantity");
            entity.Property(e => e.BookStatus).HasColumnName("bookStatus");
            entity.Property(e => e.BookTitle)
                .HasMaxLength(255)
                .HasColumnName("bookTitle");
            entity.Property(e => e.CatId).HasColumnName("catID");
            entity.Property(e => e.Dimension)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("dimension");
            entity.Property(e => e.Hardcover).HasColumnName("hardcover");
            entity.Property(e => e.Image)
                .HasColumnType("text")
                .HasColumnName("image");
            entity.Property(e => e.Isbn)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("isbn");
            entity.Property(e => e.PublicationYear).HasColumnName("publicationYear");
            entity.Property(e => e.Publisher)
                .HasMaxLength(255)
                .HasColumnName("publisher");
            entity.Property(e => e.Translator)
                .HasMaxLength(255)
                .HasColumnName("translator");
            entity.Property(e => e.Weight).HasColumnName("weight");

            entity.HasOne(d => d.Cat).WithMany(p => p.Books)
                .HasForeignKey(d => d.CatId)
                .HasConstraintName("FK_Book_Category");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CatId);

            entity.ToTable("Category");

            entity.Property(e => e.CatId).HasColumnName("catID");
            entity.Property(e => e.CatName)
                .HasMaxLength(255)
                .HasColumnName("catName");
            entity.Property(e => e.CatStatus).HasColumnName("catStatus");
            entity.Property(e => e.ParentCatId).HasColumnName("parentCatID");

            entity.HasOne(d => d.ParentCat).WithMany(p => p.InverseParentCat)
                .HasForeignKey(d => d.ParentCatId)
                .HasConstraintName("FK_Category_Category");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Order");

            entity.Property(e => e.OrderId).HasColumnName("orderID");
            entity.Property(e => e.OrderDate).HasColumnName("orderDate");
            entity.Property(e => e.OrderStatus).HasColumnName("orderStatus");
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("username");
            entity.Property(e => e.VouId).HasColumnName("vouID");

            entity.HasOne(d => d.UsernameNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.Username)
                .HasConstraintName("FK_Order_Account");

            entity.HasOne(d => d.Vou).WithMany(p => p.Orders)
                .HasForeignKey(d => d.VouId)
                .HasConstraintName("FK_Order_Promotion");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.Odid);

            entity.ToTable("OrderDetail");

            entity.Property(e => e.Odid).HasColumnName("ODID");
            entity.Property(e => e.BookId).HasColumnName("bookID");
            entity.Property(e => e.OrderId).HasColumnName("orderID");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Book).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.BookId)
                .HasConstraintName("FK_OrderDetail_Book");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_OrderDetail_Order");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupId);

            entity.ToTable("Supplier");

            entity.Property(e => e.SupId).HasColumnName("supID");
            entity.Property(e => e.SupAddress)
                .HasMaxLength(255)
                .HasColumnName("supAddress");
            entity.Property(e => e.SupEmail)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("supEmail");
            entity.Property(e => e.SupName)
                .HasMaxLength(255)
                .HasColumnName("supName");
            entity.Property(e => e.SupPhone)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("supPhone");
            entity.Property(e => e.SupStatus).HasColumnName("supStatus");
        });

        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.HasKey(e => e.VouId).HasName("PK_Promotion");

            entity.ToTable("Voucher");

            entity.Property(e => e.VouId).HasColumnName("vouID");
            entity.Property(e => e.Discount).HasColumnName("discount");
            entity.Property(e => e.EndDate).HasColumnName("endDate");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.StartDate).HasColumnName("startDate");
            entity.Property(e => e.VouCode)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("vouCode");
            entity.Property(e => e.VouName)
                .HasMaxLength(255)
                .HasColumnName("vouName");
            entity.Property(e => e.VouStatus).HasColumnName("vouStatus");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
