# Spor Salonu Yönetim ve Randevu Sistemi

Web Programlama dersi için geliştirilen spor salonu yönetim ve randevu sistemi projesi.

## Kurulum

**macOS/Linux:**
```bash
chmod +x setup.sh && ./setup.sh
```

**Windows (PowerShell):**
```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser; .\setup.ps1
```

**Projeyi Çalıştırma:**
```bash
dotnet run
```

Tarayıcıda açın: **http://localhost:5001**

## Admin Girişi

* Email: `G221210370@sakarya.edu.tr`
* Şifre: `sau`

## Özellikler

* ✅ CRUD İşlemleri (SporSalonu, Antrenor, Hizmet, Randevu)
* ✅ REST API (LINQ filtreleme ile)
* ✅ Yapay Zeka Entegrasyonu (Ollama)
* ✅ Admin Paneli
* ✅ Rol Bazlı Yetkilendirme (Admin, Üye)
* ✅ Randevu Sistemi (Çakışma kontrolü, Onay mekanizması)
* ✅ Data Validation (Client + Server)
* ✅ Bootstrap 5, jQuery

## Teknolojiler

* ASP.NET Core MVC (.NET 8.0)
* PostgreSQL
* Entity Framework Core
* Bootstrap 5
* jQuery
* Ollama (AI)
