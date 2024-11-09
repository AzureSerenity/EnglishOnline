using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using DoAnCoSo.Models;

namespace DoAnCoSo.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<CauHoi> CauHois { get; set; }
        public DbSet<BaiThi> BaiThis { get; set; }
        public DbSet<KetQua> KetQuas { get; set; }
        public DbSet<BaiThi_CauHoi> BaiThi_CauHois { get; set; }
        public DbSet<LichSuLamBai> LichSuLamBais { get; set; }
        public DbSet<ChangePassword> ChangePassword { get; set; }
        public DbSet<Answer> Answers { get; set; }
        //public DbSet<UserAnswer> UserAnswers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<ApplicationUser>()
            //    .HasMany(u => u.UserAnswers)
            //    .WithOne(ua => ua.User)
            //    .HasForeignKey(ua => ua.UserId);

            //modelBuilder.Entity<UserAnswer>()
            //    .HasOne(ua => ua.CauHoi)
            //    .WithMany()
            //    .HasForeignKey(ua => ua.CauHoiId)
            //    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BaiThi>()
                .HasMany(b => b.BaiThi_CauHois)
                .WithOne(bth => bth.BaiThi)
                .HasForeignKey(bth => bth.BaiThiId);

            modelBuilder.Entity<BaiThi_CauHoi>()
                .HasKey(bth => new { bth.BaiThiId, bth.CauHoiId });

            modelBuilder.Entity<BaiThi_CauHoi>()
                .HasOne(bth => bth.CauHoi)
                .WithMany(c => c.BaiThi_CauHois)
                .HasForeignKey(bth => bth.CauHoiId);

            //modelBuilder.Entity<CauHoi>()
            //    .HasMany(c => c.UserAnswers)
            //    .WithOne(ua => ua.CauHoi)
            //    .HasForeignKey(ua => ua.CauHoiId);

            //modelBuilder.Entity<UserAnswer>()
            //    .HasOne(ua => ua.Answer)
            //    .WithMany(a => a.UserAnswers)
            //    .HasForeignKey(ua => ua.AnswerId);

            //modelBuilder.Entity<UserAnswer>()
            //    .HasOne(ua => ua.User)
            //    .WithMany(u => u.UserAnswers)
            //    .HasForeignKey(ua => ua.UserId);

            //modelBuilder.Entity<UserAnswer>()
            //    .HasOne(ua => ua.CauHoi)
            //    .WithMany(c => c.UserAnswers)
            //    .HasForeignKey(ua => ua.CauHoiId);
        }
    }
}
