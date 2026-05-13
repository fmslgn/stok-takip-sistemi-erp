using System.Collections.ObjectModel;
using StokTakip.Business;
using StokTakip.Entities;

namespace StokTakip.Wpf.ViewModels;

/// <summary>
/// Kategori yönetimi ekranında kategori listesini Business katmanı üzerinden yönetir.
/// </summary>
public class CategoryManagementViewModel : ViewModelBase
{
    private readonly KategoriManager _kategoriManager = new();

    public ObservableCollection<Kategori> Kategoriler { get; } = new();

    public void Yukle()
    {
        Kategoriler.Clear();

        try
        {
            foreach (var kategori in _kategoriManager.GetAll())
            {
                Kategoriler.Add(kategori);
            }
        }
        catch
        {
            // Bağlantı sorunu varsa ekran boş listeyle açılabilir.
        }
    }
}
