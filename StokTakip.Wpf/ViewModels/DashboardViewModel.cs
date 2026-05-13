using StokTakip.Business;

namespace StokTakip.Wpf.ViewModels;

/// <summary>
/// Ana menu kartlarinda gosterilecek ozet stok degerlerini Business katmanindan yukler.
/// </summary>
public class DashboardViewModel : ViewModelBase
{
    private readonly RaporManager _raporManager = new();

    private int _toplamUrun;
    private int _kritikStok;
    private int _toplamStok;

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

    public string SaklamaAsistaniDurumu => "Hazır";

    public void Yukle()
    {
        try
        {
            ToplamUrun = _raporManager.ToplamUrunSayisiGetir();
            KritikStok = _raporManager.KritikStokUrunSayisiGetir();
            ToplamStok = _raporManager.ToplamStokMiktariGetir();
        }
        catch
        {
            ToplamUrun = 0;
            KritikStok = 0;
            ToplamStok = 0;
        }
    }
}
