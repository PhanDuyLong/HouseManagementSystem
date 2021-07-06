using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace HMS.Data.Models
{
    public partial class HMSDBContext : DbContext
    {
        public HMSDBContext()
        {
        }

        public HMSDBContext(DbContextOptions<HMSDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Bill> Bills { get; set; }
        public virtual DbSet<BillItem> BillItems { get; set; }
        public virtual DbSet<Clock> Clocks { get; set; }
        public virtual DbSet<ClockCategory> ClockCategories { get; set; }
        public virtual DbSet<ClockValue> ClockValues { get; set; }
        public virtual DbSet<Contract> Contracts { get; set; }
        public virtual DbSet<House> Houses { get; set; }
        public virtual DbSet<HouseInfo> HouseInfos { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<ServiceContract> ServiceContracts { get; set; }
        public virtual DbSet<ServiceType> ServiceTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(e => e.Username)
                    .HasName("PK_Account_1");

                entity.ToTable("Account");

                entity.Property(e => e.Username)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Password)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Role).HasMaxLength(20);
            });

            modelBuilder.Entity<Bill>(entity =>
            {
                entity.ToTable("Bill");

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.IssueDate).HasColumnType("date");

                entity.Property(e => e.Note).HasMaxLength(150);

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.HasOne(d => d.Contract)
                    .WithMany(p => p.Bills)
                    .HasForeignKey(d => d.ContractId)
                    .HasConstraintName("FK_Bill_Contract");
            });

            modelBuilder.Entity<BillItem>(entity =>
            {
                entity.ToTable("BillItem");

                entity.HasOne(d => d.Bill)
                    .WithMany(p => p.BillItems)
                    .HasForeignKey(d => d.BillId)
                    .HasConstraintName("FK_Bill_Item_Bill");

                entity.HasOne(d => d.ServiceContract)
                    .WithMany(p => p.BillItems)
                    .HasForeignKey(d => d.ServiceContractId)
                    .HasConstraintName("FK_BillItem_ServiceContract");
            });

            modelBuilder.Entity<Clock>(entity =>
            {
                entity.ToTable("Clock");

                entity.Property(e => e.Id)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ClockCategoryId)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.HasOne(d => d.ClockCategory)
                    .WithMany(p => p.Clocks)
                    .HasForeignKey(d => d.ClockCategoryId)
                    .HasConstraintName("FK_Clock_Clock_Category");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.Clocks)
                    .HasForeignKey(d => d.RoomId)
                    .HasConstraintName("FK_Clock_Room");
            });

            modelBuilder.Entity<ClockCategory>(entity =>
            {
                entity.ToTable("ClockCategory");

                entity.Property(e => e.Id)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<ClockValue>(entity =>
            {
                entity.ToTable("ClockValue");

                entity.Property(e => e.ClockId)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.CreateDate).HasColumnType("date");

                entity.Property(e => e.RecordDate).HasColumnType("date");

                entity.HasOne(d => d.Clock)
                    .WithMany(p => p.ClockValues)
                    .HasForeignKey(d => d.ClockId)
                    .HasConstraintName("FK_Clock_Value_Clock");
            });

            modelBuilder.Entity<Contract>(entity =>
            {
                entity.ToTable("Contract");

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.OwnerUsername)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.Property(e => e.TenantUsername)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.Contracts)
                    .HasForeignKey(d => d.RoomId)
                    .HasConstraintName("FK_Contract_Room");

                entity.HasOne(d => d.TenantUsernameNavigation)
                    .WithMany(p => p.Contracts)
                    .HasForeignKey(d => d.TenantUsername)
                    .HasConstraintName("FK_Contract_Account1");
            });

            modelBuilder.Entity<House>(entity =>
            {
                entity.ToTable("House");

                entity.Property(e => e.Id)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.OwnerUsername)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.HasOne(d => d.OwnerUsernameNavigation)
                    .WithMany(p => p.Houses)
                    .HasForeignKey(d => d.OwnerUsername)
                    .HasConstraintName("FK_House_Account");
            });

            modelBuilder.Entity<HouseInfo>(entity =>
            {
                entity.ToTable("HouseInfo");

                entity.Property(e => e.Address).HasMaxLength(80);

                entity.Property(e => e.HouseId)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Image).IsUnicode(false);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.HasOne(d => d.House)
                    .WithMany(p => p.HouseInfos)
                    .HasForeignKey(d => d.HouseId)
                    .HasConstraintName("FK_HouseInfo_House");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("Payment");

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.Note).HasMaxLength(50);

                entity.HasOne(d => d.Bill)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.BillId)
                    .HasConstraintName("FK_Payment_Bill");
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.ToTable("Room");

                entity.Property(e => e.HouseId)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.HasOne(d => d.House)
                    .WithMany(p => p.Rooms)
                    .HasForeignKey(d => d.HouseId)
                    .HasConstraintName("FK_Room_House1");
            });

            modelBuilder.Entity<Service>(entity =>
            {
                entity.ToTable("Service");

                entity.Property(e => e.CalculationUnit).HasMaxLength(50);

                entity.Property(e => e.HouseId)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.HasOne(d => d.House)
                    .WithMany(p => p.Services)
                    .HasForeignKey(d => d.HouseId)
                    .HasConstraintName("FK_Service_House");

                entity.HasOne(d => d.ServiceType)
                    .WithMany(p => p.Services)
                    .HasForeignKey(d => d.ServiceTypeId)
                    .HasConstraintName("FK_Service_ServiceType");
            });

            modelBuilder.Entity<ServiceContract>(entity =>
            {
                entity.ToTable("ServiceContract");

                entity.Property(e => e.ClockId)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.HasOne(d => d.Clock)
                    .WithMany(p => p.ServiceContracts)
                    .HasForeignKey(d => d.ClockId)
                    .HasConstraintName("FK_ServiceContract_Clock");

                entity.HasOne(d => d.Contract)
                    .WithMany(p => p.ServiceContracts)
                    .HasForeignKey(d => d.ContractId)
                    .HasConstraintName("FK_ServiceContract_Contract");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.ServiceContracts)
                    .HasForeignKey(d => d.ServiceId)
                    .HasConstraintName("FK_ServiceContract_Service");
            });

            modelBuilder.Entity<ServiceType>(entity =>
            {
                entity.ToTable("ServiceType");

                entity.Property(e => e.Name).HasMaxLength(100);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
