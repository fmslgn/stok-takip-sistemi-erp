using System.Collections.ObjectModel;
using StokTakip.Business;
using StokTakip.Entities;

namespace StokTakip.Wpf.ViewModels;

/// <summary>
/// Urun yonetimi WPF ekraninda kategori, saklama kosulu ve urun listesini Business katmani uzerinden hazirlar.
/// </summary>
public class ProductManagementViewModel : ViewModelBase
{
    private readonly UrunManager _urunManager = new();
    private readonly KategoriManager _kategoriManager = new();
    private readonly SaklamaKosuluManager _saklamaKosuluManager = new();

    public ObservableCollection<Urun> Urunler { get; } = new();
    public ObservableCollection<Kategori> Kategoriler { get; } = new();
    public ObservableCollection<SaklamaKosulu> SaklamaKosullari { get; } = new();

    public void Yukle()
    {
        Kategoriler.Clear();
        SaklamaKosullari.Clear();
        Urunler.Clear();

        try
        {
            foreach (var kategori in _kategoriManager.GetAll())
            {
                Kategoriler.Add(kategori);
            }

            foreach (var kosul in _saklamaKosuluManager.GetAll())
            {
                SaklamaKosullari.Add(kosul);
            }

            foreach (var urun in _urunManager.GetAll())
            {
                Urunler.Add(urun);
            }
        }
        catch
        {
            // Ilk WPF iskeletinde veri alinamazsa ekran bos listeyle calismaya devam eder.
        }
    }
}
