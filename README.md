# Spor Salonu Yönetim ve Randevu Sistemi

Web Programlama dersi için geliştirilen spor salonu yönetim ve randevu sistemi projesi.

## Proje Hakkında

Bu projede **ASP.NET Core MVC** kullanılarak bir spor salonu yönetim sistemi geliştirilmiştir. Sistem; spor salonlarının sunduğu hizmetleri, antrenörlerin uzmanlık alanlarını, üyelerin randevularını ve **yerel yapay zekâ (LLM) tabanlı egzersiz önerilerini (Ollama)** yönetebilecek şekilde tasarlanmıştır.

Bulut tabanlı API’ler yerine **Ollama** kullanılarak, yapay zekâ özellikleri **lokal ortamda**, internet bağımlılığı olmadan çalışmaktadır.

## Özellikler

### ✅ Tamamlanan Özellikler

* Front-End (arayüz) - Bootstrap 5 ile modern ve responsive tasarım
* CRUD İşlemleri - Tüm temel varlıklar için (SporSalonu, Antrenor, Hizmet, Randevu)
* Veri Doğrulama - Hem istemci hem sunucu tarafında (jQuery Validation, Data Annotations)
* Admin Paneli - Tüm yönetim işlemleri için ayrı panel
* Kullanıcı Kayıt/Üye Olma - Identity ile güvenli kayıt sistemi
* Rol Bazlı Yetkilendirme - Admin ve Üye rolleri
* REST API - LINQ sorguları ile filtreleme desteği
* Yapay Zeka Entegrasyonu - Ollama ile lokal AI egzersiz önerileri

## Kurulum

### Gereksinimler

* .NET 8.0 SDK veya üzeri
* SQLite (Entity Framework Core ile otomatik yönetilir)
* Ollama (AI özellikleri için - opsiyonel)

### Ollama Kurulumu

1. Ollama’yı indirin ve kurun: [https://ollama.com](https://ollama.com)
2. Bir model indirin (örnek):

   ```bash
   ollama pull llama3
   ```
3. Ollama servisinin çalıştığından emin olun:

   ```bash
   ollama run llama3
   ```

### Proje Kurulumu

1. Projeyi klonlayın veya indirin
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```
5. Projeyi çalıştırın:

   ```bash
   dotnet run
   ```

**Not:** Ollama kurulu değilse, AI egzersiz önerileri özelliği çalışmayacaktır. Diğer tüm özellikler normal çalışır.

## Kullanım

### Admin Girişi

* Email: `G221210370@sakarya.edu.tr`
* Şifre: `sau`

**Not:** Admin kullanıcısı ilk çalıştırmada otomatik oluşturulur.
### Kullanıcı Rolleri

* **Admin**: Tüm CRUD işlemleri, randevu onaylama/reddetme
* **Üye**: Randevu alma, kendi randevularını görüntüleme, AI destekli egzersiz önerileri

## API Endpoints

### Antrenör API

* `GET /api/AntrenorApi` – Tüm antrenörleri listele
  *(Filtreler: sporSalonuId, uzmanlikAlani, tarih, saat)*
* `GET /api/AntrenorApi/{id}` – Belirli bir antrenörü getir

### Randevu API

* `GET /api/RandevuApi` – Randevuları listele
  *(Filtreler: uyeId, antrenorId, baslangicTarihi, bitisTarihi, durum)*
* `GET /api/RandevuApi/{id}` – Belirli bir randevuyu getir

## Proje Yapısı

```
SporSalonuYonetim/
├── Controllers/
│   ├── Admin/          # Admin paneli controller'ları
│   ├── Api/            # REST API controller'ları
│   ├── AccountController.cs
│   ├── HomeController.cs
│   ├── RandevuController.cs
│   └── AIEgzersizController.cs
├── Models/             # Veritabanı modelleri ve ViewModel'ler
├── Views/              # Razor view'ları
├── Data/               # DbContext ve DbInitializer
├── Services/           # Business logic servisleri (Ollama AI)
└── wwwroot/            # Statik dosyalar (CSS, JS, lib)
```

## Veritabanı Modelleri

* **ApplicationUser**: Kullanıcı bilgileri (Identity genişletilmiş)
* **SporSalonu**: Spor salonu bilgileri
* **Antrenor**: Antrenör bilgileri
* **Hizmet**: Hizmet türleri ve fiyatları
* **Randevu**: Randevu bilgileri
* **AntrenorMusaitlik**: Antrenör müsaitlik saatleri

## Geliştirme Notları

* Proje ilk çalıştırmada örnek veriler (seed data) otomatik oluşturulur
* Tüm CRUD işlemleri için validation kuralları uygulanmıştır
* Client-side ve server-side validation aktiftir
* Bootstrap 5 ile responsive tasarım
* LINQ sorguları ile gelişmiş filtreleme
* Yapay zekâ işlemleri **Ollama üzerinden lokal olarak** gerçekleştirilmektedir

## Lisans

Bu proje eğitim amaçlı geliştirilmiştir.

## İletişim

Proje hakkında sorularınız için GitHub üzerinden iletişime geçebilirsiniz.
