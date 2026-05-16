using System.Collections.ObjectModel;
using System.Windows.Input;
using StokTakip.Business;
using StokTakip.Entities;
using StokTakip.Wpf.Helpers;

namespace StokTakip.Wpf.ViewModels;

/// <summary>
/// Kullanıcı yönetimi ekranında listeleme ve CRUD işlemlerini Business katmanı üzerinden yönetir.
/// </summary>
public class UserManagementViewModel : ViewModelBase
{
    private readonly KullaniciManager _kullaniciManager = new();

    private Kullanici? _seciliKullanici;
    private string _kullaniciAdi = string.Empty;
    private string _adSoyad = string.Empty;
    private string _sifre = string.Empty;
    private string _rol = string.Empty;
    private bool _aktifMi = true;
    private string _durumMesaji = "Kullanıcı yönetimi hazır.";

    public UserManagementViewModel()
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

    public ObservableCollection<Kullanici> Kullanicilar { get; } = new();
    public ObservableCollection<string> Roller { get; } = new() { "Yönetici", "Personel" };

    public Kullanici? SeciliKullanici
    {
        get => _seciliKullanici;
        set
        {
            _seciliKullanici = value;
            OnPropertyChanged();
            SeciliKullaniciFormaAktar();
        }
    }

    public string KullaniciAdi
    {
        get => _kullaniciAdi;
        set
        {
            _kullaniciAdi = value;
            OnPropertyChanged();
        }
    }

    public string AdSoyad
    {
        get => _adSoyad;
        set
        {
            _adSoyad = value;
            OnPropertyChanged();
        }
    }

    /// <summary>PasswordBox ile code-behind üzerinden senkronize edilir.</summary>
    public string Sifre
    {
        get => _sifre;
        set
        {
            _sifre = value;
            OnPropertyChanged();
        }
    }

    public string Rol
    {
        get => _rol;
        set
        {
            _rol = value;
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
    /// Kullanıcı listesini async yükler.
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
                    return _kullaniciManager.GetAll();
                }
                catch
                {
                    return new List<Kullanici>();
                }
            }).ConfigureAwait(true);

            Kullanicilar.Clear();
            foreach (var kullanici in liste)
            {
                Kullanicilar.Add(kullanici);
            }

            DurumMesaji = Kullanicilar.Count > 0
                ? "Kullanıcı listesi güncellendi."
                : "Kayıt bulunamadı veya veritabanına bağlanılamadı.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task EkleAsync()
    {
        // Placeholder metni kayda gitmesin; gercek rol secimi zorunlu.
        if (!RolSecimiGecerliMi())
        {
            return;
        }

        IsBusy = true;
        DurumMesaji = "Kaydediliyor...";

        try
        {
            var kullanici = FormdanKullaniciOlustur();
            await Task.Run(() => _kullaniciManager.Add(kullanici)).ConfigureAwait(true);
            await YukleAsync().ConfigureAwait(true);
            Temizle();
            DurumMesaji = "Kullanıcı başarıyla eklendi.";
        }
        catch (ArgumentException ex)
        {
            DurumMesaji = ex.Message;
            DialogHelper.ShowWarning(ex.Message);
        }
        catch (Exception ex)
        {
            DurumMesaji = "Kullanıcı eklenemedi.";
            DialogHelper.ShowError(ex.Message, "Kullanıcı Yönetimi");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task GuncelleAsync()
    {
        if (SeciliKullanici is null)
        {
            DialogHelper.ShowWarning("Güncellemek için listeden bir kullanıcı seçmelisiniz.");
            return;
        }

        if (!RolSecimiGecerliMi())
        {
            return;
        }

        IsBusy = true;
        DurumMesaji = "Güncelleniyor...";

        try
        {
            var kullanici = FormdanKullaniciOlustur();
            kullanici.Id = SeciliKullanici.Id;
            kullanici.OlusturmaTarihi = SeciliKullanici.OlusturmaTarihi;

            await Task.Run(() => _kullaniciManager.Update(kullanici)).ConfigureAwait(true);
            await YukleAsync().ConfigureAwait(true);
            Temizle();
            DurumMesaji = "Kullanıcı başarıyla güncellendi.";
        }
        catch (ArgumentException ex)
        {
            DurumMesaji = ex.Message;
            DialogHelper.ShowWarning(ex.Message);
        }
        catch (Exception ex)
        {
            DurumMesaji = "Kullanıcı güncellenemedi.";
            DialogHelper.ShowError(ex.Message, "Kullanıcı Yönetimi");
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>Silme öncesi onay alır; Evet sonrası Business katmanından siler.</summary>
    private async Task SilOnayliAsync()
    {
        if (SeciliKullanici is null)
        {
            DialogHelper.ShowWarning("Silmek için listeden bir kullanıcı seçmelisiniz.");
            return;
        }

        if (!DialogHelper.ShowConfirm("Bu kullanıcıyı silmek istediğinize emin misiniz?", "Silme Onayı"))
        {
            return;
        }

        IsBusy = true;
        DurumMesaji = "Siliniyor...";

        try
        {
            int id = SeciliKullanici.Id;
            await Task.Run(() => _kullaniciManager.Delete(id)).ConfigureAwait(true);
            await YukleAsync().ConfigureAwait(true);
            Temizle();
            DurumMesaji = "Kullanıcı başarıyla silindi.";
            DialogHelper.ShowSuccess("Kullanıcı başarıyla silindi.");
        }
        catch (ArgumentException ex)
        {
            DurumMesaji = ex.Message;
            DialogHelper.ShowWarning(ex.Message);
        }
        catch (Exception ex)
        {
            DurumMesaji = "Kullanıcı silinemedi.";
            DialogHelper.ShowError(ex.Message, "Kullanıcı Yönetimi");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void SeciliKullaniciFormaAktar()
    {
        if (SeciliKullanici is null)
        {
            return;
        }

        KullaniciAdi = SeciliKullanici.KullaniciAdi;
        AdSoyad = SeciliKullanici.AdSoyad;
        Sifre = SeciliKullanici.Sifre;
        Rol = SeciliKullanici.Rol ?? string.Empty;
        AktifMi = SeciliKullanici.AktifMi;
        OnPropertyChanged(nameof(Sifre));
    }

    private Kullanici FormdanKullaniciOlustur()
    {
        string adSoyad = string.IsNullOrWhiteSpace(AdSoyad) ? KullaniciAdi.Trim() : AdSoyad.Trim();
        return new Kullanici
        {
            KullaniciAdi = KullaniciAdi.Trim(),
            AdSoyad = adSoyad,
            Sifre = Sifre,
            Rol = Rol.Trim(),
            AktifMi = AktifMi
        };
    }

    public void Temizle()
    {
        SeciliKullanici = null;
        KullaniciAdi = string.Empty;
        AdSoyad = string.Empty;
        Sifre = string.Empty;
        Rol = string.Empty;
        AktifMi = true;
        DurumMesaji = "Form temizlendi.";
        OnPropertyChanged(nameof(Sifre));
    }

    /// <summary>ComboBox placeholder secili degilken kayit engellenir.</summary>
    private bool RolSecimiGecerliMi()
    {
        if (string.IsNullOrWhiteSpace(Rol) || !Roller.Contains(Rol))
        {
            DialogHelper.ShowWarning("Lütfen rol seçiniz.");
            return false;
        }

        return true;
    }
}
