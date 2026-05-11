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
    /// Profesyonel giris ekranindaki kontrolleri hazirlar.
    /// </summary>
    private void InitializeComponent()
    {
        WinFormsUiHelper.ApplyFormStyle(this, "SYA Stok Takip Sistemi - Giriş");
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(760, 420);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        var brandPanel = new Panel
        {
            Dock = DockStyle.Left,
            Width = 300,
            BackColor = WinFormsUiHelper.Navy
        };

        var lblBrand = new Label
        {
            Text = "SYA",
            Location = new Point(38, 92),
            AutoSize = true,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 34F, FontStyle.Bold)
        };

        var lblTitle = new Label
        {
            Text = "Stok Takip Sistemi",
            Location = new Point(42, 160),
            AutoSize = true,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 15F, FontStyle.Bold)
        };

        var lblSlogan = new Label
        {
            Text = "Stoklarını düzenle,\nişini kolaylaştır.",
            Location = new Point(44, 205),
            AutoSize = true,
            ForeColor = Color.FromArgb(214, 229, 245),
            Font = new Font("Segoe UI", 11F)
        };

        brandPanel.Controls.AddRange(new Control[] { lblBrand, lblTitle, lblSlogan });

        var loginPanel = new Panel
        {
            Location = new Point(350, 54),
            Size = new Size(360, 300),
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        };

        var lblLoginTitle = new Label
        {
            Text = "Yönetici Girişi",
            Location = new Point(28, 24),
            AutoSize = true,
            ForeColor = WinFormsUiHelper.Navy,
            Font = new Font("Segoe UI", 15F, FontStyle.Bold)
        };

        var lblKullaniciAdi = new Label { Text = "Kullanıcı Adı", Location = new Point(30, 82), AutoSize = true };
        var lblSifre = new Label { Text = "Şifre", Location = new Point(30, 142), AutoSize = true };
        var btnGiris = new Button { Text = "Giriş Yap", Location = new Point(30, 215), Size = new Size(140, 38) };
        var btnCikis = new Button { Text = "Çıkış", Location = new Point(185, 215), Size = new Size(110, 38) };

        _txtKullaniciAdi.Location = new Point(30, 104);
        _txtKullaniciAdi.Size = new Size(270, 27);
        _txtSifre.Location = new Point(30, 164);
        _txtSifre.Size = new Size(270, 27);
        _txtSifre.PasswordChar = '*';

        btnGiris.Click += BtnGiris_Click;
        btnCikis.Click += BtnCikis_Click;
        AcceptButton = btnGiris;
        CancelButton = btnCikis;

        WinFormsUiHelper.StyleTextBox(_txtKullaniciAdi);
        WinFormsUiHelper.StyleTextBox(_txtSifre);
        WinFormsUiHelper.StylePrimaryButton(btnGiris);
        WinFormsUiHelper.StyleSecondaryButton(btnCikis);

        loginPanel.Controls.AddRange(new Control[] { lblLoginTitle, lblKullaniciAdi, _txtKullaniciAdi, lblSifre, _txtSifre, btnGiris, btnCikis });
        Controls.AddRange(new Control[] { brandPanel, loginPanel });
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
