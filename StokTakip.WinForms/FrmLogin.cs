using System.Drawing;
using StokTakip.Business;
using StokTakip.Entities;

namespace StokTakip.WinForms;

/// <summary>
/// Kullanici girisinin yapilacagi ilk formdur.
/// </summary>
public class FrmLogin : Form
{
    private readonly KullaniciManager _kullaniciManager = new();
    private readonly TextBox _txtKullaniciAdi = new();
    private readonly TextBox _txtSifre = new();

    public FrmLogin()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Giris ekranindaki temel kontrolleri hazirlar.
    /// </summary>
    private void InitializeComponent()
    {
        Text = "Stok Takip Sistemi - Giris";
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(360, 230);

        var lblBaslik = new Label { Text = "StokTakipSistemi", Location = new Point(30, 20), AutoSize = true, Font = new Font(FontFamily.GenericSansSerif, 14, FontStyle.Bold) };
        var lblKullaniciAdi = new Label { Text = "Kullanici Adi", Location = new Point(30, 70), AutoSize = true };
        var lblSifre = new Label { Text = "Sifre", Location = new Point(30, 110), AutoSize = true };
        var btnGiris = new Button { Text = "Giris Yap", Location = new Point(130, 150), Size = new Size(110, 32) };

        _txtKullaniciAdi.Location = new Point(130, 66);
        _txtKullaniciAdi.Size = new Size(170, 27);
        _txtSifre.Location = new Point(130, 106);
        _txtSifre.Size = new Size(170, 27);
        _txtSifre.UseSystemPasswordChar = true;

        btnGiris.Click += BtnGiris_Click;
        Controls.AddRange(new Control[] { lblBaslik, lblKullaniciAdi, _txtKullaniciAdi, lblSifre, _txtSifre, btnGiris });
    }

    /// <summary>
    /// Giris butonuna basilinca kullanici bilgisi Business katmanina aktarilir.
    /// </summary>
    private void BtnGiris_Click(object? sender, EventArgs e)
    {
        // Bu asamada gercek giris kontrolu yok; katman akisini gostermek icin Manager cagrilir.
        _kullaniciManager.Hazirla(new Kullanici { KullaniciAdi = _txtKullaniciAdi.Text, Sifre = _txtSifre.Text });

        Hide();
        using var anaMenu = new FrmAnaMenu();
        anaMenu.ShowDialog();
        Close();
    }
}
