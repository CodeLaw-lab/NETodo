using Microsoft.EntityFrameworkCore;
using TodoApp.WPF.Core.Entities;

namespace TodoApp.WPF.Infrastructure.Data
{
   public class AppDbContext : DbContext
   {
      public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
      {
      }

      public DbSet<TodoTask> Tasks { get; set; }
      public DbSet<Category> Categories { get; set; }

      protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
      {
         if (!optionsBuilder.IsConfigured)
         {
            optionsBuilder.UseSqlite("Data Source=todoapp.db");
         }
      }

      protected override void OnModelCreating(ModelBuilder modelBuilder)
      {
         base.OnModelCreating(modelBuilder);

         // Конфигурация TodoTask
         modelBuilder.Entity<TodoTask>(entity =>
         {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.Priority).HasConversion<int>();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.ModifiedAt).IsRequired();

            entity.HasQueryFilter(e => !e.IsDeleted);

            // Связь с Category
            entity.HasOne(e => e.Category)
                .WithMany(c => c.Tasks)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
         });

         // Конфигурация Category
         modelBuilder.Entity<Category>(entity =>
         {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Color).IsRequired().HasMaxLength(7);
            entity.Property(e => e.Icon).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.ModifiedAt).IsRequired();

            entity.HasQueryFilter(e => !e.IsDeleted);

            // Уникальность имени категории
            entity.HasIndex(e => e.Name).IsUnique();
         });
      }
   }
}