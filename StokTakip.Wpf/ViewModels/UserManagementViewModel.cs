using System.Collections.ObjectModel;
using StokTakip.Business;
using StokTakip.Entities;

namespace StokTakip.Wpf.ViewModels;

/// <summary>
/// Kullanıcı yönetimi ekranında kullanıcı listesini Business katmanından hazırlar.
/// </summary>
public class UserManagementViewModel : ViewModelBase
{
    private readonly KullaniciManager _kullaniciManager = new();

    public ObservableCollection<Kullanici> Kullanicilar { get; } = new();

    public void Yukle()
    {
        Kullanicilar.Clear();

        try
        {
            foreach (var kullanici in _kullaniciManager.GetAll())
            {
                Kullanicilar.Add(kullanici);
            }
        }
        catch
        {
            // Veri alınamazsa WPF iskeleti boş listeyle çalışmaya devam eder.
        }
    }
}
