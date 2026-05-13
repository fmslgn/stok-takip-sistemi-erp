using System.Collections.ObjectModel;
using StokTakip.Business;
using StokTakip.Entities;

namespace StokTakip.Wpf.ViewModels;

/// <summary>
/// Stok giriş ekranında ürünleri ve giriş hareketlerini Business katmanından yükler.
/// </summary>
public class StockEntryViewModel : ViewModelBase
{
    private readonly UrunManager _urunManager = new();
    private readonly StokGirisManager _stokGirisManager = new();

    public ObservableCollection<Urun> Urunler { get; } = new();
    public ObservableCollection<StokGiris> StokGirisleri { get; } = new();

    public void Yukle()
    {
        Urunler.Clear();
        StokGirisleri.Clear();

        try
        {
            foreach (var urun in _urunManager.GetAll())
            {
                Urunler.Add(urun);
            }

            foreach (var hareket in _stokGirisManager.GetAll())
            {
                StokGirisleri.Add(hareket);
            }
        }
        catch
        {
            // Veri alınamazsa ekran form iskeletiyle çalışmaya devam eder.
        }
    }

    public void StokGirisiKaydet(Urun? urun, int miktar, string aciklama, DateTime tarih)
    {
        if (urun is null)
        {
            throw new ArgumentException("Ürün seçilmelidir.");
        }

        // Stok miktarı artırma ve hareket kaydı Business katmanındaki manager içinde yapılır.
        _stokGirisManager.Add(new StokGiris
        {
            UrunId = urun.Id,
            KullaniciId = 1,
            Miktar = miktar,
            Aciklama = aciklama,
            GirisTarihi = tarih
        });

        Yukle();
    }
}
