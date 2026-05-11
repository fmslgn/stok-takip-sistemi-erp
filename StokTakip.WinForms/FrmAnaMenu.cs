using System.Drawing;
using StokTakip.Entities;

namespace StokTakip.WinForms;

/// <summary>
/// Sistemdeki ana modullere gecis yapilan menudur.
/// </summary>
public class FrmAnaMenu : Form
{
    private readonly Kullanici? _aktifKullanici;

    public FrmAnaMenu(Kullanici? aktifKullanici = null)
    {
        _aktifKullanici = aktifKullanici;
        InitializeComponent();
    }

    /// <summary>
    /// Ana menu butonlarini ve form duzenini hazirlar.
    /// </summary>
    private void InitializeComponent()
    {
        Text = "SYA Stok Takip Sistemi";
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(620, 360);

        var lblBaslik = new Label { Text = "SYA Stok Takip Sistemi", Location = new Point(24, 20), AutoSize = true, Font = new Font(FontFamily.GenericSansSerif, 15, FontStyle.Bold) };
        var kullaniciAdi = _aktifKullanici?.KullaniciAdi ?? "admin";
        var lblKullanici = new Label { Text = $"Hos geldiniz: {kullaniciAdi}", Location = new Point(24, 58), AutoSize = true };

        Controls.AddRange(new Control[] { lblBaslik, lblKullanici });

        AddMenuButton("Kullanici Yonetimi", 24, 105, (_, _) => OpenForm(new FrmKullaniciYonetimi()));
        AddMenuButton("Kategori Yonetimi", 214, 105, (_, _) => OpenForm(new FrmKategori()));
        AddMenuButton("Urun Yonetimi", 404, 105, (_, _) => OpenForm(new FrmUrunYonetimi()));
        AddMenuButton("Stok Giris", 24, 160, (_, _) => OpenForm(new FrmStokGiris(_aktifKullanici?.Id ?? 1)));
        AddMenuButton("Stok Cikis", 214, 160, (_, _) => OpenForm(new FrmStokCikis(_aktifKullanici?.Id ?? 1)));
        AddMenuButton("Kritik Stok", 404, 160, (_, _) => OpenForm(new FrmKritikStok()));
        AddMenuButton("Raporlama", 24, 215, (_, _) => OpenForm(new FrmRaporlama()));
        AddMenuButton("Saklama Kosulu Oneri", 214, 215, (_, _) => OpenForm(new FrmSaklamaKosuluAI()));
        AddMenuButton("Cikis", 404, 215, (_, _) => Close());
    }

    /// <summary>
    /// Ana menude tekrar eden buton olusturma islemini merkezi hale getirir.
    /// </summary>
    private void AddMenuButton(string text, int x, int y, EventHandler clickHandler)
    {
        var button = new Button { Text = text, Location = new Point(x, y), Size = new Size(165, 38) };
        button.Click += clickHandler;
        Controls.Add(button);
    }

    /// <summary>
    /// Secilen modul formunu modal olarak acar.
    /// </summary>
    private static void OpenForm(Form form)
    {
        using (form)
        {
            form.ShowDialog();
        }
    }
}
