using Microsoft.EntityFrameworkCore;

namespace MvcCore.Data
{
    public partial class mvccoreContext : DbContext
    {
        public mvccoreContext()
        {
        }

        public mvccoreContext(DbContextOptions<mvccoreContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Blog> Blogs { get; set; }
        public virtual DbSet<Category> Categories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>(entity =>
            {
                entity.ToTable("blog");

                entity.HasIndex(e => e.CategoryId)
                    .HasName("fkCategoryID");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CategoryId)
                    .HasColumnName("CategoryID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.InsertDate).HasColumnType("datetime");

                entity.Property(e => e.MaxDate).HasColumnType("datetime");

                entity.Property(e => e.MaxPostCount).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(30)");

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Blog)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("blog_ibfk_1");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("category");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)");
            });
        }
    }
}
