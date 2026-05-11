# StokTakipSistemi

## Proje Adi

StokTakipSistemi

## Proje Amaci

Bu proje, C# Windows Forms ve PostgreSQL kullanarak katmanli mimariye uygun bir ERP stok takip sistemi gelistirmek amaciyla hazirlanmistir. Sistem; kullanici yonetimi, kategori, urun, stok giris-cikis, kritik stok, raporlama ve saklama kosulu modullerini kapsar.

## Kullanilan Teknolojiler

- C#
- .NET 8
- Windows Forms
- PostgreSQL
- Npgsql
- Katmanli mimari

## Katmanli Mimari

Solution dort ana katmandan olusur:

- `StokTakip.Entities`: Veritabaninda tutulacak temel nesne siniflari.
- `StokTakip.DataAccess`: PostgreSQL baglantisi ve veritabani islemlerinin yazilacagi katman.
- `StokTakip.Business`: Is kurallari ve kontrollerin yer alacagi katman.
- `StokTakip.WinForms`: Kullanici arayuzu formlarinin bulundugu katman.

UI katmani dogrudan SQL sorgusu yazmaz. WinForms sadece Business katmanini cagirir, Business katmani DataAccess katmanini kullanir, PostgreSQL baglantisi ise DataAccess katmaninda yonetilir.

## Moduller

- Kullanici Girisi
- Ana Menu
- Kullanici Yonetimi
- Kategori Yonetimi
- Urun Yonetimi
- Stok Giris
- Stok Cikis
- Kritik Stok Takibi
- Raporlama
- Saklama Kosulu AI

## Veritabani

Veritabani olarak PostgreSQL kullanilacaktir. Baglanti islemleri `StokTakip.DataAccess` katmanindaki `DbHelper` sinifi uzerinden yapilacaktir. Projenin bu ilk asamasinda gercek CRUD sorgulari eklenmemistir.

Veritabani tablo yapisi ve ornek veriler `database/database.sql` dosyasinda bulunur. PostgreSQL tarafinda veritabanini olusturmak ve SQL dosyasini calistirmak icin:

```bash
createdb -U postgres stok_takip_db
psql -U postgres -d stok_takip_db -f database/database.sql
```

Detayli aciklama icin `database/README.md` dosyasina bakilabilir.

## Ekip Uyeleri

- Furkan Mehmet Salgin - 245611029
- Furkan Avci - 245611047
- Ekrem Ali Yildirim - 245611021

## Kurulum

1. PostgreSQL kurulumu yapilir.
2. `StokTakipSistemi` adinda bir veritabani olusturulur.
3. `StokTakip.DataAccess/DbHelper.cs` icindeki baglanti bilgileri yerel PostgreSQL bilgilerine gore duzenlenir.
4. Proje Visual Studio veya .NET CLI ile acilir.
5. Paketler restore edilir ve solution build edilir.

```bash
dotnet restore
dotnet build
```

## Ekran Goruntuleri

Final asamasinda uygulama ekran goruntuleri bu bolume eklenecektir.
