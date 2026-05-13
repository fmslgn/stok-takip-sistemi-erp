using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using StokTakip.Business;

namespace StokTakip.WinForms;

/// <summary>
/// Temel stok rapor bilgilerinin gosterildigi formdur.
/// </summary>
public class FrmRaporlama : Form
{
    private readonly RaporManager _raporManager = new();

    private readonly Label _lblToplamUrunDeger = new();
    private readonly Label _lblKritikStokDeger = new();
    private readonly Label _lblToplamStokDeger = new();
    private readonly Label _lblDurumMesaj = new();
    private readonly Label _lblSonGuncelleme = new();
    private readonly Label _lblKritikOran = new();
    private readonly Label _lblNormalDagilim = new();
    private readonly Label _lblKritikDagilim = new();
    private readonly Label _lblToplamStokGorsel = new();
    private readonly Panel _pnlKritikOranBar = new();
    private readonly Panel _pnlNormalUrunBar = new();
    private readonly Panel _pnlKritikUrunBar = new();
    private readonly Panel _pnlToplamStokBar = new();
    private Button? _btnYenile;

    public FrmRaporlama()
    {
        InitializeComponent();
        Load += FrmRaporlama_Load;
    }

    /// <summary>
    /// Raporlama formundaki kontrolleri hazirlar.
    /// </summary>
    private void InitializeComponent()
    {
        Text = "Raporlama";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(960, 705);
        BackColor = Color.FromArgb(244, 247, 251);
        FormBorderStyle = FormBorderStyle.None;

        BuildLayout();
    }

    private void BuildLayout()
    {
        Controls.Clear();

        BuildHeader();
        BuildReportCards();
        BuildSummaryPanel();
        BuildInfoPanel();
        BuildVisualReportPanel();
    }

    private void BuildHeader()
    {
        var headerCard = CreateRoundedPanel(0, 0, 960, 86, Color.White, 22);
        Controls.Add(headerCard);

        headerCard.Controls.Add(ModernUi.Label(
            "Raporlama",
            28,
            18,
            300,
            32,
            17f,
            FontStyle.Bold,
            ModernUi.Dark));

        headerCard.Controls.Add(ModernUi.Label(
            "Stok durumunu, kritik ürünleri ve genel ürün sayısını tek ekrandan takip edin.",
            28,
            52,
            650,
            22,
            8.8f,
            FontStyle.Regular,
            ModernUi.Muted));

        var badge = CreateRoundedPanel(870, 22, 54, 24, Color.FromArgb(248, 250, 252), 12);
        headerCard.Controls.Add(badge);

        badge.Controls.Add(ModernUi.Label(
            "SYA",
            12,
            4,
            32,
            16,
            7f,
            FontStyle.Bold,
            ModernUi.Muted));
    }

    private void BuildReportCards()
    {
        Controls.Add(CreateMetricCard(
            "Toplam Ürün",
            "Sistemde kayıtlı ürün sayısı",
            _lblToplamUrunDeger,
            0,
            108,
            Color.FromArgb(238, 242, 255),
            ModernUi.Accent));

        Controls.Add(CreateMetricCard(
            "Kritik Stok",
            "Kritik seviyedeki ürün sayısı",
            _lblKritikStokDeger,
            320,
            108,
            Color.FromArgb(254, 232, 232),
            Color.FromArgb(220, 38, 38)));

        Controls.Add(CreateMetricCard(
            "Toplam Stok",
            "Genel stok miktarı",
            _lblToplamStokDeger,
            640,
            108,
            Color.FromArgb(220, 252, 231),
            Color.FromArgb(22, 163, 74)));
    }

    private Panel CreateMetricCard(
        string title,
        string description,
        Label valueLabel,
        int x,
        int y,
        Color softColor,
        Color accentColor)
    {
        var card = CreateRoundedPanel(x, y, 300, 150, Color.White, 22);

        var iconBox = CreateRoundedPanel(24, 25, 48, 48, softColor, 16);
        card.Controls.Add(iconBox);

        iconBox.Controls.Add(CreatePlainLabel(
            GetMetricIcon(title),
            0,
            0,
            48,
            48,
            15f,
            FontStyle.Bold,
            accentColor,
            ContentAlignment.MiddleCenter));

        card.Controls.Add(ModernUi.Label(
            title,
            92,
            27,
            180,
            24,
            11.2f,
            FontStyle.Bold,
            ModernUi.Dark));

        card.Controls.Add(ModernUi.Label(
            description,
            92,
            54,
            190,
            20,
            7.9f,
            FontStyle.Regular,
            ModernUi.Muted));

        var valuePanel = CreateRoundedPanel(24, 94, 252, 38, Color.FromArgb(248, 250, 252), 10);
        card.Controls.Add(valuePanel);

        ConfigureValueLabel(valueLabel, 0, 0, 252, 38, accentColor);
        valuePanel.Controls.Add(valueLabel);

        return card;
    }

    private void BuildSummaryPanel()
    {
        var summaryCard = CreateRoundedPanel(0, 286, 610, 230, Color.White, 22);
        Controls.Add(summaryCard);

        summaryCard.Controls.Add(ModernUi.Label(
            "Rapor Özeti",
            28,
            22,
            250,
            26,
            13f,
            FontStyle.Bold,
            ModernUi.Dark));

        summaryCard.Controls.Add(ModernUi.Label(
            "Bu alan, sistemdeki temel stok durumunu hızlıca yorumlamak için hazırlanmıştır.",
            28,
            52,
            520,
            22,
            8.4f,
            FontStyle.Regular,
            ModernUi.Muted));

        summaryCard.Controls.Add(CreateSummaryRow(
            "Ürün Takibi",
            "Kayıtlı ürün sayısı sistemdeki ürün çeşitliliğini gösterir.",
            28,
            94,
            ModernUi.Accent));

        summaryCard.Controls.Add(CreateSummaryRow(
            "Kritik Stok Kontrolü",
            "Kritik stoktaki ürünler öncelikli olarak kontrol edilmelidir.",
            28,
            138,
            Color.FromArgb(220, 38, 38)));

        summaryCard.Controls.Add(CreateSummaryRow(
            "Toplam Stok Durumu",
            "Depodaki genel stok miktarı operasyonel kapasiteyi gösterir.",
            28,
            182,
            Color.FromArgb(22, 163, 74)));
    }

    private Panel CreateSummaryRow(string title, string text, int x, int y, Color accent)
    {
        var row = new Panel
        {
            Location = new Point(x, y),
            Size = new Size(550, 38),
            BackColor = Color.White
        };

        var line = new Panel
        {
            Location = new Point(0, 5),
            Size = new Size(4, 28),
            BackColor = accent
        };

        row.Controls.Add(line);

        row.Controls.Add(ModernUi.Label(
            title,
            18,
            0,
            190,
            19,
            8.6f,
            FontStyle.Bold,
            ModernUi.Dark));

        row.Controls.Add(ModernUi.Label(
            text,
            18,
            19,
            500,
            18,
            7.8f,
            FontStyle.Regular,
            ModernUi.Muted));

        return row;
    }

    private void BuildInfoPanel()
    {
        var actionCard = CreateRoundedPanel(630, 286, 330, 230, Color.White, 22);
        Controls.Add(actionCard);

        actionCard.Controls.Add(ModernUi.Label(
            "İşlem Paneli",
            24,
            22,
            200,
            26,
            13f,
            FontStyle.Bold,
            ModernUi.Dark));

        actionCard.Controls.Add(ModernUi.Label(
            "Rapor değerlerini güncellemek için aşağıdaki butonu kullanabilirsiniz.",
            24,
            52,
            260,
            38,
            8.4f,
            FontStyle.Regular,
            ModernUi.Muted));

        _btnYenile = ModernUi.Button("Raporları Getir / Yenile", 24, 106, 220, 38, true);
        _btnYenile.Font = ModernUi.UiFont(8.8f, FontStyle.Bold);
        _btnYenile.Click += BtnYenile_Click;
        actionCard.Controls.Add(_btnYenile);

        // Durum karti, rapor yenileme sonucunu kullanicinin ekranda net sekilde gormesi icin kullanilir.
        var statusPanel = CreateRoundedPanel(24, 158, 282, 56, Color.FromArgb(248, 250, 252), 12);
        actionCard.Controls.Add(statusPanel);

        var statusLine = new Panel
        {
            Location = new Point(0, 9),
            Size = new Size(4, 38),
            BackColor = ModernUi.Accent
        };
        statusPanel.Controls.Add(statusLine);

        ConfigureStatusLabel(
            _lblDurumMesaj,
            "Raporlar yüklenmeye hazır.",
            16,
            8,
            250,
            20,
            ModernUi.Accent,
            FontStyle.Bold);
        statusPanel.Controls.Add(_lblDurumMesaj);

        ConfigureStatusLabel(
            _lblSonGuncelleme,
            "Son güncelleme: -",
            16,
            31,
            250,
            20,
            ModernUi.Muted,
            FontStyle.Regular);
        statusPanel.Controls.Add(_lblSonGuncelleme);
    }

    /// <summary>
    /// Standart WinForms kontrolleriyle stok durumunu anlatan gorsel rapor kartini hazirlar.
    /// </summary>
    private void BuildVisualReportPanel()
    {
        var visualCard = CreateRoundedPanel(0, 540, 960, 160, Color.White, 22);
        Controls.Add(visualCard);

        visualCard.Controls.Add(ModernUi.Label(
            "Görsel Rapor",
            28,
            15,
            220,
            26,
            13f,
            FontStyle.Bold,
            ModernUi.Dark));

        visualCard.Controls.Add(ModernUi.Label(
            "Stok durumunu ve kritik stok oranını görsel olarak takip edebilirsiniz.",
            28,
            43,
            560,
            20,
            8.4f,
            FontStyle.Regular,
            ModernUi.Muted));

        visualCard.Controls.Add(ModernUi.Label(
            "Kritik Stok Oranı",
            28,
            72,
            170,
            20,
            8.7f,
            FontStyle.Bold,
            ModernUi.Dark));
        ConfigureVisualLabel(_lblKritikOran, 200, 72, 105, 20, Color.FromArgb(220, 38, 38), FontStyle.Bold);
        visualCard.Controls.Add(_lblKritikOran);

        var kritikOranTrack = CreateBarTrack(28, 102, 280, 16);
        ConfigureBarFill(_pnlKritikOranBar, Color.FromArgb(220, 38, 38));
        kritikOranTrack.Controls.Add(_pnlKritikOranBar);
        visualCard.Controls.Add(kritikOranTrack);

        visualCard.Controls.Add(ModernUi.Label(
            "Normal Ürün / Kritik Ürün Dağılımı",
            345,
            72,
            260,
            20,
            8.7f,
            FontStyle.Bold,
            ModernUi.Dark));

        ConfigureVisualLabel(_lblNormalDagilim, 345, 95, 260, 18, Color.FromArgb(22, 163, 74), FontStyle.Regular);
        visualCard.Controls.Add(_lblNormalDagilim);
        var normalTrack = CreateBarTrack(345, 116, 255, 8);
        ConfigureBarFill(_pnlNormalUrunBar, Color.FromArgb(22, 163, 74));
        normalTrack.Controls.Add(_pnlNormalUrunBar);
        visualCard.Controls.Add(normalTrack);

        ConfigureVisualLabel(_lblKritikDagilim, 345, 128, 260, 18, Color.FromArgb(220, 38, 38), FontStyle.Regular);
        visualCard.Controls.Add(_lblKritikDagilim);
        var kritikTrack = CreateBarTrack(345, 148, 255, 8);
        ConfigureBarFill(_pnlKritikUrunBar, Color.FromArgb(220, 38, 38));
        kritikTrack.Controls.Add(_pnlKritikUrunBar);
        visualCard.Controls.Add(kritikTrack);

        visualCard.Controls.Add(ModernUi.Label(
            "Toplam Stok Görseli",
            660,
            72,
            240,
            20,
            8.7f,
            FontStyle.Bold,
            ModernUi.Dark));
        ConfigureVisualLabel(_lblToplamStokGorsel, 660, 96, 250, 28, ModernUi.Accent, FontStyle.Bold);
        _lblToplamStokGorsel.Font = ModernUi.UiFont(12.5f, FontStyle.Bold);
        visualCard.Controls.Add(_lblToplamStokGorsel);

        var toplamStokTrack = CreateBarTrack(660, 132, 240, 14);
        ConfigureBarFill(_pnlToplamStokBar, ModernUi.Accent);
        toplamStokTrack.Controls.Add(_pnlToplamStokBar);
        visualCard.Controls.Add(toplamStokTrack);
    }

    /// <summary>
    /// Form acilisinda temel rapor bilgileri yuklenir.
    /// </summary>
    private void FrmRaporlama_Load(object? sender, EventArgs e)
    {
        RaporlariYukle();
    }

    /// <summary>
    /// Yenile butonu rapor degerlerini tekrar hesaplar ve ekranda durum bilgisini gunceller.
    /// </summary>
    private void BtnYenile_Click(object? sender, EventArgs e)
    {
        // Kullanici yenilemeye bastiginda buton gecici olarak pasiflestirilir ve durum alani guncellenir.
        if (_btnYenile is null)
        {
            RaporlariYukle();
            return;
        }

        var eskiMetin = _btnYenile.Text;

        try
        {
            _btnYenile.Enabled = false;
            _btnYenile.Text = "Yenileniyor...";
            _btnYenile.Refresh();

            RaporlariYukle();
        }
        finally
        {
            _btnYenile.Text = eskiMetin;
            _btnYenile.Enabled = true;
        }
    }

    /// <summary>
    /// Rapor kartlarindaki toplam urun, kritik stok ve toplam stok degerlerini Business katmanindan yukler ve durum bilgisini ekrana yazar.
    /// </summary>
    private bool RaporlariYukle()
    {
        try
        {
            var toplamUrun = _raporManager.ToplamUrunSayisiGetir();
            var kritikStok = _raporManager.KritikStokUrunSayisiGetir();
            var toplamStok = _raporManager.ToplamStokMiktariGetir();

            _lblToplamUrunDeger.Text = toplamUrun.ToString();
            _lblKritikStokDeger.Text = kritikStok.ToString();
            _lblToplamStokDeger.Text = toplamStok.ToString();
            GorselRaporlariGuncelle(toplamUrun, kritikStok, toplamStok);

            RaporDurumunuGoster("Raporlar başarıyla güncellendi.", false);
            return true;
        }
        catch
        {
            _lblToplamUrunDeger.Text = "0";
            _lblKritikStokDeger.Text = "0";
            _lblToplamStokDeger.Text = "0";
            GorselRaporlariGuncelle(0, 0, 0);
            RaporDurumunuGoster("Raporlar güncellenemedi.", true);
            WinFormsUiHelper.ShowError("Rapor bilgileri alınırken bir hata oluştu. Lütfen veritabanı bağlantısını kontrol ediniz.");
            return false;
        }
    }

    /// <summary>
    /// Sayisal rapor degerlerinden kritik oran, dagilim ve toplam stok gorsellerini gunceller.
    /// </summary>
    private void GorselRaporlariGuncelle(int toplamUrun, int kritikStok, int toplamStok)
    {
        var normalUrun = Math.Max(0, toplamUrun - kritikStok);

        // Toplam urun 0 ise bolme yapmadan oranlari 0 gostererek formun hata vermesi engellenir.
        var kritikOrani = toplamUrun == 0 ? 0 : (double)kritikStok / toplamUrun;
        var normalOrani = toplamUrun == 0 ? 0 : (double)normalUrun / toplamUrun;

        _lblKritikOran.Text = $"%{kritikOrani * 100:0.0}";
        _lblNormalDagilim.Text = $"Normal Ürün: {normalUrun} (%{normalOrani * 100:0.0})";
        _lblKritikDagilim.Text = $"Kritik Ürün: {kritikStok} (%{kritikOrani * 100:0.0})";
        _lblToplamStokGorsel.Text = $"Toplam Stok Miktarı: {toplamStok}";

        BarGuncelle(_pnlKritikOranBar, kritikOrani);
        BarGuncelle(_pnlNormalUrunBar, normalOrani);
        BarGuncelle(_pnlKritikUrunBar, kritikOrani);

        var toplamStokOrani = toplamStok <= 0 ? 0 : Math.Min(1d, toplamStok / 1000d);
        BarGuncelle(_pnlToplamStokBar, toplamStokOrani);
    }

    /// <summary>
    /// Verilen orana gore panel genisligini ayarlayarak basit bar gorseli olusturur.
    /// </summary>
    private static void BarGuncelle(Panel barPanel, double oran)
    {
        if (barPanel.Parent is null)
        {
            return;
        }

        var temizOran = Math.Max(0, Math.Min(1, oran));
        barPanel.Width = (int)Math.Round(barPanel.Parent.ClientSize.Width * temizOran);
        barPanel.Height = barPanel.Parent.ClientSize.Height;
        barPanel.Visible = temizOran > 0;
    }

    private void RaporDurumunuGoster(string mesaj, bool hata)
    {
        _lblDurumMesaj.Text = mesaj;
        _lblDurumMesaj.ForeColor = hata ? Color.FromArgb(185, 28, 28) : ModernUi.Accent;
        _lblSonGuncelleme.Text = $"Son güncelleme: {DateTime.Now:dd.MM.yyyy HH:mm}";
        _lblDurumMesaj.Visible = true;
        _lblSonGuncelleme.Visible = true;
        _lblDurumMesaj.BringToFront();
        _lblSonGuncelleme.BringToFront();
    }

    private static void ConfigureValueLabel(Label label, int x, int y, int width, int height, Color color)
    {
        label.Location = new Point(x, y);
        label.Size = new Size(width, height);
        label.Font = ModernUi.UiFont(15.5f, FontStyle.Bold);
        label.ForeColor = color;
        label.BackColor = Color.Transparent;
        label.TextAlign = ContentAlignment.MiddleCenter;
        label.AutoSize = false;
    }

    private static void ConfigureStatusLabel(
        Label label,
        string text,
        int x,
        int y,
        int width,
        int height,
        Color color,
        FontStyle style)
    {
        label.Location = new Point(x, y);
        label.Size = new Size(width, height);
        label.Text = text;
        label.Font = ModernUi.UiFont(8.2f, style);
        label.ForeColor = color;
        label.BackColor = Color.Transparent;
        label.AutoSize = false;
        label.TextAlign = ContentAlignment.MiddleLeft;
        label.Visible = true;
    }

    private static void ConfigureVisualLabel(
        Label label,
        int x,
        int y,
        int width,
        int height,
        Color color,
        FontStyle style)
    {
        label.Location = new Point(x, y);
        label.Size = new Size(width, height);
        label.Font = ModernUi.UiFont(8f, style);
        label.ForeColor = color;
        label.BackColor = Color.Transparent;
        label.TextAlign = ContentAlignment.MiddleLeft;
        label.AutoSize = false;
    }

    private Panel CreateBarTrack(int x, int y, int width, int height)
    {
        return CreateRoundedPanel(x, y, width, height, Color.FromArgb(229, 231, 235), Math.Max(1, height / 2));
    }

    private static void ConfigureBarFill(Panel barPanel, Color color)
    {
        barPanel.Location = new Point(0, 0);
        barPanel.Size = new Size(0, 1);
        barPanel.BackColor = color;
    }

    private string GetMetricIcon(string title)
    {
        return title switch
        {
            "Toplam Ürün" => "▦",
            "Kritik Stok" => "△",
            "Toplam Stok" => "▤",
            _ => "●"
        };
    }

    private Label CreatePlainLabel(
        string text,
        int x,
        int y,
        int width,
        int height,
        float fontSize,
        FontStyle style,
        Color color,
        ContentAlignment align)
    {
        return new Label
        {
            Text = text,
            Location = new Point(x, y),
            Size = new Size(width, height),
            Font = ModernUi.UiFont(fontSize, style),
            ForeColor = color,
            BackColor = Color.Transparent,
            TextAlign = align,
            AutoSize = false
        };
    }

    private Panel CreateRoundedPanel(int x, int y, int width, int height, Color backColor, int radius)
    {
        var panel = new Panel
        {
            Location = new Point(x, y),
            Size = new Size(width, height),
            BackColor = backColor
        };

        panel.Resize += (_, _) => ApplyRoundedRegion(panel, radius);
        ApplyRoundedRegion(panel, radius);

        return panel;
    }

    private void ApplyRoundedRegion(Control control, int radius)
    {
        if (control.Width <= 0 || control.Height <= 0)
            return;

        using var path = GetRoundedPath(new Rectangle(0, 0, control.Width, control.Height), radius);
        control.Region = new Region(path);
    }

    private GraphicsPath GetRoundedPath(Rectangle rect, int radius)
    {
        int d = radius * 2;
        var path = new GraphicsPath();

        path.AddArc(rect.X, rect.Y, d, d, 180, 90);
        path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
        path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
        path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
        path.CloseFigure();

        return path;
    }
}
