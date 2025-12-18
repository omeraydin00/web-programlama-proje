# Spor Salonu Yönetim ve Randevu Sistemi

Web Programlama dersi için geliştirilen spor salonu yönetim ve randevu sistemi projesi.

## Proje Hakkında

Bu projede **ASP.NET Core MVC** kullanılarak bir spor salonu yönetim sistemi geliştirilmiştir. Sistem; spor salonlarının sunduğu hizmetleri, antrenörlerin uzmanlık alanlarını, üyelerin randevularını ve **yerel yapay zekâ (LLM) tabanlı egzersiz önerilerini (Ollama)** yönetebilecek şekilde tasarlanmıştır.

Bulut tabanlı API’ler yerine **Ollama** kullanılarak, yapay zekâ özellikleri **lokal ortamda**, internet bağımlılığı olmadan çalışmaktadır.

## Özellikler

### ✅ Tamamlanan Özellikler

* **Spor Salonu Yönetimi**: Spor salonlarının tanımlanması, çalışma saatleri ve hizmet türleri
* **Antrenör Yönetimi**: Antrenörlerin uzmanlık alanları ve müsaitlik saatleri
* **Hizmet Yönetimi**: Hizmet türleri, süre ve ücret bilgileri
* **Randevu Sistemi**:

  * Randevu alma (müsaitlik ve çakışma kontrolü)
  * Randevu onaylama/reddetme (Admin)
  * Randevu durum takibi
* **Kullanıcı Yönetimi**:

  * Kullanıcı kayıt/giriş
  * Rol bazlı yetkilendirme (Admin, Üye)
  * Profil yönetimi
* **REST API**:

  * Antrenör sorgulama ve filtreleme (LINQ)
  * Randevu sorgulama ve filtreleme
* **Yapay Zeka Entegrasyonu (Ollama)**:

  * Yerel LLM kullanarak kişiselleştirilmiş egzersiz önerileri
  * İnternet bağlantısı gerektirmez
  * API key ihtiyacı yoktur
* **Admin Paneli**: Tüm CRUD işlemleri için yönetim paneli
* **Veri Doğrulama**: Client-side ve server-side validation

## Kullanılan Teknolojiler

* **ASP.NET Core MVC 8.0** (LTS)
* **C#**
* **SQL Server** (LocalDB)
* **Entity Framework Core 8.0**
* **ASP.NET Core Identity** (Kimlik doğrulama ve yetkilendirme)
* **Bootstrap 5** (Frontend framework)
* **jQuery & jQuery Validation** (Client-side validation)
* **Ollama** (Yerel yapay zekâ / LLM entegrasyonu)

## Kurulum

### Gereksinimler

* .NET 8.0 SDK
* SQL Server (LocalDB veya SQL Server Express)
* Visual Studio 2022 veya VS Code
* **Ollama (Local LLM Runtime)**

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
2. `appsettings.json` dosyasında veritabanı bağlantı string’ini kontrol edin
3. **Ollama için ekstra API key gerekmez**
4. Migration’ları oluşturun ve uygulayın:

   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```
5. Projeyi çalıştırın:

   ```bash
   dotnet run
   ```

## Kullanım

### Admin Girişi

* **Email**: `123456789@sakarya.edu.tr` (DbInitializer’da oluşturulur)
* **Şifre**: `sau`

**Not**: Admin kullanıcısı ilk çalıştırmada otomatik oluşturulur. Öğrenci numaranıza göre `DbInitializer.cs` dosyasından güncellenebilir.

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
