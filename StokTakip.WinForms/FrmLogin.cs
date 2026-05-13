using System;
using System.Drawing;
using System.Windows.Forms;
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
        ClientSize = new Size(1180, 620);
        MinimumSize = new Size(1180, 620);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        BackColor = Color.FromArgb(245, 248, 252);
        Font = ModernUi.UiFont(9f, FontStyle.Regular);

        BuildLoginScreen();
    }

    private void BuildLoginScreen()
    {
        Controls.Clear();

        BuildLeftInfoArea();
        BuildLoginCard();
    }

    private void BuildLeftInfoArea()
    {
        var appBadge = ModernUi.CardPanel(70, 75, 230, 44);
        appBadge.BackColor = Color.White;
        Controls.Add(appBadge);

        appBadge.Controls.Add(ModernUi.Label(
            "▧",
            20,
            12,
            22,
            20,
            10.5f,
            FontStyle.Bold,
            ModernUi.Accent));

        appBadge.Controls.Add(ModernUi.Label(
            "Stok Takip Uygulaması",
            52,
            12,
            160,
            20,
            9f,
            FontStyle.Regular,
            ModernUi.Dark));

        Controls.Add(ModernUi.Label(
            "SYA ile stok süreçlerini",
            70,
            158,
            560,
            58,
            27f,
            FontStyle.Bold,
            ModernUi.Dark));

        Controls.Add(ModernUi.Label(
            "tek ekrandan yönet.",
            70,
            218,
            560,
            58,
            27f,
            FontStyle.Bold,
            ModernUi.Dark));

        Controls.Add(ModernUi.Label(
            "Bu tasarım, kullanıcı girişi, ürün yönetimi, stok giriş-çıkış, kritik stok kontrolü ve",
            72,
            310,
            620,
            24,
            9.6f,
            FontStyle.Regular,
            ModernUi.Muted));

        Controls.Add(ModernUi.Label(
            "raporlama akışlarını sade ve kullanıcı dostu bir yapı ile göstermektedir.",
            72,
            338,
            620,
            24,
            9.6f,
            FontStyle.Regular,
            ModernUi.Muted));

        Controls.Add(CreateFeatureChip("▣", "Kolay kullanım", 70, 410));
        Controls.Add(CreateFeatureChip("↻", "Anlık stok takibi", 275, 410));
        Controls.Add(CreateFeatureChip("◇", "Düzenli yönetim", 480, 410));
    }

    private void BuildLoginCard()
    {
        var loginCard = ModernUi.CardPanel(690, 42, 420, 525);
        loginCard.BackColor = Color.White;
        Controls.Add(loginCard);

        loginCard.Controls.Add(ModernUi.Label(
            "Kullanıcı Giriş Ekranı",
            36,
            34,
            340,
            36,
            17f,
            FontStyle.Bold,
            ModernUi.Dark));

        loginCard.Controls.Add(ModernUi.Label(
            "Bilgilerinizi girerek sisteme güvenli şekilde erişebilirsiniz.",
            36,
            76,
            350,
            22,
            8.8f,
            FontStyle.Regular,
            ModernUi.Muted));

        loginCard.Controls.Add(ModernUi.Label(
            "Kullanıcı Adı",
            36,
            118,
            180,
            20,
            9f,
            FontStyle.Bold,
            ModernUi.Dark));

        var userInputPanel = CreateInputPanel(36, 143, 348, 44);
        loginCard.Controls.Add(userInputPanel);

        userInputPanel.Controls.Add(ModernUi.Label(
            "♙",
            17,
            12,
            20,
            20,
            10f,
            FontStyle.Bold,
            ModernUi.Muted));

        PrepareTextBox(_txtKullaniciAdi, 48, 10, 282, 24, "Kullanıcı adınızı giriniz");
        userInputPanel.Controls.Add(_txtKullaniciAdi);

        loginCard.Controls.Add(ModernUi.Label(
            "Şifre",
            36,
            205,
            180,
            20,
            9f,
            FontStyle.Bold,
            ModernUi.Dark));

        var passwordInputPanel = CreateInputPanel(36, 230, 348, 44);
        loginCard.Controls.Add(passwordInputPanel);

        passwordInputPanel.Controls.Add(ModernUi.Label(
            "▣",
            17,
            12,
            20,
            20,
            9f,
            FontStyle.Bold,
            ModernUi.Muted));

        PrepareTextBox(_txtSifre, 48, 10, 282, 24, "Şifrenizi giriniz");
        _txtSifre.PasswordChar = '*';
        passwordInputPanel.Controls.Add(_txtSifre);

        BuildSecurityInfo(loginCard);
        var btnGiris = ModernUi.Button("↪  Giriş Yap", 36, 382, 140, 42, true);
        btnGiris.Font = ModernUi.UiFont(9.2f, FontStyle.Bold);
        btnGiris.Click += BtnGiris_Click;
        loginCard.Controls.Add(btnGiris);

        var btnTemizle = ModernUi.Button("Temizle", 190, 382, 120, 42, false);
        btnTemizle.Font = ModernUi.UiFont(9.2f, FontStyle.Bold);
        btnTemizle.Click += BtnTemizle_Click;
        loginCard.Controls.Add(btnTemizle);

        AcceptButton = btnGiris;
        CancelButton = btnTemizle;
    }

    private void BuildSecurityInfo(Control parent)
    {
        var infoPanel = ModernUi.CardPanel(36, 295, 348, 58);
        infoPanel.BackColor = Color.FromArgb(248, 250, 252);
        parent.Controls.Add(infoPanel);

        infoPanel.Controls.Add(ModernUi.Label(
            "Güvenli Giriş",
            18,
            8,
            180,
            18,
            8.2f,
            FontStyle.Bold,
            ModernUi.Dark));

        infoPanel.Controls.Add(ModernUi.Label(
            "Bilgiler doğruysa ana menü açılır, hatalıysa uyarı mesajı gösterilir.",
            18,
            29,
            305,
            18,
            7.6f,
            FontStyle.Regular,
            ModernUi.Muted));
    }

    private Panel CreateFeatureChip(string icon, string text, int x, int y)
    {
        var chip = ModernUi.CardPanel(x, y, 185, 48);
        chip.BackColor = Color.White;

        chip.Controls.Add(ModernUi.Label(
            icon,
            18,
            14,
            24,
            20,
            10f,
            FontStyle.Bold,
            ModernUi.Accent));

        chip.Controls.Add(ModernUi.Label(
            text,
            50,
            13,
            125,
            22,
            8.6f,
            FontStyle.Regular,
            ModernUi.Dark));

        return chip;
    }

    private Panel CreateInputPanel(int x, int y, int width, int height)
    {
        return new Panel
        {
            Location = new Point(x, y),
            Size = new Size(width, height),
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        };
    }

    private void PrepareTextBox(TextBox textBox, int x, int y, int width, int height, string placeholder)
    {
        textBox.Location = new Point(x, y);
        textBox.Size = new Size(width, height);
        textBox.Font = ModernUi.UiFont(9.2f, FontStyle.Regular);
        textBox.ForeColor = ModernUi.Dark;
        textBox.BackColor = Color.White;
        textBox.BorderStyle = BorderStyle.None;
        textBox.PlaceholderText = placeholder;
    }

    /// <summary>
    /// Giris butonunda kullanici bilgileri Business katmaninda kontrol edilir.
    /// </summary>
    private void BtnGiris_Click(object? sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_txtKullaniciAdi.Text) ||
                string.IsNullOrWhiteSpace(_txtSifre.Text))
            {
                WinFormsUiHelper.ShowWarning("Kullanıcı adı ve şifre boş bırakılamaz.");
                return;
            }

            Kullanici? kullanici = _kullaniciManager.LoginKontrol(
                _txtKullaniciAdi.Text.Trim(),
                _txtSifre.Text.Trim());

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

    private void BtnTemizle_Click(object? sender, EventArgs e)
    {
        _txtKullaniciAdi.Clear();
        _txtSifre.Clear();

        _txtKullaniciAdi.Focus();
    }
}
