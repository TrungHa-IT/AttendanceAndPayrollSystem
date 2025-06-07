using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BusinessObject.Models;

public partial class FunattendanceAndPayrollSystemContext : DbContext
{
    public FunattendanceAndPayrollSystemContext()
    {
    }

    public FunattendanceAndPayrollSystemContext(DbContextOptions<FunattendanceAndPayrollSystemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Attendance> Attendances { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Payroll> Payrolls { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(config.GetConnectionString("Mycnn"));
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("Account");

            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(50);
        });

        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.ToTable("Attendance");

            entity.HasOne(d => d.Employee).WithMany(p => p.Attendances)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Attendance_Employee");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployId);

            entity.ToTable("Employee");

            entity.Property(e => e.EmployeeName).HasMaxLength(50);

            entity.HasOne(d => d.Account).WithMany(p => p.Employees)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_Employee_Account");
        });

        modelBuilder.Entity<Payroll>(entity =>
        {
            entity.Property(e => e.TotalSalary).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.TotalWorkHour).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.Employee).WithMany(p => p.Payrolls)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payrolls_Employee");
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.ToTable("Schedule");

            entity.HasOne(d => d.Employee).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Schedule_Employee");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
