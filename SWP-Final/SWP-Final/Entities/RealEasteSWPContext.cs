using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SWP_Final.Entities
{
    public partial class RealEasteSWPContext : DbContext
    {
        public RealEasteSWPContext()
        {
        }

        public RealEasteSWPContext(DbContextOptions<RealEasteSWPContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Agency> Agencies { get; set; } = null!;
        public virtual DbSet<Apartment> Apartments { get; set; } = null!;
        public virtual DbSet<Booking> Bookings { get; set; } = null!;
        public virtual DbSet<Building> Buildings { get; set; } = null!;
        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<Post> Posts { get; set; } = null!;
        public virtual DbSet<Project> Projects { get; set; } = null!;
        public virtual DbSet<ProjectUtility> ProjectUtilities { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<Utility> Utilities { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=ADMIN;Initial Catalog=RealEasteSWP;User ID=sa;Password=12345;Encrypt=True;Trust Server Certificate=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Agency>(entity =>
            {
                entity.ToTable("Agency");

                entity.Property(e => e.AgencyId)
                    .HasMaxLength(255)
                    .HasColumnName("AgencyID");

                entity.Property(e => e.Address)
                    .HasMaxLength(255)
                    .UseCollation("Vietnamese_CI_AS");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(255)
                    .UseCollation("Vietnamese_CI_AS");

                entity.Property(e => e.Gender)
                    .HasMaxLength(50)
                    .UseCollation("Vietnamese_CI_AS");

                entity.Property(e => e.LastName)
                    .HasMaxLength(255)
                    .UseCollation("Vietnamese_CI_AS");

                entity.Property(e => e.Phone)
                    .HasMaxLength(50)
                    .UseCollation("Vietnamese_CI_AS");

                entity.Property(e => e.UserId)
                    .HasMaxLength(255)
                    .HasColumnName("UserID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Agencies)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Agency__UserID__29572725");
            });

            modelBuilder.Entity<Apartment>(entity =>
            {
                entity.ToTable("Apartment");

                entity.Property(e => e.ApartmentId)
                    .HasMaxLength(255)
                    .HasColumnName("ApartmentID");

                entity.Property(e => e.AgencyId)
                    .HasMaxLength(255)
                    .HasColumnName("AgencyID");

                entity.Property(e => e.ApartmentType)
                    .HasMaxLength(50)
                    .UseCollation("Vietnamese_CI_AS");

                entity.Property(e => e.BuildingId)
                    .HasMaxLength(255)
                    .HasColumnName("BuildingID");

                entity.Property(e => e.Description).UseCollation("Vietnamese_CI_AS");

                entity.Property(e => e.Furniture)
                    .HasMaxLength(255)
                    .UseCollation("Vietnamese_CI_AS");

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .UseCollation("Vietnamese_CI_AS");

                entity.HasOne(d => d.Agency)
                    .WithMany(p => p.Apartments)
                    .HasForeignKey(d => d.AgencyId)
                    .HasConstraintName("FK__Apartment__Agenc__37A5467C");

                entity.HasOne(d => d.Building)
                    .WithMany(p => p.Apartments)
                    .HasForeignKey(d => d.BuildingId)
                    .HasConstraintName("FK__Apartment__Build__36B12243");
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.ToTable("Booking");

                entity.Property(e => e.BookingId)
                    .HasMaxLength(255)
                    .HasColumnName("BookingID");

                entity.Property(e => e.AgencyId)
                    .HasMaxLength(255)
                    .HasColumnName("AgencyID");

                entity.Property(e => e.ApartmentId)
                    .HasMaxLength(255)
                    .HasColumnName("ApartmentID");

                entity.Property(e => e.CustomerId)
                    .HasMaxLength(255)
                    .HasColumnName("CustomerID");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .UseCollation("Vietnamese_CI_AS");

                entity.HasOne(d => d.Agency)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.AgencyId)
                    .HasConstraintName("FK__Booking__AgencyI__3E52440B");

                entity.HasOne(d => d.Apartment)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.ApartmentId)
                    .HasConstraintName("FK__Booking__Apartme__3F466844");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__Booking__Custome__403A8C7D");
            });

            modelBuilder.Entity<Building>(entity =>
            {
                entity.ToTable("Building");

                entity.Property(e => e.BuildingId)
                    .HasMaxLength(255)
                    .HasColumnName("BuildingID");

                entity.Property(e => e.Address)
                    .HasMaxLength(255)
                    .UseCollation("Vietnamese_CI_AS");

                entity.Property(e => e.Amenities).UseCollation("Vietnamese_CI_AS");

                entity.Property(e => e.Describe).UseCollation("Vietnamese_CI_AS");

                entity.Property(e => e.Investor)
                    .HasMaxLength(255)
                    .UseCollation("Vietnamese_CI_AS");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .UseCollation("Vietnamese_CI_AS");

                entity.Property(e => e.ProjectId)
                    .HasMaxLength(255)
                    .HasColumnName("ProjectID");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .UseCollation("Vietnamese_CI_AS");

                entity.Property(e => e.TypeOfRealEstate)
                    .HasMaxLength(255)
                    .UseCollation("Vietnamese_CI_AS");

                entity.Property(e => e.YearOfConstruction).HasColumnType("date");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Buildings)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK__Building__Projec__33D4B598");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customer");

                entity.Property(e => e.CustomerId)
                    .HasMaxLength(255)
                    .HasColumnName("CustomerID");

                entity.Property(e => e.Address)
                    .HasMaxLength(255)
                    .UseCollation("Vietnamese_CI_AS");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(255)
                    .UseCollation("Vietnamese_CI_AS");

                entity.Property(e => e.Gender)
                    .HasMaxLength(50)
                    .UseCollation("Vietnamese_CI_AS");

                entity.Property(e => e.LastName)
                    .HasMaxLength(255)
                    .UseCollation("Vietnamese_CI_AS");

                entity.Property(e => e.Phone)
                    .HasMaxLength(50)
                    .UseCollation("Vietnamese_CI_AS");

                entity.Property(e => e.UserId)
                    .HasMaxLength(255)
                    .HasColumnName("UserID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Customers)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Customer__UserID__267ABA7A");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.OrderId)
                    .HasMaxLength(255)
                    .HasColumnName("OrderID");

                entity.Property(e => e.AgencyId)
                    .HasMaxLength(255)
                    .HasColumnName("AgencyID");

                entity.Property(e => e.ApartmentId)
                    .HasMaxLength(255)
                    .HasColumnName("ApartmentID");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .UseCollation("Vietnamese_CI_AS");

                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Agency)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.AgencyId)
                    .HasConstraintName("FK__Orders__AgencyID__3A81B327");

                entity.HasOne(d => d.Apartment)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.ApartmentId)
                    .HasConstraintName("FK__Orders__Apartmen__3B75D760");
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.ToTable("Post");

                entity.Property(e => e.PostId)
                    .HasMaxLength(255)
                    .HasColumnName("PostID");

                entity.Property(e => e.AgencyId)
                    .HasMaxLength(255)
                    .HasColumnName("AgencyID");

                entity.Property(e => e.BuildingId)
                    .HasMaxLength(255)
                    .HasColumnName("BuildingID");

                entity.Property(e => e.Description).UseCollation("Vietnamese_CI_AS");

                entity.Property(e => e.PostDate).HasColumnType("datetime");

                entity.Property(e => e.PriorityMethod)
                    .HasMaxLength(255)
                    .UseCollation("Vietnamese_CI_AS");

                entity.Property(e => e.SalesClosingDate).HasColumnType("datetime");

                entity.Property(e => e.SalesOpeningDate).HasColumnType("datetime");

                entity.HasOne(d => d.Agency)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.AgencyId)
                    .HasConstraintName("FK__Post__AgencyID__440B1D61");

                entity.HasOne(d => d.Building)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.BuildingId)
                    .HasConstraintName("FK__Post__BuildingID__4316F928");
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.ToTable("Project");

                entity.Property(e => e.ProjectId)
                    .HasMaxLength(255)
                    .HasColumnName("ProjectID");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .UseCollation("Vietnamese_CI_AS");
            });

            modelBuilder.Entity<ProjectUtility>(entity =>
            {
                entity.HasKey(e => e.ProjectUtilitiesId)
                    .HasName("PK__ProjectU__44DCD06B9EA9089B");

                entity.Property(e => e.ProjectUtilitiesId)
                    .HasMaxLength(255)
                    .HasColumnName("ProjectUtilitiesID");

                entity.Property(e => e.ProjectId)
                    .HasMaxLength(255)
                    .HasColumnName("ProjectID");

                entity.Property(e => e.UtilitiesId)
                    .HasMaxLength(255)
                    .HasColumnName("UtilitiesID");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.ProjectUtilities)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK__ProjectUt__Proje__300424B4");

                entity.HasOne(d => d.Utilities)
                    .WithMany(p => p.ProjectUtilities)
                    .HasForeignKey(d => d.UtilitiesId)
                    .HasConstraintName("FK__ProjectUt__Utili__30F848ED");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserId)
                    .HasMaxLength(255)
                    .HasColumnName("UserID");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Password).HasMaxLength(255);

                entity.Property(e => e.RoleId)
                    .HasMaxLength(255)
                    .HasColumnName("RoleID");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .UseCollation("Vietnamese_CI_AS");

                entity.Property(e => e.Username)
                    .HasMaxLength(255)
                    .UseCollation("Vietnamese_CI_AS");
            });

            modelBuilder.Entity<Utility>(entity =>
            {
                entity.HasKey(e => e.UtilitiesId)
                    .HasName("PK__Utilitie__655EED220928EFAD");

                entity.Property(e => e.UtilitiesId)
                    .HasMaxLength(255)
                    .HasColumnName("UtilitiesID");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .UseCollation("Vietnamese_CI_AS");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
