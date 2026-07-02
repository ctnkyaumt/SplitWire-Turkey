#!/bin/sh

# Zapret Otomatik Kurulum için özel blog-hidden.sh (v2)
# Bu script log dosyası oluşturur ve işlem tamamlanana kadar bekler

EXEDIR="$(dirname "$0")"
EXEDIR="$(cd "$EXEDIR"; pwd)"

# Otomatik ve sessiz çalışma parametreleri
export BATCH=1
export HIDDEN_MODE=1
export DOMAINS=${DOMAINS:-"roblox.com"}
export IPVS=${IPVS:-"4"}
export ENABLE_HTTP=${ENABLE_HTTP:-"0"}
export ENABLE_HTTPS_TLS12=${ENABLE_HTTPS_TLS12:-"1"}
export ENABLE_HTTPS_TLS13=${ENABLE_HTTPS_TLS13:-"0"}
export ENABLE_HTTP3=${ENABLE_HTTP3:-"0"}
export REPEATS=${REPEATS:-"1"}
export PARALLEL=${PARALLEL:-"0"}
export SCANLEVEL=${SCANLEVEL:-"quick"}

# Blockcheck v2'yi çalıştır ve çıktıyı log dosyasına yönlendir
"$EXEDIR/blockcheck2.sh" 2>&1 | tee "$EXEDIR/../blockcheck.log"

# İşlem tamamlanana kadar bekle
wait $!

# Windows 7 notepad does not view unix EOL correctly
unix2dos "$EXEDIR/../blockcheck.log" 2>/dev/null || true

# Log dosyasının oluştuğunu doğrula
if [ -f "$EXEDIR/../blockcheck.log" ]; then
    echo "Log dosyası başarıyla oluşturuldu: $EXEDIR/../blockcheck.log"
else
    echo "HATA: Log dosyası oluşturulamadı!"
    exit 1
fi
