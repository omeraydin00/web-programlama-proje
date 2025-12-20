# Spor Salonu Yonetim Sistemi - Otomatik Kurulum Scripti
# Windows PowerShell icin

Write-Host "Spor Salonu Yonetim Sistemi - Kurulum Baslatiliyor..." -ForegroundColor Cyan
Write-Host ""

# .NET SDK kontrolu
Write-Host ".NET 8.0 SDK kontrol ediliyor..." -ForegroundColor Yellow
try {
    $dotnetVersion = dotnet --version
    Write-Host "[OK] .NET SDK kurulu: $dotnetVersion" -ForegroundColor Green
} catch {
    Write-Host "[HATA] .NET SDK bulunamadi!" -ForegroundColor Red
    Write-Host "Lutfen .NET 8.0 SDK'yi indirin ve kurun:" -ForegroundColor Yellow
    Write-Host "https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Cyan
    exit 1
}

# PostgreSQL kontrolu
Write-Host ""
Write-Host "PostgreSQL kontrol ediliyor..." -ForegroundColor Yellow
$pgPath = Get-Command psql -ErrorAction SilentlyContinue
if ($pgPath) {
    Write-Host "[OK] PostgreSQL kurulu" -ForegroundColor Green
} else {
    Write-Host "[HATA] PostgreSQL bulunamadi!" -ForegroundColor Red
    Write-Host "Lutfen PostgreSQL'i indirin ve kurun:" -ForegroundColor Yellow
    Write-Host "https://www.postgresql.org/download/windows/" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Kurulumdan sonra bu scripti tekrar calistirin." -ForegroundColor Yellow
    exit 1
}

# PostgreSQL servisini baslat
Write-Host ""
Write-Host "PostgreSQL servisi kontrol ediliyor..." -ForegroundColor Yellow
$pgService = Get-Service -Name "postgresql*" -ErrorAction SilentlyContinue
if ($pgService) {
    if ($pgService.Status -ne "Running") {
        Start-Service $pgService.Name
        Write-Host "[OK] PostgreSQL servisi baslatildi" -ForegroundColor Green
    } else {
        Write-Host "[OK] PostgreSQL servisi calisiyor" -ForegroundColor Green
    }
} else {
    Write-Host "[UYARI] PostgreSQL servisi bulunamadi" -ForegroundColor Yellow
}

# Veritabani olustur
Write-Host ""
Write-Host "Veritabani olusturuluyor..." -ForegroundColor Yellow
Write-Host "PostgreSQL sifrenizi girmeniz gerekebilir..." -ForegroundColor Yellow
try {
    $result = psql -U postgres -c "CREATE DATABASE `"SporSalonuYonetimDb`";" 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "[OK] Veritabani olusturuldu" -ForegroundColor Green
    } else {
        Write-Host "[UYARI] Sifre gerekiyor veya veritabani zaten mevcut" -ForegroundColor Yellow
        Write-Host "Manuel olarak olusturmak icin:" -ForegroundColor Yellow
        Write-Host "   psql -U postgres -c `"CREATE DATABASE SporSalonuYonetimDb;`"" -ForegroundColor Cyan
        Write-Host "   Veya pgAdmin kullanarak olusturun" -ForegroundColor Cyan
    }
} catch {
    Write-Host "[UYARI] Veritabani olusturulamadi. Manuel olarak olusturun." -ForegroundColor Yellow
}

# .NET paketlerini restore et
Write-Host ""
Write-Host ".NET paketleri yukleniyor..." -ForegroundColor Yellow
dotnet restore
if ($LASTEXITCODE -eq 0) {
    Write-Host "[OK] Paketler yuklendi" -ForegroundColor Green
} else {
    Write-Host "[HATA] Paket yukleme hatasi" -ForegroundColor Red
    exit 1
}

# Connection string otomatik ayarla
Write-Host ""
Write-Host "Connection string ayarlaniyor..." -ForegroundColor Yellow
$appsettings = Get-Content appsettings.json -Raw
if ($appsettings -match "Host=localhost") {
    Write-Host "[OK] Connection string ayarli" -ForegroundColor Green
    Write-Host "[UYARI] Windows'ta PostgreSQL sifrenizi appsettings.json'da guncelleyin!" -ForegroundColor Yellow
    Write-Host "   Password=your_password kismini duzenleyin" -ForegroundColor Cyan
} else {
    Write-Host "[UYARI] Connection string'i kontrol edin: appsettings.json" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "[TAMAMLANDI] Kurulum tamamlandi!" -ForegroundColor Green
Write-Host ""
Write-Host "Projeyi calistirmak icin:" -ForegroundColor Cyan
Write-Host "   dotnet run" -ForegroundColor White
Write-Host ""
Write-Host "Tarayicida acin: http://localhost:5001" -ForegroundColor Cyan
Write-Host ""
Write-Host "Admin girisi:" -ForegroundColor Cyan
Write-Host "   Email: G221210370@sakarya.edu.tr" -ForegroundColor White
Write-Host "   Sifre: sau" -ForegroundColor White
Write-Host ""
