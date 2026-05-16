using StokTakip.Business;

namespace StokTakip.Wpf.ViewModels;

/// <summary>
/// Ana menu kartlarinda gosterilecek ozet stok degerlerini Business katmanindan yukler.
/// Agir RaporManager cagrilari Task.Run ile arka planda calistirilarak UI thread donmasi onlenir.
/// </summary>
public class DashboardViewModel : ViewModelBase
{
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

    /// <summary>
    /// Dashboard kartlarini veritabanindan async yukler; baglanti hatasinda sifir degerleri gosterir.
    /// </summary>
    public async Task YukleAsync()
    {
        IsBusy = true;
        try
        {
            var rapor = new RaporManager();
            var sonuc = await Task.Run(() =>
            {
                try
                {
                    return (
                        rapor.ToplamUrunSayisiGetir(),
                        rapor.KritikStokUrunSayisiGetir(),
                        rapor.ToplamStokMiktariGetir());
                }
                catch
                {
                    return (0, 0, 0);
                }
            }).ConfigureAwait(true);

            ToplamUrun = sonuc.Item1;
            KritikStok = sonuc.Item2;
            ToplamStok = sonuc.Item3;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
