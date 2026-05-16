using System.Collections.Generic;
using System.Windows;
using StokTakip.Entities;
using StokTakip.Wpf.Helpers;

namespace StokTakip.Wpf.Views;

/// <summary>
/// Ana ekrandaki filtrelenmiş ürün listesini salt okunur ve geniş bir pencerede gösterir; CRUD ve seçim akışını ana ekrana bırakır.
/// </summary>
public partial class ProductListPreviewWindow : Window
{
    private readonly IEnumerable<Urun> _urunler;

    public ProductListPreviewWindow(IEnumerable<Urun> urunler)
    {
        InitializeComponent();
        _urunler = urunler;
        UrunGrid.ItemsSource = urunler;
    }

    private void BtnKapat_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    /// <summary>Önizlemedeki ürün listesini PDF olarak dışa aktarır.</summary>
    private void BtnPdfKaydet_Click(object sender, RoutedEventArgs e)
    {
        PdfExportHelper.UrunListesiniPdfKaydet(this, _urunler, "Ürün listesi önizleme penceresinden dışa aktarılmıştır.");
    }
}
