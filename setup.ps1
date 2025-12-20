# Spor Salonu Yönetim Sistemi - Otomatik Kurulum Scripti
# Windows PowerShell için

Write-Host "🚀 Spor Salonu Yönetim Sistemi - Kurulum Başlatılıyor..." -ForegroundColor Cyan
Write-Host ""

# .NET SDK kontrolü
Write-Host "📦 .NET 8.0 SDK kontrol ediliyor..." -ForegroundColor Yellow
try {
    $dotnetVersion = dotnet --version
    Write-Host "✓ .NET SDK kurulu: $dotnetVersion" -ForegroundColor Green
} catch {
    Write-Host "✗ .NET SDK bulunamadı!" -ForegroundColor Red
    Write-Host "Lütfen .NET 8.0 SDK'yı indirin ve kurun:" -ForegroundColor Yellow
    Write-Host "https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Cyan
    exit 1
}

# PostgreSQL kontrolü
Write-Host ""
Write-Host "🐘 PostgreSQL kontrol ediliyor..." -ForegroundColor Yellow
$pgPath = Get-Command psql -ErrorAction SilentlyContinue
if ($pgPath) {
    Write-Host "✓ PostgreSQL kurulu" -ForegroundColor Green
} else {
    Write-Host "✗ PostgreSQL bulunamadı!" -ForegroundColor Red
    Write-Host "Lütfen PostgreSQL'i indirin ve kurun:" -ForegroundColor Yellow
    Write-Host "https://www.postgresql.org/download/windows/" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Kurulumdan sonra bu scripti tekrar çalıştırın." -ForegroundColor Yellow
    exit 1
}

# PostgreSQL servisini başlat
Write-Host ""
Write-Host "🔄 PostgreSQL servisi kontrol ediliyor..." -ForegroundColor Yellow
$pgService = Get-Service -Name "postgresql*" -ErrorAction SilentlyContinue
if ($pgService) {
    if ($pgService.Status -ne "Running") {
        Start-Service $pgService.Name
        Write-Host "✓ PostgreSQL servisi başlatıldı" -ForegroundColor Green
    } else {
        Write-Host "✓ PostgreSQL servisi çalışıyor" -ForegroundColor Green
    }
} else {
    Write-Host "⚠ PostgreSQL servisi bulunamadı" -ForegroundColor Yellow
}

# Veritabanı oluştur
Write-Host ""
Write-Host "💾 Veritabanı oluşturuluyor..." -ForegroundColor Yellow
Write-Host "PostgreSQL şifrenizi girmeniz gerekebilir..." -ForegroundColor Yellow
try {
    # Şifre olmadan deneme (trust authentication)
    $result = psql -U postgres -c "CREATE DATABASE `"SporSalonuYonetimDb`";" 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Veritabanı oluşturuldu" -ForegroundColor Green
    } else {
        Write-Host "⚠ Şifre gerekiyor veya veritabanı zaten mevcut" -ForegroundColor Yellow
        Write-Host "Manuel olarak oluşturmak için:" -ForegroundColor Yellow
        Write-Host "   psql -U postgres -c `"CREATE DATABASE SporSalonuYonetimDb;`"" -ForegroundColor Cyan
        Write-Host "   Veya pgAdmin kullanarak oluşturun" -ForegroundColor Cyan
    }
} catch {
    Write-Host "⚠ Veritabanı oluşturulamadı. Manuel olarak oluşturun." -ForegroundColor Yellow
}

# .NET paketlerini restore et
Write-Host ""
Write-Host "📚 .NET paketleri yükleniyor..." -ForegroundColor Yellow
dotnet restore
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Paketler yüklendi" -ForegroundColor Green
} else {
    Write-Host "✗ Paket yükleme hatası" -ForegroundColor Red
    exit 1
}

# Connection string kontrolü
Write-Host ""
Write-Host "⚙️  Connection string kontrol ediliyor..." -ForegroundColor Yellow
$appsettings = Get-Content appsettings.json -Raw
if ($appsettings -match "Host=localhost") {
    Write-Host "✓ Connection string ayarlı" -ForegroundColor Green
    Write-Host "⚠ Şifrenizi appsettings.json'da güncelleyin!" -ForegroundColor Yellow
} else {
    Write-Host "⚠ Connection string'i kontrol edin: appsettings.json" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "✅ Kurulum tamamlandı!" -ForegroundColor Green
Write-Host ""
Write-Host "🚀 Projeyi çalıştırmak için:" -ForegroundColor Cyan
Write-Host "   dotnet run" -ForegroundColor White
Write-Host ""
Write-Host "🌐 Tarayıcıda açın: http://localhost:5001" -ForegroundColor Cyan
Write-Host ""
Write-Host "👤 Admin girişi:" -ForegroundColor Cyan
Write-Host "   Email: G221210370@sakarya.edu.tr" -ForegroundColor White
Write-Host "   Şifre: sau" -ForegroundColor White
Write-Host ""
