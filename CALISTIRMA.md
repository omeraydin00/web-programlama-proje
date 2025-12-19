# Projeyi Çalıştırma Rehberi

## Mac'te Projeyi Çalıştırma

### 1. .NET 8.0 SDK Kurulumu

Proje .NET 8.0 gerektiriyor. Kurulum için:

**Yöntem 1: Homebrew (Önerilen)**
```bash
brew install --cask dotnet-sdk
```

**Yöntem 2: Manuel İndirme**
- https://dotnet.microsoft.com/download/dotnet/8.0 adresine gidin
- "macOS" için "SDK 8.0.x" indirin
- İndirilen .pkg dosyasını çalıştırın

**Kurulumu Kontrol Edin:**
```bash
dotnet --version
# Çıktı: 8.0.x olmalı
```

### 2. Ollama Kontrolü (AI Özelliği İçin)

Ollama'nın çalıştığından emin olun:

```bash
# Ollama servisini kontrol et
brew services list | grep ollama

# Eğer çalışmıyorsa başlat
brew services start ollama

# Modelin indirildiğini kontrol et
ollama list
# llama3.2 görünmeli
```

### 3. Projeyi Çalıştırma

**Terminal'de proje klasörüne gidin:**
```bash
cd /Users/omer/Documents/GitHub/web-programlama-proje
```

**Bağımlılıkları yükleyin:**
```bash
dotnet restore
```

**Projeyi çalıştırın:**
```bash
dotnet run
```

**Veya belirli bir port ile:**
```bash
dotnet run --urls "http://localhost:5000"
```

### 4. Tarayıcıda Açın

Terminal'de şu mesajı göreceksiniz:
```
Now listening on: http://localhost:5000
```

Tarayıcıda şu adresi açın:
- **http://localhost:5000**
- veya **https://localhost:5001** (HTTPS varsa)

### 5. İlk Giriş

**Admin Girişi:**
- Email: `G221210370@sakarya.edu.tr`
- Şifre: `sau`

**Not:** İlk çalıştırmada:
- Veritabanı otomatik oluşturulur
- Admin kullanıcısı otomatik oluşturulur
- Örnek veriler (spor salonu, antrenör, hizmet) eklenir

## Hızlı Başlangıç (Tüm Komutlar)

```bash
# 1. Proje klasörüne git
cd /Users/omer/Documents/GitHub/web-programlama-proje

# 2. .NET 8.0 SDK kurulu mu kontrol et
dotnet --version

# 3. Ollama çalışıyor mu kontrol et
curl http://localhost:11434/api/tags

# 4. Bağımlılıkları yükle
dotnet restore

# 5. Projeyi çalıştır
dotnet run
```

## Sorun Giderme

### .NET 8.0 SDK Bulunamadı Hatası

```
error NETSDK1045: The current .NET SDK does not support targeting .NET 8.0
```

**Çözüm:** .NET 8.0 SDK'yı kurun (yukarıdaki adım 1)

### Ollama Bağlantı Hatası

AI özelliği çalışmıyorsa:
```bash
# Ollama'yı başlat
brew services start ollama

# Modeli kontrol et
ollama list
```

### Port Kullanımda Hatası

Farklı bir port kullanın:
```bash
dotnet run --urls "http://localhost:5001"
```

### Veritabanı Hatası

Veritabanı dosyasını silip yeniden oluşturun:
```bash
rm SporSalonuYonetimDb.db*
dotnet run
```

## Projeyi Durdurma

Terminal'de `Ctrl + C` tuşlarına basın.

## Visual Studio Code ile Çalıştırma

1. Proje klasörünü VS Code'da açın
2. F5 tuşuna basın veya "Run and Debug" menüsünden başlatın
3. Tarayıcı otomatik açılacaktır

## Visual Studio 2022 ile Çalıştırma

1. `.sln` dosyasını açın (varsa)
2. F5 tuşuna basın veya "Start" butonuna tıklayın

