using System.Windows;
using System.Windows.Controls;
using StokTakip.Wpf.ViewModels;

namespace StokTakip.Wpf.Views;

/// <summary>
/// WPF raporlama ekranında sayısal kartları, durum mesajını ve görsel rapor alanını gösterir.
/// </summary>
public partial class ReportsView : UserControl
{
    private readonly ReportsViewModel _viewModel = new();

    public ReportsView()
    {
        InitializeComponent();
        DataContext = _viewModel;

        try
        {
            // Rapor ekranı açılır açılmaz değerler Business katmanından güvenli şekilde yüklenir.
            _viewModel.RaporlariYukle();
        }
        catch
        {
            MessageBox.Show("Raporlama ekranı açılırken bir hata oluştu.", "Raporlama", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
