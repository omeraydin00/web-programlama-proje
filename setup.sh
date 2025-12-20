#!/bin/bash

# Spor Salonu Yönetim Sistemi - Otomatik Kurulum Scripti
# macOS için

echo "🚀 Spor Salonu Yönetim Sistemi - Kurulum Başlatılıyor..."
echo ""

# Renkler
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

# .NET SDK kontrolü
echo "📦 .NET 8.0 SDK kontrol ediliyor..."
if ! command -v dotnet &> /dev/null; then
    echo -e "${YELLOW}.NET SDK bulunamadı. Kuruluyor...${NC}"
    if command -v brew &> /dev/null; then
        brew install --cask dotnet-sdk
    else
        echo -e "${RED}Homebrew bulunamadı. Lütfen önce Homebrew kurun: https://brew.sh${NC}"
        exit 1
    fi
else
    DOTNET_VERSION=$(dotnet --version)
    echo -e "${GREEN}✓ .NET SDK kurulu: $DOTNET_VERSION${NC}"
fi

# PostgreSQL kontrolü
echo ""
echo "🐘 PostgreSQL kontrol ediliyor..."
if ! command -v psql &> /dev/null; then
    echo -e "${YELLOW}PostgreSQL bulunamadı. Kuruluyor...${NC}"
    if command -v brew &> /dev/null; then
        brew install postgresql@14
        brew services start postgresql@14
        sleep 2
    else
        echo -e "${RED}Homebrew bulunamadı. Lütfen önce Homebrew kurun: https://brew.sh${NC}"
        exit 1
    fi
else
    echo -e "${GREEN}✓ PostgreSQL kurulu${NC}"
fi

# PostgreSQL servisini başlat
echo ""
echo "🔄 PostgreSQL servisi başlatılıyor..."
if command -v brew &> /dev/null; then
    brew services start postgresql@14 2>/dev/null || brew services restart postgresql@14
    sleep 2
fi

# Veritabanı oluştur
echo ""
echo "💾 Veritabanı oluşturuluyor..."
psql -d postgres -c "CREATE DATABASE \"SporSalonuYonetimDb\";" 2>/dev/null
if [ $? -eq 0 ]; then
    echo -e "${GREEN}✓ Veritabanı oluşturuldu${NC}"
else
    echo -e "${YELLOW}⚠ Veritabanı zaten mevcut veya oluşturulamadı (sorun değil)${NC}"
fi

# Ollama kontrolü (opsiyonel)
echo ""
echo "🤖 Ollama kontrol ediliyor (opsiyonel)..."
if ! command -v ollama &> /dev/null; then
    echo -e "${YELLOW}Ollama bulunamadı. AI özellikleri çalışmayacak.${NC}"
    echo -e "${YELLOW}Kurulum için: brew install ollama${NC}"
else
    echo -e "${GREEN}✓ Ollama kurulu${NC}"
    # Ollama servisini başlat
    ollama serve > /dev/null 2>&1 &
    sleep 1
fi

# .NET paketlerini restore et
echo ""
echo "📚 .NET paketleri yükleniyor..."
dotnet restore
if [ $? -eq 0 ]; then
    echo -e "${GREEN}✓ Paketler yüklendi${NC}"
else
    echo -e "${RED}✗ Paket yükleme hatası${NC}"
    exit 1
fi

# Connection string otomatik ayarla
echo ""
echo "⚙️  Connection string ayarlanıyor..."
CURRENT_USER=$(whoami)
# macOS'ta genellikle şifre gerektirmez
sed -i.bak "s/Username=.*/Username=$CURRENT_USER/" appsettings.json 2>/dev/null || \
sed -i '' "s/Username=.*/Username=$CURRENT_USER/" appsettings.json 2>/dev/null
if [ $? -eq 0 ]; then
    echo -e "${GREEN}✓ Connection string otomatik ayarlandı (Username: $CURRENT_USER)${NC}"
else
    echo -e "${YELLOW}⚠ Connection string'i manuel kontrol edin: appsettings.json${NC}"
fi

echo ""
echo -e "${GREEN}✅ Kurulum tamamlandı!${NC}"
echo ""
echo "🚀 Projeyi çalıştırmak için:"
echo "   dotnet run"
echo ""
echo "🌐 Tarayıcıda açın: http://localhost:5001"
echo ""
echo "👤 Admin girişi:"
echo "   Email: G221210370@sakarya.edu.tr"
echo "   Şifre: sau"
echo ""
