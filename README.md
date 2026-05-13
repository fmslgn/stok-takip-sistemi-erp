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

## Saklama Koşulu Öneri Modülü

Bu modül, kullanıcı tarafından girilen ürün adı ve kategori bilgisine göre kural tabanlı saklama koşulu önerisi üretir. Gerçek yapay zeka API bağlantısı kullanılmadan, ürün türüne göre akıllı öneri mantığıyla çalışır.

## Ürün Yönetimi İçinde Saklama Koşulu Önerisi

Ürün Yönetimi ekranında ürün adı ve kategori bilgisine göre kural tabanlı saklama koşulu önerisi alınabilir. Bu özellik gerçek API kullanmadan akıllı öneri mantığıyla çalışır.

## Web Veri Destekli Saklama Koşulu Önerisi

Ürün Yönetimi ekranında ürün adı, kategori ve barkod bilgisine göre saklama koşulu önerisi alınabilir. Sistem mümkün olduğunda Open Food Facts gibi açık ürün verisi sağlayan web servislerinden destek alır. Web servisinden veri alınamazsa kural tabanlı öneri sistemi otomatik olarak devreye girer. Bu yapı API key gerektirmez ve uygulamanın çevrimdışı durumda da çalışmasını sağlar.

## Saklama Önerisi Revize Özelliği

Ürün Yönetimi ekranında kullanıcı saklama koşulu önerisi aldıktan sonra öneriyi kısa, detaylı veya güvenlik uyarısı içerecek şekilde revize edebilir. Revize işlemi Business katmanında çalışan kural tabanlı mantıkla yapılır.

## Ana Menü / Dashboard

Login sonrası kullanıcıyı karşılayan Ana Menü ekranında sistem modüllerine hızlı erişim ve temel özet kartları bulunur.

## Ana Menü ve Saklama Önerisi UI Düzenlemesi

Uygulamada login sonrası kullanıcıyı karşılayan Ana Menü / Dashboard ekranı yer alır. Ürün Yönetimi ekranında saklama koşulu önerisi ve öneri revize alanları daha okunabilir şekilde düzenlenmiştir.

## UI/UX Sadeleştirme

Uygulamadaki sabit başarı, uyarı ve bilgi kutuları kaldırılarak daha sade bir arayüz düzeni oluşturulmuştur. İşlem bildirimleri kullanıcıya MessageBox ile gösterilmeye devam eder. Ürün Yönetimi ekranındaki Saklama Koşulu Önerisi alanı daha okunabilir ve düzenli hale getirilmiştir.

## Raporlama Ekranı

Raporlama ekranında toplam ürün sayısı, kritik stoktaki ürün sayısı ve toplam stok miktarı görüntülenebilir. Raporları Getir / Yenile butonu ile değerler güncellenir ve son güncelleme bilgisi ekranda gösterilir.

## Görsel Raporlama

Raporlama ekranında toplam ürün, kritik stok ve toplam stok bilgileri sayısal kartların yanında görsel bar göstergeleriyle de sunulur. Raporları Getir / Yenile butonu ile kartlar ve görsel rapor alanı güncellenir.

## Saklama Asistanı Chatbot

Ürün Yönetimi ekranında yer alan chat ikonu ile Saklama Asistanı açılır. Kullanıcı ürün adı, kategori ve barkod bilgisine göre saklama önerisi alabilir; öneriyi daha kısa, daha detaylı veya güvenlik uyarısı içerecek şekilde chatbot üzerinden tartışabilir. Sistem API key gerektirmeden kural tabanlı akıllı cevap mantığıyla çalışır.

## Ürün Yönetimi Ekranı UI Düzeni

Ürün Yönetimi ekranında ürün bilgi formu ve ürün listesi yan yana konumlandırılarak kullanıcıların ürün ekleme, güncelleme, filtreleme ve listeleme işlemlerini daha kolay takip etmesi sağlanmıştır.

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

## Test Edilen Modüller

- Login işlemi admin / 1234 ile test edildi.
- Kategori CRUD işlemleri test edildi.
- Ürün CRUD işlemleri test edildi.
- Stok giriş işleminde stok miktarının arttığı test edildi.
- Stok çıkış işleminde stok miktarının azaldığı ve yetersiz stok kontrolünün çalıştığı test edildi.
- Kritik stok listeleme test edildi.
- Saklama koşulu öneri ekranı test edildi.
- Raporlama ekranı test edildi.

## Ekran Goruntuleri

Final asamasinda uygulama ekran goruntuleri bu bolume eklenecektir.
