using System.Windows.Input;
using StokTakip.Business;
using StokTakip.Wpf.Helpers;

namespace StokTakip.Wpf.ViewModels;

/// <summary>
/// Raporlama ekranında sayısal kartları ve görsel oranları Business katmanından gelen verilerle yönetir.
/// </summary>
public class ReportsViewModel : ViewModelBase
{
    private int _toplamUrun;
    private int _kritikStok;
    private int _toplamStok;
    private double _kritikStokOrani;
    private double _normalUrunOrani;
    private string _kritikStokOraniMetni = "%0,0";
    private string _normalUrunOraniMetni = "%0,0";
    private string _sonGuncelleme = "Son güncelleme: -";
    private string _durumMesaji = "Rapor verileri hazır.";

    public ReportsViewModel()
    {
        YenileCommand = new RelayCommand(_ => RaporlariYukle());
    }

    public ICommand YenileCommand { get; }

    public int ToplamUrun
    {
        get => _toplamUrun;
        private set
        {
            _toplamUrun = value;
            OnPropertyChanged();
        }
    }

    public int KritikStok
    {
        get => _kritikStok;
        private set
        {
            _kritikStok = value;
            OnPropertyChanged();
        }
    }

    public int ToplamStok
    {
        get => _toplamStok;
        private set
        {
            _toplamStok = value;
            OnPropertyChanged();
        }
    }

    public double KritikStokOrani
    {
        get => _kritikStokOrani;
        private set
        {
            _kritikStokOrani = value;
            OnPropertyChanged();
        }
    }

    public double NormalUrunOrani
    {
        get => _normalUrunOrani;
        private set
        {
            _normalUrunOrani = value;
            OnPropertyChanged();
        }
    }

    public string KritikStokOraniMetni
    {
        get => _kritikStokOraniMetni;
        private set
        {
            _kritikStokOraniMetni = value;
            OnPropertyChanged();
        }
    }

    public string NormalUrunOraniMetni
    {
        get => _normalUrunOraniMetni;
        private set
        {
            _normalUrunOraniMetni = value;
            OnPropertyChanged();
        }
    }

    public string SonGuncelleme
    {
        get => _sonGuncelleme;
        private set
        {
            _sonGuncelleme = value;
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
    /// Rapor kartlarını Business katmanındaki RaporManager metotlarıyla güvenli şekilde yükler.
    /// </summary>
    public void RaporlariYukle()
    {
        try
        {
            var raporManager = new RaporManager();
            ToplamUrun = Math.Max(0, raporManager.ToplamUrunSayisiGetir());
            KritikStok = Math.Max(0, raporManager.KritikStokUrunSayisiGetir());
            ToplamStok = Math.Max(0, raporManager.ToplamStokMiktariGetir());
            DurumMesaji = "Raporlar başarıyla güncellendi.";
        }
        catch
        {
            // Veri alınamazsa ekran kapanmaz; rapor kartları güvenli varsayılan değerlerle gösterilir.
            ToplamUrun = 0;
            KritikStok = 0;
            ToplamStok = 0;
            DurumMesaji = "Rapor verileri alınamadı.";
        }

        OranlariGuncelle();
        SonGuncelleme = $"Son güncelleme: {DateTime.Now:dd.MM.yyyy HH:mm}";
    }

    /// <summary>
    /// Toplam ürün 0 olduğunda bölme hatası ve NaN/Infinity değerleri oluşmaması için oranları sınırlar.
    /// </summary>
    private void OranlariGuncelle()
    {
        double kritikYuzde = ToplamUrun <= 0 ? 0 : (double)KritikStok / ToplamUrun * 100;
        double normalYuzde = ToplamUrun <= 0 ? 0 : (double)Math.Max(0, ToplamUrun - KritikStok) / ToplamUrun * 100;

        KritikStokOrani = YuzdeyiGuvenliHaleGetir(kritikYuzde);
        NormalUrunOrani = YuzdeyiGuvenliHaleGetir(normalYuzde);
        KritikStokOraniMetni = $"%{KritikStokOrani:0.0}";
        NormalUrunOraniMetni = $"%{NormalUrunOrani:0.0}";
    }

    private static double YuzdeyiGuvenliHaleGetir(double deger)
    {
        if (double.IsNaN(deger) || double.IsInfinity(deger))
        {
            return 0;
        }

        return Math.Clamp(deger, 0, 100);
    }
}
