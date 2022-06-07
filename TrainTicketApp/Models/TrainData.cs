using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace TrainTicketApp.Models
{
    public partial class TrainData : DbContext
    {
        public TrainData()
        {
        }

        public TrainData(DbContextOptions<TrainData> options)
            : base(options)
        {
        }

        public virtual DbSet<Day> Days { get; set; }
        public virtual DbSet<Owner> Owners { get; set; }
        public virtual DbSet<Ticket> Tickets { get; set; }
        public virtual DbSet<TicketType> TicketTypes { get; set; }
        public virtual DbSet<Train> Trains { get; set; }
        public virtual DbSet<TrainTime> TrainTimes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\egyetem\\Csh\\TrainTicketApp\\TrainDatabase.mdf;Integrated Security=True;Connect Timeout=30");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Day>(entity =>
            {
                entity.ToTable("Day");

                entity.Property(e => e.Date).HasColumnType("date");
            });

            modelBuilder.Entity<Owner>(entity =>
            {
                entity.ToTable("Owner");

                entity.Property(e => e.Address).IsRequired();

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.ToTable("Ticket");

                entity.Property(e => e.TicketId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.Tickets)
                    .HasForeignKey(d => d.OwnerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Ticket_ToOwner");

                entity.HasOne(d => d.Time)
                    .WithMany(p => p.Tickets)
                    .HasForeignKey(d => d.TimeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Ticket_ToTrainTime");
            });

            modelBuilder.Entity<TicketType>(entity =>
            {
                entity.ToTable("TicketType");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Train>(entity =>
            {
                entity.ToTable("Train");

                entity.Property(e => e.DestStation)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.OriginStation)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.TravelTime).HasColumnName("TravelTIme");
            });

            modelBuilder.Entity<TrainTime>(entity =>
            {
                entity.ToTable("TrainTime");

                entity.HasOne(d => d.Day)
                    .WithMany(p => p.TrainTimes)
                    .HasForeignKey(d => d.DayId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrainTime_ToDay");

                entity.HasOne(d => d.TicketType)
                    .WithMany(p => p.TrainTimes)
                    .HasForeignKey(d => d.TicketTypeId)
                    .HasConstraintName("FK_TrainTime_ToTicketType");

                entity.HasOne(d => d.Train)
                    .WithMany(p => p.TrainTimes)
                    .HasForeignKey(d => d.TrainId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrainTime_ToTrain");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
