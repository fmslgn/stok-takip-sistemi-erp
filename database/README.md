# Veritabani Kurulumu

Bu klasorde PostgreSQL icin hazirlanan `database.sql` dosyasi bulunur. Dosya; tabloları, foreign key iliskilerini, check constraint kurallarini ve ornek verileri olusturur.

## Gereksinimler

- PostgreSQL kurulu olmalidir.
- Komutlarin calisabilmesi icin `createdb` ve `psql` araclari sistem PATH icinde olmalidir.

## Veritabanini Olusturma

Asagidaki komut yeni veritabanini olusturur:

```bash
createdb -U postgres stok_takip_db
```

## SQL Dosyasini Calistirma

Proje kok dizinindeyken asagidaki komut calistirilir:

```bash
psql -U postgres -d stok_takip_db -f database/database.sql
```

## Baglanti Ayari

Uygulama tarafinda PostgreSQL baglantisi `StokTakip.DataAccess/DbHelper.cs` dosyasindaki connection string uzerinden yonetilir. Yerel kullanici adi, sifre veya veritabani adi farkliysa bu alan guncellenmelidir.
