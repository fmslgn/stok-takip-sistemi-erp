using System.Collections.ObjectModel;
using StokTakip.Business;
using StokTakip.Entities;

namespace StokTakip.Wpf.ViewModels;

/// <summary>
/// Stok çıkış ekranında ürünleri ve çıkış hareketlerini Business katmanından yükler.
/// </summary>
public class StockExitViewModel : ViewModelBase
{
    private readonly UrunManager _urunManager = new();
    private readonly StokCikisManager _stokCikisManager = new();

    public ObservableCollection<Urun> Urunler { get; } = new();
    public ObservableCollection<StokCikis> StokCikislari { get; } = new();

    public void Yukle()
    {
        Urunler.Clear();
        StokCikislari.Clear();

        try
        {
            foreach (var urun in _urunManager.GetAll())
            {
                Urunler.Add(urun);
            }

            foreach (var hareket in _stokCikisManager.GetAll())
            {
                StokCikislari.Add(hareket);
            }
        }
        catch
        {
            // Veri alınamazsa ekran form iskeletiyle çalışmaya devam eder.
        }
    }

    public void StokCikisiKaydet(Urun? urun, int miktar, string aciklama, DateTime tarih)
    {
        if (urun is null)
        {
            throw new ArgumentException("Ürün seçilmelidir.");
        }

        // Yetersiz stok kontrolü ve stok azaltma Business katmanındaki manager içinde yapılır.
        _stokCikisManager.Add(new StokCikis
        {
            UrunId = urun.Id,
            KullaniciId = 1,
            Miktar = miktar,
            Aciklama = aciklama,
            CikisTarihi = tarih
        });

        Yukle();
    }
}
