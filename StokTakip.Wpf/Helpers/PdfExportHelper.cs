using System.Globalization;
using System.Windows;
using Microsoft.Win32;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using StokTakip.Entities;
using StokTakip.Wpf.ViewModels;

namespace StokTakip.Wpf.Helpers;

/// <summary>
/// WPF listelerinden PDF dosyası oluşturur; SaveFileDialog ve QuestPDF ile Türkçe metin ve tablo çıktısı üretir.
/// </summary>
public static class PdfExportHelper
{
    private const string BosListeMesaji = "PDF oluşturmak için listede veri bulunmalıdır.";
    private const string PdfHataMesaji = "PDF oluşturulurken bir hata oluştu.";
    private const string SistemBasligi = "SYA Stok Takip Sistemi";

    private static readonly IReadOnlyList<string> UrunSutunlari =
    [
        "ID", "Ürün Adı", "Barkod", "Birim", "Stok", "Kritik Stok", "Alış", "Satış", "Aktif", "Açıklama"
    ];

    private static readonly IReadOnlyList<string> KullaniciSutunlari =
    [
        "ID", "Kullanıcı Adı", "Ad Soyad", "Rol", "Aktif", "Oluşturma Tarihi"
    ];

    private static readonly IReadOnlyList<string> KategoriSutunlari =
    [
        "ID", "Kategori Adı", "Açıklama", "Aktif", "Oluşturma Tarihi"
    ];

    private static readonly IReadOnlyList<string> StokGirisSutunlari =
    [
        "ID", "Ürün ID", "Miktar", "Açıklama", "Giriş Tarihi"
    ];

    private static readonly IReadOnlyList<string> StokCikisSutunlari =
    [
        "ID", "Ürün ID", "Miktar", "Açıklama", "Çıkış Tarihi"
    ];

    /// <summary>
    /// Ürün listesini (ana ekran veya büyütülmüş penceredeki filtrelenmiş liste) PDF olarak kaydeder.
    /// </summary>
    public static void UrunListesiniPdfKaydet(Window owner, IEnumerable<Urun> urunler, string altAciklama)
    {
        var satirlar = urunler.Select(u => (IReadOnlyList<string>)UrunSatiriOlustur(u).ToList()).ToList();
        TabloPdfKaydet(owner, "UrunListesi", "Ürün Listesi", altAciklama, UrunSutunlari, satirlar);
    }

    /// <summary>
    /// Kullanıcı listesini PDF olarak kaydeder (şifre alanı dışarıda bırakılır).
    /// </summary>
    public static void KullaniciListesiniPdfKaydet(Window owner, IEnumerable<Kullanici> liste, string altAciklama)
    {
        var satirlar = liste.Select(k => (IReadOnlyList<string>)KullaniciSatiriOlustur(k).ToList()).ToList();
        TabloPdfKaydet(owner, "KullaniciListesi", "Kullanıcı Listesi", altAciklama, KullaniciSutunlari, satirlar);
    }

    /// <summary>
    /// Kategori listesini PDF olarak kaydeder.
    /// </summary>
    public static void KategoriListesiniPdfKaydet(Window owner, IEnumerable<Kategori> liste, string altAciklama)
    {
        var satirlar = liste.Select(k => (IReadOnlyList<string>)KategoriSatiriOlustur(k).ToList()).ToList();
        TabloPdfKaydet(owner, "KategoriListesi", "Kategori Listesi", altAciklama, KategoriSutunlari, satirlar);
    }

    /// <summary>
    /// Depoya yapılan stok giriş hareketlerini PDF olarak kaydeder.
    /// </summary>
    public static void StokGirisHareketleriniPdfKaydet(Window owner, IEnumerable<StokGiris> liste)
    {
        const string alt = "Depoya yapılan stok giriş kayıtları listesi.";
        var satirlar = liste.Select(h => (IReadOnlyList<string>)StokGirisSatiriOlustur(h).ToList()).ToList();
        TabloPdfKaydet(owner, "StokGirisHareketleri", "Stok Giriş Hareketleri", alt, StokGirisSutunlari, satirlar);
    }

    /// <summary>
    /// Depodan yapılan stok çıkış hareketlerini PDF olarak kaydeder.
    /// </summary>
    public static void StokCikisHareketleriniPdfKaydet(Window owner, IEnumerable<StokCikis> liste)
    {
        const string alt = "Depodan yapılan stok çıkış kayıtları listesi.";
        var satirlar = liste.Select(h => (IReadOnlyList<string>)StokCikisSatiriOlustur(h).ToList()).ToList();
        TabloPdfKaydet(owner, "StokCikisHareketleri", "Stok Çıkış Hareketleri", alt, StokCikisSutunlari, satirlar);
    }

    /// <summary>
    /// Kritik stoktaki ürünleri PDF olarak kaydeder.
    /// </summary>
    public static void KritikStokListesiniPdfKaydet(Window owner, IEnumerable<Urun> liste, string altAciklama)
    {
        var satirlar = liste.Select(u => (IReadOnlyList<string>)UrunSatiriOlustur(u).ToList()).ToList();
        TabloPdfKaydet(owner, "KritikStokListesi", "Kritik Stok Listesi", altAciklama, UrunSutunlari, satirlar);
    }

    /// <summary>
    /// Raporlama ekranındaki özet kartları ve oranları PDF olarak kaydeder.
    /// </summary>
    public static void RaporOzetiPdfKaydet(Window owner, ReportsViewModel vm)
    {
        try
        {
            var dlg = new SaveFileDialog
            {
                Title = "PDF kaydet",
                Filter = "PDF dosyası|*.pdf",
                DefaultExt = "pdf",
                FileName = "RaporOzeti"
            };

            if (dlg.ShowDialog(owner) != true)
            {
                return;
            }

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(36);
                    page.DefaultTextStyle(t => t.FontFamily("Segoe UI").FontSize(10));
                    page.Content().Column(col =>
                    {
                        col.Spacing(10);
                        col.Item().Text(SistemBasligi).FontSize(18).SemiBold().FontColor(Colors.Blue.Darken2);
                        col.Item().Text("Rapor Özeti").FontSize(14).SemiBold();
                        col.Item().Text($"Oluşturulma: {DateTime.Now:dd.MM.yyyy HH:mm:ss}").FontColor(Colors.Grey.Darken1);
                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

                        col.Item().Row(r =>
                        {
                            r.RelativeItem().Text("Toplam Ürün:");
                            r.AutoItem().Text(vm.ToplamUrun.ToString(CultureInfo.InvariantCulture)).SemiBold();
                        });
                        col.Item().Row(r =>
                        {
                            r.RelativeItem().Text("Kritik Stok:");
                            r.AutoItem().Text(vm.KritikStok.ToString(CultureInfo.InvariantCulture)).SemiBold();
                        });
                        col.Item().Row(r =>
                        {
                            r.RelativeItem().Text("Toplam Stok:");
                            r.AutoItem().Text(vm.ToplamStok.ToString(CultureInfo.InvariantCulture)).SemiBold();
                        });
                        col.Item().Row(r =>
                        {
                            r.RelativeItem().Text("Kritik stok oranı:");
                            r.AutoItem().Text(vm.KritikStokOraniMetni).SemiBold();
                        });
                        col.Item().Row(r =>
                        {
                            r.RelativeItem().Text("Normal ürün oranı:");
                            r.AutoItem().Text(vm.NormalUrunOraniMetni).SemiBold();
                        });
                        col.Item().Row(r =>
                        {
                            r.RelativeItem().Text("Son güncelleme tarihi:");
                            r.AutoItem().Text(vm.SonGuncelleme).SemiBold();
                        });
                    });
                });
            }).GeneratePdf(dlg.FileName);

            DialogHelper.ShowSuccess("PDF dosyası kaydedildi.", "PDF", owner);
        }
        catch (Exception)
        {
            DialogHelper.ShowWarning(PdfHataMesaji, "PDF", owner);
        }
    }

    private static void TabloPdfKaydet(
        Window owner,
        string varsayilanDosyaAdi,
        string bolumBasligi,
        string? altAciklama,
        IReadOnlyList<string> sutunBasliklari,
        IReadOnlyList<IReadOnlyList<string>> satirlar)
    {
        if (satirlar.Count == 0)
        {
            DialogHelper.ShowInfo(BosListeMesaji, bolumBasligi, owner);
            return;
        }

        try
        {
            var dlg = new SaveFileDialog
            {
                Title = "PDF kaydet",
                Filter = "PDF dosyası|*.pdf",
                DefaultExt = "pdf",
                FileName = varsayilanDosyaAdi
            };

            if (dlg.ShowDialog(owner) != true)
            {
                return;
            }

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(28);
                    page.DefaultTextStyle(t => t.FontFamily("Segoe UI").FontSize(9));
                    page.Content().Column(column =>
                    {
                        column.Spacing(8);
                        column.Item().Text(SistemBasligi).FontSize(17).SemiBold().FontColor(Colors.Blue.Darken2);
                        column.Item().Text(bolumBasligi).FontSize(13).SemiBold();
                        if (!string.IsNullOrWhiteSpace(altAciklama))
                        {
                            column.Item().Text(altAciklama).FontColor(Colors.Grey.Darken1);
                        }

                        column.Item().Text($"Oluşturulma: {DateTime.Now:dd.MM.yyyy HH:mm:ss}").FontColor(Colors.Grey.Medium);
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                foreach (var _ in sutunBasliklari)
                                {
                                    cols.RelativeColumn();
                                }
                            });

                            table.Header(header =>
                            {
                                foreach (var baslik in sutunBasliklari)
                                {
                                    header.Cell().Element(UstHucre).Text(baslik).SemiBold();
                                }
                            });

                            foreach (var satir in satirlar)
                            {
                                foreach (var hucre in satir)
                                {
                                    table.Cell().Element(VeriHucre).Text(hucre ?? string.Empty);
                                }
                            }
                        });
                    });
                });
            }).GeneratePdf(dlg.FileName);

            DialogHelper.ShowSuccess("PDF dosyası kaydedildi.", "PDF", owner);
        }
        catch (Exception)
        {
            DialogHelper.ShowWarning(PdfHataMesaji, bolumBasligi, owner);
        }
    }

    private static IContainer UstHucre(IContainer c) =>
        c.DefaultTextStyle(x => x.SemiBold()).Background(Colors.Grey.Lighten3).BorderBottom(1).BorderColor(Colors.Grey.Medium)
            .PaddingVertical(6).PaddingHorizontal(4);

    private static IContainer VeriHucre(IContainer c) =>
        c.BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4).PaddingHorizontal(4);

    private static string[] UrunSatiriOlustur(Urun u) =>
    [
        u.Id.ToString(CultureInfo.InvariantCulture),
        u.UrunAdi,
        u.Barkod,
        u.Birim,
        u.StokMiktari.ToString(CultureInfo.InvariantCulture),
        u.KritikStokSeviyesi.ToString(CultureInfo.InvariantCulture),
        u.AlisFiyati.ToString("N2", CultureInfo.GetCultureInfo("tr-TR")),
        u.SatisFiyati.ToString("N2", CultureInfo.GetCultureInfo("tr-TR")),
        u.AktifMi ? "Evet" : "Hayır",
        u.Aciklama
    ];

    private static string[] KullaniciSatiriOlustur(Kullanici k) =>
    [
        k.Id.ToString(CultureInfo.InvariantCulture),
        k.KullaniciAdi,
        k.AdSoyad,
        k.Rol,
        k.AktifMi ? "Evet" : "Hayır",
        k.OlusturmaTarihi.ToString("dd.MM.yyyy HH:mm", CultureInfo.GetCultureInfo("tr-TR"))
    ];

    private static string[] KategoriSatiriOlustur(Kategori k) =>
    [
        k.Id.ToString(CultureInfo.InvariantCulture),
        k.KategoriAdi,
        k.Aciklama,
        k.AktifMi ? "Evet" : "Hayır",
        k.OlusturmaTarihi.ToString("dd.MM.yyyy HH:mm", CultureInfo.GetCultureInfo("tr-TR"))
    ];

    private static string[] StokGirisSatiriOlustur(StokGiris h) =>
    [
        h.Id.ToString(CultureInfo.InvariantCulture),
        h.UrunId.ToString(CultureInfo.InvariantCulture),
        h.Miktar.ToString(CultureInfo.InvariantCulture),
        h.Aciklama,
        h.GirisTarihi.ToString("dd.MM.yyyy HH:mm", CultureInfo.GetCultureInfo("tr-TR"))
    ];

    private static string[] StokCikisSatiriOlustur(StokCikis h) =>
    [
        h.Id.ToString(CultureInfo.InvariantCulture),
        h.UrunId.ToString(CultureInfo.InvariantCulture),
        h.Miktar.ToString(CultureInfo.InvariantCulture),
        h.Aciklama,
        h.CikisTarihi.ToString("dd.MM.yyyy HH:mm", CultureInfo.GetCultureInfo("tr-TR"))
    ];
}
