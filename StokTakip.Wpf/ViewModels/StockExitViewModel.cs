using System.Collections.ObjectModel;
using StokTakip.Business;
using StokTakip.Entities;

namespace StokTakip.Wpf.ViewModels;

/// <summary>
/// Stok çıkış ekranında ürünleri ve çıkış hareketlerini Business katmanından yükler.
/// Veri cekme islemleri Task.Run ile arka planda yapilir.
/// </summary>
public class StockExitViewModel : ViewModelBase
{
    private readonly UrunManager _urunManager = new();
    private readonly StokCikisManager _stokCikisManager = new();

    public ObservableCollection<Urun> Urunler { get; } = new();
    public ObservableCollection<StokCikis> StokCikislari { get; } = new();

    /// <summary>
    /// Urun ve stok cikis listelerini async yukler; baglanti hatasinda listeler bos kalir.
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
                    return (_urunManager.GetAll().ToList(), _stokCikisManager.GetAll().ToList());
                }
                catch
                {
                    return (new List<Urun>(), new List<StokCikis>());
                }
            }).ConfigureAwait(true);

            Urunler.Clear();
            foreach (var urun in sonuc.Item1)
            {
                Urunler.Add(urun);
            }

            StokCikislari.Clear();
            foreach (var hareket in sonuc.Item2)
            {
                StokCikislari.Add(hareket);
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
    /// Stok cikisi kaydeder ve listeyi yeniden yukler.
    /// </summary>
    public async Task StokCikisiKaydetAsync(Urun? urun, int miktar, string aciklama, DateTime tarih)
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
                _stokCikisManager.Add(new StokCikis
                {
                    UrunId = urun.Id,
                    KullaniciId = 1,
                    Miktar = miktar,
                    Aciklama = aciklama,
                    CikisTarihi = tarih
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
