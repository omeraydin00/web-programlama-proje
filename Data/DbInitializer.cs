using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SporSalonuYonetim.Models;

namespace SporSalonuYonetim.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Veritabanını oluştur
        await context.Database.EnsureCreatedAsync();

        // Rolleri oluştur
        await SeedRolesAsync(roleManager);

        // Admin kullanıcısını oluştur
        await SeedAdminUserAsync(userManager);

        // Örnek verileri ekle
        await SeedSampleDataAsync(context, userManager);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = { "Admin", "Üye" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
    {
        // Admin kullanıcısı için öğrenci numarası formatında email
        // Örnek: G221210370@sakarya.edu.tr
        string adminEmail = "G221210370@sakarya.edu.tr";
        string adminPassword = "sau"; // Gereksinimlere uygun şifre

        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                Ad = "Admin",
                Soyad = "Kullanıcı"
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }

    private static async Task SeedSampleDataAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        // Örnek spor salonu ekle
        if (!context.SporSalonlari.Any())
        {
            var sporSalonu = new SporSalonu
            {
                Ad = "FitZone Spor Salonu",
                Adres = "Sakarya Üniversitesi Kampüsü",
                Telefon = "0264 123 45 67",
                Email = "info@fitzone.com",
                AcilisSaati = new TimeSpan(6, 0, 0),
                KapanisSaati = new TimeSpan(23, 0, 0)
            };

            context.SporSalonlari.Add(sporSalonu);
            await context.SaveChangesAsync();

            // Örnek hizmetler ekle
            var hizmetler = new List<Hizmet>
            {
                new Hizmet
                {
                    SporSalonuId = sporSalonu.Id,
                    Ad = "Fitness Antrenmanı",
                    Aciklama = "Kişisel fitness antrenmanı",
                    Sure = 60,
                    Ucret = 200.00m,
                    HizmetTuru = "Fitness"
                },
                new Hizmet
                {
                    SporSalonuId = sporSalonu.Id,
                    Ad = "Yoga Dersi",
                    Aciklama = "Hatha yoga dersi",
                    Sure = 90,
                    Ucret = 150.00m,
                    HizmetTuru = "Yoga"
                },
                new Hizmet
                {
                    SporSalonuId = sporSalonu.Id,
                    Ad = "Pilates Dersi",
                    Aciklama = "Temel pilates antrenmanı",
                    Sure = 60,
                    Ucret = 180.00m,
                    HizmetTuru = "Pilates"
                }
            };

            context.Hizmetler.AddRange(hizmetler);
            await context.SaveChangesAsync();

            // Örnek antrenörler ekle
            var antrenorler = new List<Antrenor>
            {
                new Antrenor
                {
                    SporSalonuId = sporSalonu.Id,
                    Ad = "Ahmet",
                    Soyad = "Yılmaz",
                    Email = "ahmet@fitzone.com",
                    Telefon = "0532 111 22 33",
                    UzmanlikAlanlari = "Kas geliştirme, Kilo verme",
                    Aciklama = "10 yıllık deneyimli fitness antrenörü"
                },
                new Antrenor
                {
                    SporSalonuId = sporSalonu.Id,
                    Ad = "Ayşe",
                    Soyad = "Demir",
                    Email = "ayse@fitzone.com",
                    Telefon = "0532 222 33 44",
                    UzmanlikAlanlari = "Yoga, Pilates",
                    Aciklama = "Sertifikalı yoga ve pilates eğitmeni"
                }
            };

            context.Antrenorler.AddRange(antrenorler);
            await context.SaveChangesAsync();

            // Antrenör müsaitlikleri ekle
            var antrenor1 = antrenorler[0];
            var antrenor2 = antrenorler[1];

            var musaitlikler = new List<AntrenorMüsaitlik>
            {
                // Ahmet Yılmaz - Pazartesi-Cuma 09:00-17:00
                new AntrenorMüsaitlik { AntrenorId = antrenor1.Id, HaftaninGunu = DayOfWeek.Monday, BaslangicSaati = new TimeSpan(9, 0, 0), BitisSaati = new TimeSpan(17, 0, 0) },
                new AntrenorMüsaitlik { AntrenorId = antrenor1.Id, HaftaninGunu = DayOfWeek.Tuesday, BaslangicSaati = new TimeSpan(9, 0, 0), BitisSaati = new TimeSpan(17, 0, 0) },
                new AntrenorMüsaitlik { AntrenorId = antrenor1.Id, HaftaninGunu = DayOfWeek.Wednesday, BaslangicSaati = new TimeSpan(9, 0, 0), BitisSaati = new TimeSpan(17, 0, 0) },
                new AntrenorMüsaitlik { AntrenorId = antrenor1.Id, HaftaninGunu = DayOfWeek.Thursday, BaslangicSaati = new TimeSpan(9, 0, 0), BitisSaati = new TimeSpan(17, 0, 0) },
                new AntrenorMüsaitlik { AntrenorId = antrenor1.Id, HaftaninGunu = DayOfWeek.Friday, BaslangicSaati = new TimeSpan(9, 0, 0), BitisSaati = new TimeSpan(17, 0, 0) },
                
                // Ayşe Demir - Pazartesi-Cuma 10:00-18:00
                new AntrenorMüsaitlik { AntrenorId = antrenor2.Id, HaftaninGunu = DayOfWeek.Monday, BaslangicSaati = new TimeSpan(10, 0, 0), BitisSaati = new TimeSpan(18, 0, 0) },
                new AntrenorMüsaitlik { AntrenorId = antrenor2.Id, HaftaninGunu = DayOfWeek.Tuesday, BaslangicSaati = new TimeSpan(10, 0, 0), BitisSaati = new TimeSpan(18, 0, 0) },
                new AntrenorMüsaitlik { AntrenorId = antrenor2.Id, HaftaninGunu = DayOfWeek.Wednesday, BaslangicSaati = new TimeSpan(10, 0, 0), BitisSaati = new TimeSpan(18, 0, 0) },
                new AntrenorMüsaitlik { AntrenorId = antrenor2.Id, HaftaninGunu = DayOfWeek.Thursday, BaslangicSaati = new TimeSpan(10, 0, 0), BitisSaati = new TimeSpan(18, 0, 0) },
                new AntrenorMüsaitlik { AntrenorId = antrenor2.Id, HaftaninGunu = DayOfWeek.Friday, BaslangicSaati = new TimeSpan(10, 0, 0), BitisSaati = new TimeSpan(18, 0, 0) }
            };

            context.AntrenorMüsaitlikler.AddRange(musaitlikler);
            await context.SaveChangesAsync();
        }

        // Örnek üye kullanıcısı oluştur (randevular için)
        string uyeEmail = "uye@sakarya.edu.tr";
        var uyeUser = await userManager.FindByEmailAsync(uyeEmail);

        if (uyeUser == null)
        {
            uyeUser = new ApplicationUser
            {
                UserName = uyeEmail,
                Email = uyeEmail,
                EmailConfirmed = true,
                Ad = "Test",
                Soyad = "Üye"
            };
            var result = await userManager.CreateAsync(uyeUser, "sau");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(uyeUser, "Üye");
            }
        }

        // Örnek randevular ekle
        if (!context.Randevular.Any() && uyeUser != null && context.Antrenorler.Any() && context.Hizmetler.Any())
        {
            var antrenorler = await context.Antrenorler.ToListAsync();
            var hizmetler = await context.Hizmetler.ToListAsync();

            if (antrenorler.Any() && hizmetler.Any())
            {
                var randevular = new List<Randevu>
                {
                    new Randevu
                    {
                        UyeId = uyeUser.Id,
                        AntrenorId = antrenorler[0].Id,
                        HizmetId = hizmetler[0].Id, // Fitness Antrenmanı
                        RandevuTarihi = DateTime.UtcNow.Date.AddDays(1), // Yarın
                        BaslangicSaati = new TimeSpan(10, 0, 0),
                        BitisSaati = new TimeSpan(11, 0, 0),
                        ToplamUcret = hizmetler[0].Ucret,
                        Durum = "Beklemede",
                        OlusturmaTarihi = DateTime.UtcNow.AddDays(-2),
                        Notlar = "İlk randevu"
                    },
                    new Randevu
                    {
                        UyeId = uyeUser.Id,
                        AntrenorId = antrenorler.Count > 1 ? antrenorler[1].Id : antrenorler[0].Id,
                        HizmetId = hizmetler.Count > 1 ? hizmetler[1].Id : hizmetler[0].Id, // Yoga Dersi
                        RandevuTarihi = DateTime.UtcNow.Date.AddDays(3), // 3 gün sonra
                        BaslangicSaati = new TimeSpan(14, 0, 0),
                        BitisSaati = new TimeSpan(15, 30, 0),
                        ToplamUcret = hizmetler.Count > 1 ? hizmetler[1].Ucret : hizmetler[0].Ucret,
                        Durum = "Onaylandı",
                        OlusturmaTarihi = DateTime.UtcNow.AddDays(-5),
                        Notlar = "Yoga dersi randevusu"
                    },
                    new Randevu
                    {
                        UyeId = uyeUser.Id,
                        AntrenorId = antrenorler[0].Id,
                        HizmetId = hizmetler.Count > 2 ? hizmetler[2].Id : hizmetler[0].Id, // Pilates Dersi
                        RandevuTarihi = DateTime.UtcNow.Date.AddDays(-2), // 2 gün önce
                        BaslangicSaati = new TimeSpan(11, 0, 0),
                        BitisSaati = new TimeSpan(12, 0, 0),
                        ToplamUcret = hizmetler.Count > 2 ? hizmetler[2].Ucret : hizmetler[0].Ucret,
                        Durum = "Tamamlandı",
                        OlusturmaTarihi = DateTime.UtcNow.AddDays(-10),
                        Notlar = "Tamamlanmış pilates dersi"
                    }
                };

                context.Randevular.AddRange(randevular);
                await context.SaveChangesAsync();
            }
        }
    }
}


