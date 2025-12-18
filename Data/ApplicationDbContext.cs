using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SporSalonuYonetim.Models;

namespace SporSalonuYonetim.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<SporSalonu> SporSalonlari { get; set; }
    public DbSet<Antrenor> Antrenorler { get; set; }
    public DbSet<Hizmet> Hizmetler { get; set; }
    public DbSet<Randevu> Randevular { get; set; }
    public DbSet<AntrenorMüsaitlik> AntrenorMüsaitlikler { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // SporSalonu yapılandırması
        builder.Entity<SporSalonu>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Ad).IsRequired().HasMaxLength(200);
        });

        // Antrenor yapılandırması
        builder.Entity<Antrenor>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Ad).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Soyad).IsRequired().HasMaxLength(100);
            entity.HasOne(e => e.SporSalonu)
                  .WithMany(s => s.Antrenorler)
                  .HasForeignKey(e => e.SporSalonuId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Hizmet yapılandırması
        builder.Entity<Hizmet>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Ad).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Ucret).HasPrecision(18, 2);
            entity.HasOne(e => e.SporSalonu)
                  .WithMany(s => s.Hizmetler)
                  .HasForeignKey(e => e.SporSalonuId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Randevu yapılandırması
        builder.Entity<Randevu>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Durum).HasMaxLength(50);
            entity.Property(e => e.ToplamUcret).HasPrecision(18, 2);
            entity.HasOne(e => e.Uye)
                  .WithMany(u => u.Randevular)
                  .HasForeignKey(e => e.UyeId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Antrenor)
                  .WithMany(a => a.Randevular)
                  .HasForeignKey(e => e.AntrenorId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Hizmet)
                  .WithMany(h => h.Randevular)
                  .HasForeignKey(e => e.HizmetId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // AntrenorMüsaitlik yapılandırması
        builder.Entity<AntrenorMüsaitlik>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Antrenor)
                  .WithMany(a => a.Müsaitlikler)
                  .HasForeignKey(e => e.AntrenorId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}










