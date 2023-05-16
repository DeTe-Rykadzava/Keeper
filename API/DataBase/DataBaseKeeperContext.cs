using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace KeeperAPI.DataBase;

public partial class DataBaseKeeperContext : DbContext
{
    public DataBaseKeeperContext()
    {
    }

    public DataBaseKeeperContext(DbContextOptions<DataBaseKeeperContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    public virtual DbSet<ApplicaitonCustomer> ApplicaitonCustomers { get; set; }

    public virtual DbSet<Application> Applications { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Subdivision> Subdivisions { get; set; }

    public virtual DbSet<User> Users { get; set; }

//     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){
// //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
//         //optionsBuilder.UseNpgsql("host=localhost;port=5432;database=DataBaseKeeper;Username=postgres;Password=root;");
//         optionsBuilder.UseNpgsql();
//     }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicaitonCustomer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ApplicaitonCustomers_pkey");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ApplicationId).HasColumnName("Application_ID");
            entity.Property(e => e.CustomerId).HasColumnName("Customer_ID");

            entity.HasOne(d => d.Application).WithMany(p => p.ApplicaitonCustomers)
                .HasForeignKey(d => d.ApplicationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ApplicaitonCustomers_Application_ID_fkey");

            entity.HasOne(d => d.Customer).WithMany(p => p.ApplicaitonCustomers)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ApplicaitonCustomers_Customer_ID_fkey");
        });

        modelBuilder.Entity<Application>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Applications_pkey");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.BeginVisit).HasColumnType("timestamp without time zone");
            entity.Property(e => e.BeginVisitOnSubdivision).HasColumnType("timestamp without time zone");
            entity.Property(e => e.EndVisit).HasColumnType("timestamp without time zone");
            entity.Property(e => e.EndVisitOnSubdivision).HasColumnType("timestamp without time zone");
            entity.Property(e => e.SubdivisionId).HasColumnName("Subdivision_ID");
            entity.Property(e => e.UserId).HasColumnName("User_ID");

            entity.HasOne(d => d.Subdivision).WithMany(p => p.Applications)
                .HasForeignKey(d => d.SubdivisionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Applications_Subdivision_ID_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Applications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Applications_User_ID_fkey");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Customers_pkey");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.NumberPasport).HasMaxLength(6);
            entity.Property(e => e.SeriaPasport).HasMaxLength(4);
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Departments_pkey");

            entity.Property(e => e.Id).HasColumnName("ID");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Employees_pkey");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.DepartmentId).HasColumnName("Department_ID");
            entity.Property(e => e.SubdivisionId).HasColumnName("Subdivision_ID");

            entity.HasOne(d => d.Department).WithMany(p => p.Employees)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("Employees_Department_ID_fkey");

            entity.HasOne(d => d.Subdivision).WithMany(p => p.Employees)
                .HasForeignKey(d => d.SubdivisionId)
                .HasConstraintName("Employees_Subdivision_ID_fkey");
        });

        modelBuilder.Entity<Subdivision>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Subdivisions_pkey");

            entity.Property(e => e.Id).HasColumnName("ID");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Users_pkey");

            entity.Property(e => e.Id).HasColumnName("ID");
        });
        modelBuilder.HasSequence<int>("ApplicaitonCustomers_ID_seq");
        modelBuilder.HasSequence<int>("Applications_ID_seq");
        modelBuilder.HasSequence<int>("Customers_ID_seq");
        modelBuilder.HasSequence<int>("Departments_ID_seq");
        modelBuilder.HasSequence<int>("Employees_ID_seq");
        modelBuilder.HasSequence<int>("Subdivisions_ID_seq");
        modelBuilder.HasSequence<int>("Users_ID_seq");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
