using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace NetworkOfLibrariesWebApplication;

public partial class DbnetworkOfLibrariesContext : DbContext
{
    public DbnetworkOfLibrariesContext()
    {
    }

    public DbnetworkOfLibrariesContext(DbContextOptions<DbnetworkOfLibrariesContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<AuthorBook> AuthorBooks { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<BookLibrary> BookLibraries { get; set; }


    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Library> Libraries { get; set; }

    public virtual DbSet<Publisher> Publishers { get; set; }

    public virtual DbSet<Style> Styles { get; set; }

    public static int? libid { get; set; }
    public static int? auid { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
            entity.Property(e => e.Datebirth).HasMaxLength(50);
            entity.Property(e => e.Education).HasMaxLength(400);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.Surname).HasMaxLength(50);
        });

        /*modelBuilder.Entity<AuthorBook>(entity =>
        {
            entity.ToTable("AuthorBooks");
            entity.HasOne(d => d.Author).WithMany(p => p.AuthorBooks)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AuthorBooks_Authors");

            entity.HasOne(d => d.Book).WithMany(p => p.AuthorBooks)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AuthorBooks_Books"); 
        });*/

        modelBuilder.Entity<Book>(entity =>
        {
            entity.Property(e => e.Annotation).HasMaxLength(100);
            entity.Property(e => e.Title).HasMaxLength(50);
            entity.HasOne(d => d.Publisher).WithMany(p => p.Books)
                .HasForeignKey(d => d.PublisherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Books_Publishers");

            entity.HasOne(d => d.Style).WithMany(p => p.Books)
                .HasForeignKey(d => d.StyleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Books_Styles");
        });

        modelBuilder.Entity<BookLibrary>(entity =>
        {
            entity.HasOne(d => d.Book).WithMany(p => p.BookLibraries)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookLibraries_Books");

            entity.HasOne(d => d.Library).WithMany(p => p.BookLibraries)
                .HasForeignKey(d => d.LibraryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookLibraries_Libraries");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Region).HasMaxLength(50);
        });

        modelBuilder.Entity<Library>(entity =>
        {
            entity.Property(e => e.Adress).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Schedule).HasMaxLength(50);
            entity.Property(e => e.Website).HasMaxLength(50);

            entity.HasOne(d => d.City).WithMany(p => p.Libraries)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Libraries_Cities");
        });

        modelBuilder.Entity<Publisher>(entity =>
        {
            entity.Property(e => e.Adress).HasMaxLength(100);
            entity.Property(e => e.Owner).HasMaxLength(50);
            entity.Property(e => e.Power).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(50);

            entity.HasOne(d => d.City).WithMany(p => p.Publishers)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Publishers_Cities");
        });

        modelBuilder.Entity<Style>(entity =>
        {
            entity.Property(e => e.Info).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Type).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
