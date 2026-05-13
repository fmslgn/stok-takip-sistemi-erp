using System.Drawing;
using System.Windows.Forms;
using StokTakip.Business;

namespace StokTakip.WinForms;

/// <summary>
/// Urun saklama kosullari hakkinda soru-cevap seklinde kural tabanli destek veren chat formudur.
/// </summary>
public class FrmSaklamaAsistaniChat : Form
{
    private readonly SaklamaKosuluManager _saklamaKosuluManager = new();
    private readonly string _urunAdi;
    private readonly string _kategoriAdi;
    private readonly string _barkod;
    private readonly string _saklamaKosuluAdi;

    private readonly RichTextBox _txtChatGecmisi = new();
    private readonly TextBox _txtKullaniciMesaji = new();
    private Button _btnGonder = null!;
    private Button _btnOneriAl = null!;

    private string _mevcutOneri = string.Empty;

    public FrmSaklamaAsistaniChat(string urunAdi, string kategoriAdi, string barkod, string saklamaKosuluAdi)
    {
        _urunAdi = (urunAdi ?? string.Empty).Trim();
        _kategoriAdi = (kategoriAdi ?? string.Empty).Trim();
        _barkod = (barkod ?? string.Empty).Trim();
        _saklamaKosuluAdi = (saklamaKosuluAdi ?? string.Empty).Trim();

        InitializeComponent();
        BaslangicMesajiYaz();
    }

    /// <summary>
    /// Saklama asistani chat ekranindaki modern ve sade kontrolleri hazirlar.
    /// </summary>
    private void InitializeComponent()
    {
        ModernUi.ConfigureForm(this, "Saklama Asistanı", 700, 560);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Color.FromArgb(244, 247, 251);

        BuildHeaderCard();
        BuildChatCard();
        BuildInputCard();
    }

    private void BuildHeaderCard()
    {
        var headerCard = ModernUi.CardPanel(20, 20, 660, 86);
        headerCard.BackColor = Color.White;
        Controls.Add(headerCard);

        headerCard.Controls.Add(ModernUi.Label(
            "Saklama Asistanı",
            24,
            16,
            320,
            30,
            15f,
            FontStyle.Bold,
            ModernUi.Dark));

        headerCard.Controls.Add(ModernUi.Label(
            "Ürün saklama koşulları hakkında soru sorabilirsiniz.",
            24,
            48,
            560,
            22,
            8.4f,
            FontStyle.Regular,
            ModernUi.Muted));
    }

    private void BuildChatCard()
    {
        var chatCard = ModernUi.CardPanel(20, 122, 660, 306);
        chatCard.BackColor = Color.White;
        Controls.Add(chatCard);

        _txtChatGecmisi.Location = new Point(22, 20);
        _txtChatGecmisi.Size = new Size(616, 264);
        _txtChatGecmisi.ReadOnly = true;
        _txtChatGecmisi.BorderStyle = BorderStyle.None;
        _txtChatGecmisi.BackColor = Color.White;
        _txtChatGecmisi.ForeColor = ModernUi.Dark;
        _txtChatGecmisi.Font = ModernUi.UiFont(8.8f);
        _txtChatGecmisi.ScrollBars = RichTextBoxScrollBars.Vertical;
        chatCard.Controls.Add(_txtChatGecmisi);
    }

    private void BuildInputCard()
    {
        var inputCard = ModernUi.CardPanel(20, 444, 660, 92);
        inputCard.BackColor = Color.White;
        Controls.Add(inputCard);

        _txtKullaniciMesaji.Location = new Point(22, 18);
        _txtKullaniciMesaji.Size = new Size(430, 32);
        _txtKullaniciMesaji.Font = ModernUi.UiFont(8.8f);
        _txtKullaniciMesaji.PlaceholderText = "Örneğin: Daha kısa anlat, sıcaklık bilgisi ver...";
        _txtKullaniciMesaji.BorderStyle = BorderStyle.FixedSingle;
        _txtKullaniciMesaji.KeyDown += TxtKullaniciMesaji_KeyDown;
        inputCard.Controls.Add(_txtKullaniciMesaji);

        _btnGonder = ModernUi.Button("Gönder", 468, 16, 150, 34, true);
        _btnGonder.Font = ModernUi.UiFont(8.4f, FontStyle.Bold);
        _btnGonder.Click += BtnGonder_Click;
        inputCard.Controls.Add(_btnGonder);

        _btnOneriAl = ModernUi.Button("Öneri Al", 22, 56, 120, 28, true);
        _btnOneriAl.Font = ModernUi.UiFont(8f, FontStyle.Bold);
        _btnOneriAl.Click += BtnOneriAl_Click;
        inputCard.Controls.Add(_btnOneriAl);

        var btnTemizle = ModernUi.Button("Temizle", 154, 56, 110, 28);
        btnTemizle.Font = ModernUi.UiFont(8f, FontStyle.Bold);
        btnTemizle.Click += BtnTemizle_Click;
        inputCard.Controls.Add(btnTemizle);

        var btnKapat = ModernUi.Button("Kapat", 508, 56, 110, 28);
        btnKapat.Font = ModernUi.UiFont(8f, FontStyle.Bold);
        btnKapat.Click += (_, _) => Close();
        inputCard.Controls.Add(btnKapat);

        AcceptButton = _btnGonder;
    }

    private void BaslangicMesajiYaz()
    {
        if (!string.IsNullOrWhiteSpace(_urunAdi) && !string.IsNullOrWhiteSpace(_kategoriAdi))
        {
            string kosulBilgisi = string.IsNullOrWhiteSpace(_saklamaKosuluAdi)
                ? string.Empty
                : $", Seçili saklama koşulu: {_saklamaKosuluAdi}";

            ChatMesajiEkle("Asistan", $"Merhaba, seçili ürün için saklama koşulu önerisi alabilirim. Ürün: {_urunAdi}, Kategori: {_kategoriAdi}{kosulBilgisi}", ModernUi.Accent);
            return;
        }

        ChatMesajiEkle("Asistan", "Merhaba, ürün adı girerek saklama önerisi alabilirsiniz.", ModernUi.Accent);
    }

    /// <summary>
    /// Öneri Al butonu ürün bilgilerini Business katmanına gönderip sonucu chat geçmişine ekler.
    /// </summary>
    private async void BtnOneriAl_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_urunAdi))
        {
            WinFormsUiHelper.ShowWarning("Ürün adı boş bırakılamaz.");
            return;
        }

        if (string.IsNullOrWhiteSpace(_kategoriAdi))
        {
            WinFormsUiHelper.ShowWarning("Kategori seçilmelidir.");
            return;
        }

        string eskiMetin = _btnOneriAl.Text;

        try
        {
            _btnOneriAl.Enabled = false;
            _btnOneriAl.Text = "Alınıyor...";
            _btnOneriAl.Refresh();

            _mevcutOneri = await _saklamaKosuluManager.SaklamaKosuluOnerWebDestekliAsync(_urunAdi, _kategoriAdi, _barkod);
            ChatMesajiEkle("Asistan", _mevcutOneri, ModernUi.Accent);
        }
        catch
        {
            _mevcutOneri = _saklamaKosuluManager.SaklamaKosuluOner(_urunAdi, _kategoriAdi);
            ChatMesajiEkle("Asistan", $"Şu anda detaylı yanıt üretilemedi, kural tabanlı öneri üzerinden devam ediyorum. {_mevcutOneri}", ModernUi.Accent);
        }
        finally
        {
            _btnOneriAl.Text = eskiMetin;
            _btnOneriAl.Enabled = true;
        }
    }

    /// <summary>
    /// Gönder butonu kullanici mesajini chat gecmisine ekler ve Business katmanindan chatbot cevabi alir.
    /// </summary>
    private void BtnGonder_Click(object? sender, EventArgs e)
    {
        string mesaj = _txtKullaniciMesaji.Text.Trim();
        if (string.IsNullOrWhiteSpace(mesaj))
        {
            WinFormsUiHelper.ShowWarning("Mesaj boş bırakılamaz.");
            return;
        }

        ChatMesajiEkle("Kullanıcı", mesaj, ModernUi.Text);
        _txtKullaniciMesaji.Clear();

        try
        {
            string yanit = _saklamaKosuluManager.SaklamaChatbotYanitiUret(_urunAdi, _kategoriAdi, _barkod, _mevcutOneri, mesaj);
            if (!string.IsNullOrWhiteSpace(yanit))
            {
                _mevcutOneri = yanit;
            }

            ChatMesajiEkle("Asistan", yanit, ModernUi.Accent);
        }
        catch
        {
            string yedekOneri = _saklamaKosuluManager.SaklamaKosuluOner(_urunAdi, _kategoriAdi);
            ChatMesajiEkle("Asistan", $"Şu anda detaylı yanıt üretilemedi, kural tabanlı öneri üzerinden devam ediyorum. {yedekOneri}", ModernUi.Accent);
        }
    }

    private void BtnTemizle_Click(object? sender, EventArgs e)
    {
        _txtChatGecmisi.Clear();
        _txtKullaniciMesaji.Clear();
        _mevcutOneri = string.Empty;
        BaslangicMesajiYaz();
    }

    private void TxtKullaniciMesaji_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Enter)
        {
            return;
        }

        e.SuppressKeyPress = true;
        BtnGonder_Click(sender, EventArgs.Empty);
    }

    private void ChatMesajiEkle(string etiket, string mesaj, Color renk)
    {
        _txtChatGecmisi.SelectionStart = _txtChatGecmisi.TextLength;
        _txtChatGecmisi.SelectionLength = 0;
        _txtChatGecmisi.SelectionColor = renk;
        _txtChatGecmisi.SelectionFont = ModernUi.UiFont(8.8f, FontStyle.Bold);
        _txtChatGecmisi.AppendText($"{etiket}: ");
        _txtChatGecmisi.SelectionColor = ModernUi.Dark;
        _txtChatGecmisi.SelectionFont = ModernUi.UiFont(8.8f);
        _txtChatGecmisi.AppendText($"{mesaj}{Environment.NewLine}{Environment.NewLine}");
        _txtChatGecmisi.SelectionColor = _txtChatGecmisi.ForeColor;
        _txtChatGecmisi.ScrollToCaret();
    }
}
