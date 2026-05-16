using System.Collections.ObjectModel;
using System.Windows.Input;
using StokTakip.Business;
using StokTakip.Entities;
using StokTakip.Wpf.Helpers;

namespace StokTakip.Wpf.ViewModels;

/// <summary>
/// Kategori yönetimi ekranında listeleme ve CRUD işlemlerini Business katmanı üzerinden yönetir.
/// </summary>
public class CategoryManagementViewModel : ViewModelBase
{
    private readonly KategoriManager _kategoriManager = new();

    private Kategori? _seciliKategori;
    private string _kategoriAdi = string.Empty;
    private string _aciklama = string.Empty;
    private bool _aktifMi = true;
    private string _durumMesaji = "Kategori yönetimi hazır.";

    public CategoryManagementViewModel()
    {
        EkleCommand = new AsyncRelayCommand(EkleAsync, () => !IsBusy);
        GuncelleCommand = new AsyncRelayCommand(GuncelleAsync, () => !IsBusy);
        SilCommand = new AsyncRelayCommand(SilOnayliAsync, () => !IsBusy);
        TemizleCommand = new RelayCommand(_ => Temizle(), _ => !IsBusy);
        ListeYenileCommand = new AsyncRelayCommand(YukleAsync, () => !IsBusy);
    }

    public ICommand EkleCommand { get; }
    public ICommand GuncelleCommand { get; }
    public ICommand SilCommand { get; }
    public ICommand TemizleCommand { get; }
    public ICommand ListeYenileCommand { get; }

    public ObservableCollection<Kategori> Kategoriler { get; } = new();

    public Kategori? SeciliKategori
    {
        get => _seciliKategori;
        set
        {
            _seciliKategori = value;
            OnPropertyChanged();
            SeciliKategoriFormaAktar();
        }
    }

    public string KategoriAdi
    {
        get => _kategoriAdi;
        set
        {
            _kategoriAdi = value;
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

    public bool AktifMi
    {
        get => _aktifMi;
        set
        {
            _aktifMi = value;
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

    /// <summary>
    /// Kategori listesini veritabanından async yükler.
    /// </summary>
    public async Task YukleAsync()
    {
        IsBusy = true;
        DurumMesaji = "Yükleniyor...";

        try
        {
            var liste = await Task.Run(() =>
            {
                try
                {
                    return _kategoriManager.GetAll();
                }
                catch
                {
                    return new List<Kategori>();
                }
            }).ConfigureAwait(true);

            Kategoriler.Clear();
            foreach (var kategori in liste)
            {
                Kategoriler.Add(kategori);
            }

            DurumMesaji = Kategoriler.Count > 0
                ? "Kategori listesi güncellendi."
                : "Kayıt bulunamadı veya veritabanına bağlanılamadı.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task EkleAsync()
    {
        IsBusy = true;
        DurumMesaji = "Kaydediliyor...";

        try
        {
            var kategori = FormdanKategoriOlustur();
            await Task.Run(() => _kategoriManager.Add(kategori)).ConfigureAwait(true);
            await YukleAsync().ConfigureAwait(true);
            Temizle();
            DurumMesaji = "Kategori başarıyla eklendi.";
        }
        catch (ArgumentException ex)
        {
            DurumMesaji = ex.Message;
            DialogHelper.ShowWarning(ex.Message);
        }
        catch (Exception ex)
        {
            DurumMesaji = "Kategori eklenemedi.";
            DialogHelper.ShowError(ex.Message, "Kategori Yönetimi");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task GuncelleAsync()
    {
        if (SeciliKategori is null)
        {
            DialogHelper.ShowWarning("Güncellemek için listeden bir kategori seçmelisiniz.");
            return;
        }

        IsBusy = true;
        DurumMesaji = "Güncelleniyor...";

        try
        {
            var kategori = FormdanKategoriOlustur();
            kategori.Id = SeciliKategori.Id;
            kategori.OlusturmaTarihi = SeciliKategori.OlusturmaTarihi;

            await Task.Run(() => _kategoriManager.Update(kategori)).ConfigureAwait(true);
            await YukleAsync().ConfigureAwait(true);
            Temizle();
            DurumMesaji = "Kategori başarıyla güncellendi.";
        }
        catch (ArgumentException ex)
        {
            DurumMesaji = ex.Message;
            DialogHelper.ShowWarning(ex.Message);
        }
        catch (Exception ex)
        {
            DurumMesaji = "Kategori güncellenemedi.";
            DialogHelper.ShowError(ex.Message, "Kategori Yönetimi");
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>Silme öncesi onay alır; Evet sonrası Business katmanından siler.</summary>
    private async Task SilOnayliAsync()
    {
        if (SeciliKategori is null)
        {
            DialogHelper.ShowWarning("Silmek için listeden bir kategori seçmelisiniz.");
            return;
        }

        if (!DialogHelper.ShowConfirm("Bu kategoriyi silmek istediğinize emin misiniz?", "Silme Onayı"))
        {
            return;
        }

        IsBusy = true;
        DurumMesaji = "Siliniyor...";

        try
        {
            int id = SeciliKategori.Id;
            await Task.Run(() => _kategoriManager.Delete(id)).ConfigureAwait(true);
            await YukleAsync().ConfigureAwait(true);
            Temizle();
            DurumMesaji = "Kategori başarıyla silindi.";
            DialogHelper.ShowSuccess("Kategori başarıyla silindi.");
        }
        catch (ArgumentException ex)
        {
            DurumMesaji = ex.Message;
            DialogHelper.ShowWarning(ex.Message);
        }
        catch (Exception ex)
        {
            DurumMesaji = "Kategori silinemedi.";
            DialogHelper.ShowError(ex.Message, "Kategori Yönetimi");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void SeciliKategoriFormaAktar()
    {
        if (SeciliKategori is null)
        {
            return;
        }

        KategoriAdi = SeciliKategori.KategoriAdi;
        Aciklama = SeciliKategori.Aciklama;
        AktifMi = SeciliKategori.AktifMi;
    }

    private Kategori FormdanKategoriOlustur() =>
        new()
        {
            KategoriAdi = KategoriAdi.Trim(),
            Aciklama = Aciklama.Trim(),
            AktifMi = AktifMi
        };

    private void Temizle()
    {
        SeciliKategori = null;
        KategoriAdi = string.Empty;
        Aciklama = string.Empty;
        AktifMi = true;
        DurumMesaji = "Form temizlendi.";
    }
}
