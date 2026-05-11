using System.Drawing;
using StokTakip.Business;
using StokTakip.Entities;

namespace StokTakip.WinForms;

/// <summary>
/// Kullanici girisinin yapildigi ilk formdur.
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
        Text = "SYA Stok Takip Sistemi - Giriş";
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(380, 250);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        var lblBaslik = new Label { Text = "SYA Stok Takip Sistemi", Location = new Point(30, 24), AutoSize = true, Font = new Font(FontFamily.GenericSansSerif, 14, FontStyle.Bold) };
        var lblKullaniciAdi = new Label { Text = "Kullanıcı Adı", Location = new Point(30, 82), AutoSize = true };
        var lblSifre = new Label { Text = "Şifre", Location = new Point(30, 122), AutoSize = true };
        var btnGiris = new Button { Text = "Giriş Yap", Location = new Point(140, 166), Size = new Size(100, 32) };
        var btnCikis = new Button { Text = "Çıkış", Location = new Point(250, 166), Size = new Size(80, 32) };

        _txtKullaniciAdi.Location = new Point(140, 78);
        _txtKullaniciAdi.Size = new Size(190, 27);
        _txtSifre.Location = new Point(140, 118);
        _txtSifre.Size = new Size(190, 27);
        _txtSifre.PasswordChar = '*';

        btnGiris.Click += BtnGiris_Click;
        btnCikis.Click += BtnCikis_Click;
        AcceptButton = btnGiris;
        CancelButton = btnCikis;

        Controls.AddRange(new Control[] { lblBaslik, lblKullaniciAdi, _txtKullaniciAdi, lblSifre, _txtSifre, btnGiris, btnCikis });
    }

    /// <summary>
    /// Giris butonunda kullanici bilgileri Business katmaninda kontrol edilir.
    /// </summary>
    private void BtnGiris_Click(object? sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_txtKullaniciAdi.Text) || string.IsNullOrWhiteSpace(_txtSifre.Text))
            {
                WinFormsUiHelper.ShowError("Kullanıcı adı ve şifre boş bırakılamaz.");
                return;
            }

            Kullanici? kullanici = _kullaniciManager.LoginKontrol(_txtKullaniciAdi.Text, _txtSifre.Text);
            if (kullanici is null)
            {
                WinFormsUiHelper.ShowError("Kullanıcı adı veya şifre hatalı.");
                return;
            }

            Hide();
            using var anaMenu = new FrmAnaMenu(kullanici);
            anaMenu.ShowDialog();
            Close();
        }
        catch (Exception ex)
        {
            WinFormsUiHelper.ShowError(ex.Message);
        }
    }

    /// <summary>
    /// Cikis butonu uygulamayi kapatir.
    /// </summary>
    private void BtnCikis_Click(object? sender, EventArgs e)
    {
        Close();
    }
}
