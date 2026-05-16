using System.Collections.ObjectModel;
using System.Windows.Input;
using StokTakip.Business;
using StokTakip.Entities;
using StokTakip.Wpf.Helpers;

namespace StokTakip.Wpf.ViewModels;

/// <summary>
/// Kritik stok ekranında kritik seviyeye düşen ürünleri Business katmanından getirir.
/// </summary>
public class CriticalStockViewModel : ViewModelBase
{
    private readonly UrunManager _urunManager = new();

    public CriticalStockViewModel()
    {
        // Liste yenileme veritabani cagrisini UI thread disina tasir.
        YenileCommand = new AsyncRelayCommand(YukleAsync, () => !IsBusy);
    }

    public ICommand YenileCommand { get; }

    public ObservableCollection<Urun> KritikUrunler { get; } = new();

    /// <summary>
    /// Kritik stoktaki urunleri async yukler; baglanti hatasinda liste bos kalir.
    /// </summary>
    public async Task YukleAsync()
    {
        IsBusy = true;
        try
        {
            var liste = await Task.Run(() =>
            {
                try
                {
                    return _urunManager.GetKritikStoktakiler().ToList();
                }
                catch
                {
                    return new List<Urun>();
                }
            }).ConfigureAwait(true);

            KritikUrunler.Clear();
            foreach (var urun in liste)
            {
                KritikUrunler.Add(urun);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }
}
