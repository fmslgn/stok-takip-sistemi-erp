# CODEX Proje Kurallari

Bu dosya, projede bundan sonra yapilacak tum gelistirmelerde dikkate alinacak temel kurallari tanimlar.

## Proje Genel Bilgileri

- Proje C# Windows Forms + PostgreSQL ile gelistirilen ERP mantikli stok takip sistemidir.
- Proje katmanli mimariye gore gelistirilmelidir.
- Katmanlar:
  - StokTakip.Entities
  - StokTakip.DataAccess
  - StokTakip.Business
  - StokTakip.WinForms
- Veritabani PostgreSQL kullanilacaktir.
- UI tarafi Windows Forms olarak kalacaktir.

## Genel Gelistirme Kurallari

1. Her islemden once proje klasoru taranmalidir.
2. Mevcut dosya yapisi, siniflar, metotlar ve formlar incelenmeden degisiklik yapilmamalidir.
3. Calisan moduller bozulmamalidir.
4. Login, kategori, urun, stok giris, stok cikis, kritik stok ve raporlama modulleri calismaya devam etmelidir.
5. UI/UX tasarimi bozulmamalidir.
6. Mevcut form duzeni, renkler, buton yerlesimleri ve kullanici deneyimi korunmalidir.
7. Yeni ozellik eklenirken mevcut arayuz yapisina uyumlu sekilde eklenmelidir.
8. WinForms icinde dogrudan SQL sorgusu yazilmamalidir.
9. WinForms sadece Business/Manager siniflarini cagirmalidir.
10. Business katmani DataAccess katmanini cagirmalidir.
11. DataAccess katmani PostgreSQL baglantisini yonetmelidir.
12. SQL sorgulari parametreli yazilmalidir.
13. Baglantilar using blogu ile kapatilmalidir.
14. Proje her islem sonunda `dotnet build` ile derlenebilir kalmalidir.

## Kod Aciklama Kurallari

1. Kodlarda Turkce aciklayici yorum satirlari olmalidir.
2. Aciklama satiri olmayan onemli sinif, metot ve eventlere aciklama eklenmelidir.
3. Her sinifin basinda o sinifin gorevi kisa sekilde aciklanmalidir.
4. Onemli metotlarin ustunde ne yaptigi aciklanmalidir.
5. WinForms buton eventlerinde hangi islemin yapildigi yorum satiriyla belirtilmelidir.
6. DbHelper, DAL, Manager ve Entity siniflarinda aciklayici yorumlar bulunmalidir.
7. Gereksiz her satira yorum yazilmamalidir.
8. Yorumlar ogrencinin sunumda anlatabilecegi duzeyde sade ve anlasilir olmalidir.

## UI/UX Kurallari

1. Mevcut UI/UX tasarimi bozulmamalidir.
2. Yeni form veya buton eklenecekse mevcut tasarimla uyumlu olmalidir.
3. Form basliklari Turkce karakterli ve duzgun yazilmalidir.
4. Buton isimleri acik ve anlasilir olmalidir.
5. DataGridView yapilari okunabilir kalmalidir.
6. Kullaniciya hata ve basari durumlarinda MessageBox ile geri bildirim verilmelidir.
7. Form gecisleri bozulmamalidir.
8. Ana Menu uzerinden tum modullere erisim devam etmelidir.

## Saklama Kosulu / Yapay Zeka Oneri Modulu Kurallari

1. Bu modul gercek API kullanmak zorunda degildir.
2. Final projesi icin kural tabanli akilli oneri sistemi olarak calisabilir.
3. Urun adi ve kategori adina gore saklama kosulu onerisi vermelidir.
4. Turkce karakterli ve Turkce karaktersiz girisleri algilamalidir.
5. Oneriler kullaniciya anlasilir Turkce metin olarak gosterilmelidir.

## README Kurallari

1. Yeni ozellik eklenirse README.md dosyasina kisa aciklamasi eklenmelidir.
2. Kurulum ve calistirma bilgileri korunmalidir.
3. Proje amaci, teknolojiler, moduller ve ekip uyeleri bolumleri bozulmamalidir.

## Islem Sonu Kurallari

- Proje build edilebilir halde kalmalidir.
- Gereksiz dosya, zip, bin/obj veya gecici klasor eklenmemelidir.
