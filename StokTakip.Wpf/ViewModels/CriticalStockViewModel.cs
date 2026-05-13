using System.Collections.ObjectModel;
using StokTakip.Business;
using StokTakip.Entities;

namespace StokTakip.Wpf.ViewModels;

/// <summary>
/// Kritik stok ekranında kritik seviyeye düşen ürünleri Business katmanından getirir.
/// </summary>
public class CriticalStockViewModel : ViewModelBase
{
    private readonly UrunManager _urunManager = new();

    public ObservableCollection<Urun> KritikUrunler { get; } = new();

    public void Yukle()
    {
        KritikUrunler.Clear();

        try
        {
            foreach (var urun in _urunManager.GetKritikStoktakiler())
            {
                KritikUrunler.Add(urun);
            }
        }
        catch
        {
            // Bağlantı sorunu varsa ekran boş listeyle çalışmaya devam eder.
        }
    }
}
