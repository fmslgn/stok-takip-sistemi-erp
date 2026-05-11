using System.Drawing;

namespace StokTakip.WinForms;

/// <summary>
/// Sistemdeki ana modullere gecis yapilan menudur.
/// </summary>
public class FrmAnaMenu : Form
{
    public FrmAnaMenu()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Ana menu butonlarini ve form duzenini hazirlar.
    /// </summary>
    private void InitializeComponent()
    {
        Text = "Stok Takip Sistemi - Ana Menu";
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(520, 360);

        var lblBaslik = new Label { Text = "ERP Stok Takip Modulleri", Location = new Point(24, 20), AutoSize = true, Font = new Font(FontFamily.GenericSansSerif, 13, FontStyle.Bold) };
        Controls.Add(lblBaslik);

        AddMenuButton("Kullanici Yonetimi", 24, 70, (_, _) => OpenForm(new FrmKullaniciYonetimi()));
        AddMenuButton("Kategori", 190, 70, (_, _) => OpenForm(new FrmKategori()));
        AddMenuButton("Urun Yonetimi", 356, 70, (_, _) => OpenForm(new FrmUrunYonetimi()));
        AddMenuButton("Stok Giris", 24, 125, (_, _) => OpenForm(new FrmStokGiris()));
        AddMenuButton("Stok Cikis", 190, 125, (_, _) => OpenForm(new FrmStokCikis()));
        AddMenuButton("Kritik Stok", 356, 125, (_, _) => OpenForm(new FrmKritikStok()));
        AddMenuButton("Raporlama", 24, 180, (_, _) => OpenForm(new FrmRaporlama()));
        AddMenuButton("Saklama Kosulu AI", 190, 180, (_, _) => OpenForm(new FrmSaklamaKosuluAI()));
    }

    /// <summary>
    /// Ana menude tekrar eden buton olusturma islemini merkezi hale getirir.
    /// </summary>
    private void AddMenuButton(string text, int x, int y, EventHandler clickHandler)
    {
        var button = new Button { Text = text, Location = new Point(x, y), Size = new Size(140, 36) };
        button.Click += clickHandler;
        Controls.Add(button);
    }

    /// <summary>
    /// Secilen modul formunu modal olarak acar.
    /// </summary>
    private static void OpenForm(Form form)
    {
        // Ana menuden ilgili modulu acma islemi burada yapilir.
        using (form)
        {
            form.ShowDialog();
        }
    }
}
