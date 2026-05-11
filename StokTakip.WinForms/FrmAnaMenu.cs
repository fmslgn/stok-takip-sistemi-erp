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
    /// Sidebar ve modul butonlari ile ana menu duzenini hazirlar.
    /// </summary>
    private void InitializeComponent()
    {
        WinFormsUiHelper.ApplyFormStyle(this, "SYA Stok Takip Sistemi");
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(920, 560);

        var sidebar = new Panel
        {
            Dock = DockStyle.Left,
            Width = 245,
            BackColor = WinFormsUiHelper.Navy
        };

        var lblBrand = new Label
        {
            Text = "SYA",
            Location = new Point(24, 26),
            AutoSize = true,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 28F, FontStyle.Bold)
        };

        var lblSubtitle = new Label
        {
            Text = "Stoklarını düzenle,\nişini kolaylaştır.",
            Location = new Point(28, 94),
            AutoSize = true,
            ForeColor = Color.FromArgb(214, 229, 245),
            Font = new Font("Segoe UI", 10F)
        };

        var kullaniciAdi = _aktifKullanici?.KullaniciAdi ?? "admin";
        var lblKullanici = new Label
        {
            Text = $"Hoş geldiniz:\n{kullaniciAdi}",
            Location = new Point(28, 450),
            AutoSize = true,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold)
        };

        sidebar.Controls.AddRange(new Control[] { lblBrand, lblSubtitle, lblKullanici });

        var lblBaslik = new Label
        {
            Text = "SYA Stok Takip Sistemi",
            Location = new Point(285, 34),
            AutoSize = true,
            ForeColor = WinFormsUiHelper.Navy,
            Font = new Font("Segoe UI", 20F, FontStyle.Bold)
        };

        var lblAciklama = new Label
        {
            Text = "Modüller üzerinden stok, ürün, rapor ve saklama koşulu süreçlerini yönetin.",
            Location = new Point(288, 80),
            AutoSize = true,
            ForeColor = Color.FromArgb(71, 85, 105),
            Font = new Font("Segoe UI", 10F)
        };

        AddMenuButton("Kullanıcı Yönetimi", 290, 140, (_, _) => OpenForm(new FrmKullaniciYonetimi()));
        AddMenuButton("Kategori Yönetimi", 495, 140, (_, _) => OpenForm(new FrmKategori()));
        AddMenuButton("Ürün Yönetimi", 700, 140, (_, _) => OpenForm(new FrmUrunYonetimi()));
        AddMenuButton("Stok Giriş", 290, 215, (_, _) => OpenForm(new FrmStokGiris(_aktifKullanici?.Id ?? 1)));
        AddMenuButton("Stok Çıkış", 495, 215, (_, _) => OpenForm(new FrmStokCikis(_aktifKullanici?.Id ?? 1)));
        AddMenuButton("Kritik Stok", 700, 215, (_, _) => OpenForm(new FrmKritikStok()), true);
        AddMenuButton("Raporlama", 290, 290, (_, _) => OpenForm(new FrmRaporlama()));
        AddMenuButton("Saklama Koşulu Öneri", 495, 290, (_, _) => OpenForm(new FrmSaklamaKosuluAI()));
        AddMenuButton("Çıkış", 700, 290, (_, _) => Close(), false, true);

        Controls.AddRange(new Control[] { sidebar, lblBaslik, lblAciklama });
    }

    /// <summary>
    /// Ana menude tekrar eden buton olusturma islemini merkezi hale getirir.
    /// </summary>
    private void AddMenuButton(string text, int x, int y, EventHandler clickHandler, bool warning = false, bool danger = false)
    {
        var button = new Button { Text = text, Location = new Point(x, y), Size = new Size(165, 52) };
        button.Click += clickHandler;

        if (danger)
        {
            WinFormsUiHelper.StyleDangerButton(button);
        }
        else if (warning)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.BackColor = WinFormsUiHelper.Warning;
            button.ForeColor = Color.White;
            button.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button.FlatAppearance.BorderColor = WinFormsUiHelper.Warning;
        }
        else
        {
            WinFormsUiHelper.StylePrimaryButton(button);
        }

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
