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
        await context.Database.MigrateAsync();

        // Rolleri oluştur
        await SeedRolesAsync(roleManager);

        // Admin kullanıcısını oluştur
        await SeedAdminUserAsync(userManager);

        // Örnek verileri ekle
        await SeedSampleDataAsync(context);
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
        // Örnek: 123456789@sakarya.edu.tr
        string adminEmail = "123456789@sakarya.edu.tr";
        string adminPassword = "sau";

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

    private static async Task SeedSampleDataAsync(ApplicationDbContext context)
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
    }
}

