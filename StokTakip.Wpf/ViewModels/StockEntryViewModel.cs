using System.Collections.ObjectModel;
using StokTakip.Business;
using StokTakip.Entities;

namespace StokTakip.Wpf.ViewModels;

/// <summary>
/// Stok giriş ekranında ürünleri ve giriş hareketlerini Business katmanından yükler.
/// Veri cekme islemleri Task.Run ile arka planda yapilir.
/// </summary>
public class StockEntryViewModel : ViewModelBase
{
    private readonly UrunManager _urunManager = new();
    private readonly StokGirisManager _stokGirisManager = new();

    public ObservableCollection<Urun> Urunler { get; } = new();
    public ObservableCollection<StokGiris> StokGirisleri { get; } = new();

    /// <summary>
    /// Urun ve stok giris listelerini async yukler; baglanti hatasinda listeler bos kalir.
    /// </summary>
    /// <param name="isBusyYonet">False ise IsBusy bayragi dis katman tarafindan yonetilir.</param>
    public async Task YukleAsync(bool isBusyYonet = true)
    {
        if (isBusyYonet)
        {
            IsBusy = true;
        }

        try
        {
            var sonuc = await Task.Run(() =>
            {
                try
                {
                    return (_urunManager.GetAll().ToList(), _stokGirisManager.GetAll().ToList());
                }
                catch
                {
                    return (new List<Urun>(), new List<StokGiris>());
                }
            }).ConfigureAwait(true);

            Urunler.Clear();
            foreach (var urun in sonuc.Item1)
            {
                Urunler.Add(urun);
            }

            StokGirisleri.Clear();
            foreach (var hareket in sonuc.Item2)
            {
                StokGirisleri.Add(hareket);
            }
        }
        finally
        {
            if (isBusyYonet)
            {
                IsBusy = false;
            }
        }
    }

    /// <summary>
    /// Stok girisi kaydeder ve listeyi yeniden yukler.
    /// </summary>
    public async Task StokGirisiKaydetAsync(Urun? urun, int miktar, string aciklama, DateTime tarih)
    {
        if (urun is null)
        {
            throw new ArgumentException("Lütfen ürün seçiniz.");
        }

        IsBusy = true;
        try
        {
            await Task.Run(() =>
            {
                _stokGirisManager.Add(new StokGiris
                {
                    UrunId = urun.Id,
                    KullaniciId = 1,
                    Miktar = miktar,
                    Aciklama = aciklama,
                    GirisTarihi = tarih
                });
            }).ConfigureAwait(true);

            await YukleAsync(isBusyYonet: false).ConfigureAwait(true);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
