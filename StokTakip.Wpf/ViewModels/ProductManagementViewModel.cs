using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using StokTakip.Business;
using StokTakip.Entities;
using StokTakip.Wpf.Helpers;

namespace StokTakip.Wpf.ViewModels;

/// <summary>
/// WPF Ürün Yönetimi ekranında ürün listeleme, form doldurma, CRUD ve filtreleme akışını yalnızca Business (Manager) katmanı üzerinden yönetir.
/// Veritabanı çağrıları Task.Run ile arka planda çalıştırıldığı için UI thread donmaz; IsBusy ile kullanıcıya yükleniyor durumu yansıtılır.
/// </summary>
public class ProductManagementViewModel : ViewModelBase
{
    /// <summary>
    /// Veritabanı kapalı veya erişilemez olduğunda gösterilecek sabit kullanıcı mesajıdır.
    /// </summary>
    public const string VeritabaniBaglantiMesaji = "Veritabanı bağlantısı kurulamadı. Lütfen PostgreSQL sunucusunu kontrol edin.";

    private readonly UrunManager _urunManager = new();
    private readonly KategoriManager _kategoriManager = new();
    private readonly SaklamaKosuluManager _saklamaKosuluManager = new();
    private readonly List<Urun> _tumUrunler = new();

    private Urun? _seciliUrun;
    private Kategori? _seciliKategori;
    private Kategori? _filtreKategori;
    private SaklamaKosulu? _seciliSaklamaKosulu;
    private string _urunAdi = string.Empty;
    private string _barkod = string.Empty;
    private string _birim = string.Empty;
    private string _aciklama = string.Empty;
    private string _stokMiktari = "0";
    private string _kritikStokSeviyesi = "0";
    private string _alisFiyati = "0,00";
    private string _satisFiyati = "0,00";
    private string _urunAdiFiltresi = string.Empty;
    private string _durumMesaji = "Ürün yönetimi hazır.";
    private bool _aktifMi = true;

    public ProductManagementViewModel()
    {
        EkleCommand = new AsyncRelayCommand(EkleAsync, () => !IsBusy);
        GuncelleCommand = new AsyncRelayCommand(GuncelleAsync, () => !IsBusy);
        SilCommand = new AsyncRelayCommand(SilOnayliAsync, () => !IsBusy);
        TemizleCommand = new RelayCommand(_ => Temizle(), _ => !IsBusy);
        FiltreleCommand = new RelayCommand(_ => Filtrele(), _ => !IsBusy);
        FiltreTemizleCommand = new RelayCommand(_ => FiltreTemizle(), _ => !IsBusy);
        ListeYenileCommand = new AsyncRelayCommand(UrunleriYukleAsync, () => !IsBusy);
    }

    public ICommand EkleCommand { get; }
    public ICommand GuncelleCommand { get; }
    public ICommand SilCommand { get; }
    public ICommand TemizleCommand { get; }
    public ICommand FiltreleCommand { get; }
    public ICommand FiltreTemizleCommand { get; }
    public ICommand ListeYenileCommand { get; }

    public ObservableCollection<Urun> Urunler { get; } = new();
    public ObservableCollection<Kategori> Kategoriler { get; } = new();
    public ObservableCollection<Kategori> KategoriFiltreleri { get; } = new();
    public ObservableCollection<SaklamaKosulu> SaklamaKosullari { get; } = new();

    public Urun? SeciliUrun
    {
        get => _seciliUrun;
        set
        {
            _seciliUrun = value;
            OnPropertyChanged();
            SeciliUrunFormaAktar();
        }
    }

    public Kategori? SeciliKategori
    {
        get => _seciliKategori;
        set
        {
            _seciliKategori = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(SeciliKategoriAdi));
        }
    }

    public Kategori? FiltreKategori
    {
        get => _filtreKategori;
        set
        {
            _filtreKategori = value;
            OnPropertyChanged();
        }
    }

    public SaklamaKosulu? SeciliSaklamaKosulu
    {
        get => _seciliSaklamaKosulu;
        set
        {
            _seciliSaklamaKosulu = value;
            OnPropertyChanged();
        }
    }

    public string UrunAdi
    {
        get => _urunAdi;
        set
        {
            _urunAdi = value;
            OnPropertyChanged();
        }
    }

    public string Barkod
    {
        get => _barkod;
        set
        {
            _barkod = value;
            OnPropertyChanged();
        }
    }

    public string Birim
    {
        get => _birim;
        set
        {
            _birim = value;
            OnPropertyChanged();
        }
    }

    public string Aciklama
    {
        get => _aciklama;
        set
        {
            _aciklama = value;
            OnPropertyChanged();
        }
    }

    public string StokMiktari
    {
        get => _stokMiktari;
        set
        {
            _stokMiktari = value;
            OnPropertyChanged();
        }
    }

    public string KritikStokSeviyesi
    {
        get => _kritikStokSeviyesi;
        set
        {
            _kritikStokSeviyesi = value;
            OnPropertyChanged();
        }
    }

    public string AlisFiyati
    {
        get => _alisFiyati;
        set
        {
            _alisFiyati = value;
            OnPropertyChanged();
        }
    }

    public string SatisFiyati
    {
        get => _satisFiyati;
        set
        {
            _satisFiyati = value;
            OnPropertyChanged();
        }
    }

    public bool AktifMi
    {
        get => _aktifMi;
        set
        {
            _aktifMi = value;
            OnPropertyChanged();
        }
    }

    public string UrunAdiFiltresi
    {
        get => _urunAdiFiltresi;
        set
        {
            _urunAdiFiltresi = value;
            OnPropertyChanged();
        }
    }

    public string DurumMesaji
    {
        get => _durumMesaji;
        private set
        {
            _durumMesaji = value;
            OnPropertyChanged();
        }
    }

    public string SeciliKategoriAdi => SeciliKategori?.KategoriAdi ?? string.Empty;

    /// <summary>
    /// Ürün ekranı ilk açıldığında kategori, saklama koşulu ve ürün listesini arka planda yükler; UI thread mesaj döngüsünü bloke etmez.
    /// </summary>
    public async Task YukleAsync()
    {
        IsBusy = true;
        DurumMesaji = "Yükleniyor...";

        try
        {
            await ReferanslariArkaPlandaYukleVeUygulaAsync().ConfigureAwait(true);
            bool urunlerOk = await UrunListesiniYenidenCekAsync().ConfigureAwait(true);
            Temizle();
            if (urunlerOk)
            {
                DurumMesaji = "Ürün listesi ve referans veriler yüklendi.";
            }
            else
            {
                DurumMesaji = VeritabaniBaglantiMesaji;
            }
        }
        catch (Exception ex)
        {
            DurumMesaji = "Yükleme tamamlanamadı.";
            KullaniciyaHataGoster("Ürün yönetimi verileri yüklenirken bir sorun oluştu.", ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Ürünleri UrunManager üzerinden arka planda çeker, bellekte saklar ve aktif filtreleri uygular.
    /// </summary>
    public async Task UrunleriYukleAsync()
    {
        IsBusy = true;
        DurumMesaji = "Yükleniyor...";

        try
        {
            bool ok = await UrunListesiniYenidenCekAsync().ConfigureAwait(true);
            if (ok)
            {
                DurumMesaji = "Ürün listesi güncellendi.";
            }
        }
        catch (Exception ex)
        {
            DurumMesaji = "Ürün listesi alınamadı.";
            KullaniciyaHataGoster("Ürün listesi yüklenirken bir sorun oluştu.", ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Form bilgileriyle yeni ürün oluşturur; kayıt işlemi arka planda yapılır.
    /// </summary>
    private async Task EkleAsync()
    {
        if (!UrunFormSecimleriGecerliMi())
        {
            return;
        }

        IsBusy = true;
        DurumMesaji = "Kaydediliyor...";

        try
        {
            var urun = FormdanUrunOlustur();
            await Task.Run(() => _urunManager.Add(urun)).ConfigureAwait(true);
            await UrunListesiniYenidenCekAsync().ConfigureAwait(true);
            Temizle();
            DurumMesaji = "Ürün başarıyla eklendi.";
        }
        catch (ArgumentException ex)
        {
            DialogHelper.ShowWarning(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            DialogHelper.ShowWarning(ex.Message);
        }
        catch (Exception ex)
        {
            KullaniciyaHataGoster("Ürün eklenirken bir sorun oluştu.", ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Seçili ürünü formdaki bilgilerle günceller; veritabanı güncellemesi arka planda yapılır.
    /// </summary>
    private async Task GuncelleAsync()
    {
        if (SeciliUrun is null)
        {
            DialogHelper.ShowWarning("Güncellemek için listeden bir ürün seçmelisiniz.");
            return;
        }

        if (!UrunFormSecimleriGecerliMi())
        {
            return;
        }

        IsBusy = true;
        DurumMesaji = "Güncelleniyor...";

        try
        {
            var urun = FormdanUrunOlustur();
            urun.Id = SeciliUrun.Id;
            urun.OlusturmaTarihi = SeciliUrun.OlusturmaTarihi;

            await Task.Run(() => _urunManager.Update(urun)).ConfigureAwait(true);
            await UrunListesiniYenidenCekAsync().ConfigureAwait(true);
            Temizle();
            DurumMesaji = "Ürün başarıyla güncellendi.";
        }
        catch (ArgumentException ex)
        {
            DialogHelper.ShowWarning(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            DialogHelper.ShowWarning(ex.Message);
        }
        catch (Exception ex)
        {
            KullaniciyaHataGoster("Ürün güncellenirken bir sorun oluştu.", ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Onay sonrası seçili ürünü siler; silme işlemi arka planda yapılır.
    /// </summary>
    private async Task SilOnayliAsync()
    {
        if (SeciliUrun is null)
        {
            DialogHelper.ShowWarning("Silmek için listeden bir ürün seçmelisiniz.");
            return;
        }

        // Silme öncesi kullanıcı onayı; Hayır seçilirse liste ve form değişmez.
        if (!DialogHelper.ShowConfirm("Bu ürünü silmek istediğinize emin misiniz?", "Silme Onayı"))
        {
            return;
        }

        IsBusy = true;
        DurumMesaji = "Siliniyor...";

        try
        {
            int id = SeciliUrun.Id;
            await Task.Run(() => _urunManager.Delete(id)).ConfigureAwait(true);
            await UrunListesiniYenidenCekAsync().ConfigureAwait(true);
            Temizle();
            DurumMesaji = "Ürün başarıyla silindi.";
            DialogHelper.ShowSuccess("Ürün başarıyla silindi.");
        }
        catch (ArgumentException ex)
        {
            DialogHelper.ShowWarning(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            DialogHelper.ShowWarning(ex.Message);
        }
        catch (Exception ex)
        {
            KullaniciyaHataGoster("Ürün silinirken bir sorun oluştu.", ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Ürün adı ve kategori filtresini mevcut ürün listesi üzerinde LINQ ile uygular (SQL kullanılmaz).
    /// </summary>
    public void Filtrele()
    {
        try
        {
            IEnumerable<Urun> filtreliUrunler = _tumUrunler;

            if (FiltreKategori is not null && FiltreKategori.Id > 0)
            {
                filtreliUrunler = filtreliUrunler.Where(urun => urun.KategoriId == FiltreKategori.Id);
            }

            if (!string.IsNullOrWhiteSpace(UrunAdiFiltresi))
            {
                filtreliUrunler = filtreliUrunler.Where(urun =>
                    urun.UrunAdi.Contains(UrunAdiFiltresi.Trim(), StringComparison.CurrentCultureIgnoreCase));
            }

            Urunler.Clear();
            foreach (var urun in filtreliUrunler)
            {
                Urunler.Add(urun);
            }

            DurumMesaji = "Filtre uygulandı.";
        }
        catch (Exception ex)
        {
            DurumMesaji = "Filtre uygulanamadı.";
            KullaniciyaHataGoster("Filtreleme sırasında bir sorun oluştu.", ex);
        }
    }

    /// <summary>
    /// Filtre alanlarını temizleyip tüm ürünleri yeniden gösterir.
    /// </summary>
    public void FiltreTemizle()
    {
        try
        {
            FiltreKategori = KategoriFiltreleri.FirstOrDefault();
            UrunAdiFiltresi = string.Empty;
            Filtrele();
            DurumMesaji = "Filtreler temizlendi.";
        }
        catch (Exception ex)
        {
            KullaniciyaHataGoster("Filtreler temizlenirken bir sorun oluştu.", ex);
        }
    }

    /// <summary>
    /// Form alanlarını ve seçili ürünü varsayılan değerlere döndürür; aktif ürün varsayılanı true kalır.
    /// </summary>
    public void Temizle()
    {
        SeciliUrun = null;
        SeciliKategori = null;
        SeciliSaklamaKosulu = null;
        UrunAdi = string.Empty;
        Barkod = string.Empty;
        Birim = string.Empty;
        Aciklama = string.Empty;
        StokMiktari = "0";
        KritikStokSeviyesi = "0";
        AlisFiyati = "0,00";
        SatisFiyati = "0,00";
        AktifMi = true;
        DurumMesaji = "Form temizlendi.";
    }

    /// <summary>
    /// Kategori ve saklama koşullarını arka planda okuyup koleksiyonlara uygular.
    /// </summary>
    private async Task ReferanslariArkaPlandaYukleVeUygulaAsync()
    {
        var sonuc = await Task.Run(() =>
        {
            try
            {
                return (_kategoriManager.GetAll().ToList(), _saklamaKosuluManager.GetAll().ToList(), (Exception?)null);
            }
            catch (Exception ex)
            {
                return (new List<Kategori>(), new List<SaklamaKosulu>(), (Exception?)ex);
            }
        }).ConfigureAwait(true);

        Kategoriler.Clear();
        KategoriFiltreleri.Clear();
        SaklamaKosullari.Clear();

        if (sonuc.Item3 is not null)
        {
            DurumMesaji = VeritabaniBaglantiMesaji;
            DialogHelper.ShowWarning(VeritabaniBaglantiMesaji, "Ürün Yönetimi");
            KategoriFiltreleri.Add(new Kategori { Id = 0, KategoriAdi = "Tüm Kategoriler" });
            FiltreKategori = KategoriFiltreleri.FirstOrDefault();
            return;
        }

        KategoriFiltreleri.Add(new Kategori { Id = 0, KategoriAdi = "Tüm Kategoriler" });

        foreach (var kategori in sonuc.Item1)
        {
            Kategoriler.Add(kategori);
            KategoriFiltreleri.Add(kategori);
        }

        foreach (var kosul in sonuc.Item2)
        {
            SaklamaKosullari.Add(kosul);
        }

        FiltreKategori = KategoriFiltreleri.FirstOrDefault();
    }

    /// <summary>
    /// Ürün listesini arka planda çeker; IsBusy yönetimi çağıran metoda aittir. Başarılıysa true döner.
    /// </summary>
    private async Task<bool> UrunListesiniYenidenCekAsync()
    {
        List<Urun> liste;

        try
        {
            liste = await Task.Run(() => _urunManager.GetAll().ToList()).ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            _tumUrunler.Clear();
            Urunler.Clear();
            DurumMesaji = VeritabaniBaglantiMesaji;
            string mesaj = VeritabaniKaynakliMi(ex)
                ? VeritabaniBaglantiMesaji
                : "Ürün listesi alınamadı. Lütfen daha sonra tekrar deneyin.";
            DialogHelper.ShowWarning(mesaj, "Ürün Yönetimi");
            return false;
        }

        _tumUrunler.Clear();
        foreach (var urun in liste)
        {
            _tumUrunler.Add(urun);
        }

        Filtrele();
        return true;
    }

    private void SeciliUrunFormaAktar()
    {
        if (SeciliUrun is null)
        {
            return;
        }

        SeciliKategori = Kategoriler.FirstOrDefault(kategori => kategori.Id == SeciliUrun.KategoriId);
        SeciliSaklamaKosulu = SeciliUrun.SaklamaKosuluId.HasValue
            ? SaklamaKosullari.FirstOrDefault(kosul => kosul.Id == SeciliUrun.SaklamaKosuluId.Value)
            : null;

        UrunAdi = SeciliUrun.UrunAdi;
        Barkod = SeciliUrun.Barkod;
        Birim = SeciliUrun.Birim;
        Aciklama = SeciliUrun.Aciklama;
        StokMiktari = SeciliUrun.StokMiktari.ToString(CultureInfo.CurrentCulture);
        KritikStokSeviyesi = SeciliUrun.KritikStokSeviyesi.ToString(CultureInfo.CurrentCulture);
        AlisFiyati = SeciliUrun.AlisFiyati.ToString("0.00", CultureInfo.CurrentCulture);
        SatisFiyati = SeciliUrun.SatisFiyati.ToString("0.00", CultureInfo.CurrentCulture);
        AktifMi = SeciliUrun.AktifMi;
        DurumMesaji = "Seçili ürün forma aktarıldı.";
        OnPropertyChanged(nameof(SeciliKategoriAdi));
    }

    /// <summary>Placeholder secimi kayda gitmesin diye kategori ve saklama kosulu kontrol edilir.</summary>
    private bool UrunFormSecimleriGecerliMi()
    {
        if (SeciliKategori is null)
        {
            DialogHelper.ShowWarning("Lütfen kategori seçiniz.");
            return false;
        }

        if (SeciliSaklamaKosulu is null)
        {
            DialogHelper.ShowWarning("Lütfen saklama koşulu seçiniz.");
            return false;
        }

        return true;
    }

    private Urun FormdanUrunOlustur()
    {
        return new Urun
        {
            KategoriId = SeciliKategori!.Id,
            SaklamaKosuluId = SeciliSaklamaKosulu!.Id,
            UrunAdi = UrunAdi.Trim(),
            Barkod = Barkod.Trim(),
            Birim = Birim.Trim(),
            Aciklama = Aciklama.Trim(),
            StokMiktari = IntDegerOku(StokMiktari, "Stok"),
            KritikStokSeviyesi = IntDegerOku(KritikStokSeviyesi, "Kritik stok"),
            AlisFiyati = DecimalDegerOku(AlisFiyati, "Alış fiyatı"),
            SatisFiyati = DecimalDegerOku(SatisFiyati, "Satış fiyatı"),
            AktifMi = AktifMi
        };
    }

    private static int IntDegerOku(string deger, string alanAdi)
    {
        if (!int.TryParse(deger, NumberStyles.Integer, CultureInfo.CurrentCulture, out int sonuc))
        {
            throw new ArgumentException($"{alanAdi} alanı geçerli bir tam sayı olmalıdır.");
        }

        return sonuc;
    }

    private static decimal DecimalDegerOku(string deger, string alanAdi)
    {
        if (decimal.TryParse(deger, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal sonuc))
        {
            return sonuc;
        }

        if (decimal.TryParse(deger, NumberStyles.Number, CultureInfo.InvariantCulture, out sonuc))
        {
            return sonuc;
        }

        throw new ArgumentException($"{alanAdi} alanı geçerli bir sayı olmalıdır.");
    }

    private static bool VeritabaniKaynakliMi(Exception ex)
    {
        var tum = $"{ex.GetType().FullName} {ex.Message} {ex.InnerException?.Message}";
        return tum.Contains("Npgsql", StringComparison.OrdinalIgnoreCase)
               || tum.Contains("Failed to connect", StringComparison.OrdinalIgnoreCase)
               || tum.Contains("connection", StringComparison.OrdinalIgnoreCase)
               || tum.Contains("bağlan", StringComparison.OrdinalIgnoreCase)
               || tum.Contains("timeout", StringComparison.OrdinalIgnoreCase);
    }

    private static void KullaniciyaHataGoster(string baslik, Exception ex)
    {
        string mesaj = VeritabaniKaynakliMi(ex)
            ? VeritabaniBaglantiMesaji
            : "İşlem tamamlanamadı. Lütfen bilgilerinizi kontrol edip tekrar deneyin.";

        DialogHelper.ShowError(mesaj, baslik);
    }
}
