using System.Windows;
using System.Windows.Controls;
using StokTakip.Wpf;
using StokTakip.Wpf.ViewModels;

namespace StokTakip.Wpf.Views;

/// <summary>
/// Ana menü ekranında temel stok özetlerini kart düzeninde gösterir.
/// </summary>
public partial class DashboardView : UserControl
{
    private readonly DashboardViewModel _viewModel = new();

    /// <summary>
    /// Hızlı erişim kartlarından modül açılması istendiğinde ana shell bu olayı dinler (MainWindow’a sıkı referans yok).
    /// </summary>
    public event EventHandler<DashboardHizliErisimEventArgs>? HizliErisimTalepEdildi;

    public DashboardView()
    {
        InitializeComponent();
        DataContext = _viewModel;
        Loaded += DashboardView_Loaded;
    }

    /// <summary>
    /// Dashboard verilerini UI thread'i kilitlemeden arka planda yukler.
    /// </summary>
    private async void DashboardView_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= DashboardView_Loaded;

        try
        {
            await _viewModel.YukleAsync().ConfigureAwait(true);
        }
        catch
        {
            // Beklenmeyen hatalar App seviyesinde loglanir; ekran sifir degerlerle acik kalir.
        }
    }

    private void HizliErisimYayinla(DashboardHizliErisimHedef hedef) =>
        HizliErisimTalepEdildi?.Invoke(this, new DashboardHizliErisimEventArgs(hedef));

    // --- Hızlı erişim: tıklanınca hedef modülü event ile bildirir ---

    private void HizliErisimUrun_Click(object sender, RoutedEventArgs e) =>
        HizliErisimYayinla(DashboardHizliErisimHedef.UrunYonetimi);

    private void HizliErisimKategori_Click(object sender, RoutedEventArgs e) =>
        HizliErisimYayinla(DashboardHizliErisimHedef.KategoriYonetimi);

    private void HizliErisimStokGiris_Click(object sender, RoutedEventArgs e) =>
        HizliErisimYayinla(DashboardHizliErisimHedef.StokGiris);

    private void HizliErisimStokCikis_Click(object sender, RoutedEventArgs e) =>
        HizliErisimYayinla(DashboardHizliErisimHedef.StokCikis);

    private void HizliErisimKritik_Click(object sender, RoutedEventArgs e) =>
        HizliErisimYayinla(DashboardHizliErisimHedef.KritikStok);

    private void HizliErisimRapor_Click(object sender, RoutedEventArgs e) =>
        HizliErisimYayinla(DashboardHizliErisimHedef.Raporlama);
}
