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

    private Label _lblDurumBaslik = null!;
    private Label _lblDurumMesaj = null!;

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

        var btnYenile = ModernUi.Button("Raporları Getir / Yenile", 24, 110, 220, 38, true);
        btnYenile.Font = ModernUi.UiFont(8.8f, FontStyle.Bold);
        btnYenile.Click += BtnYenile_Click;
        actionCard.Controls.Add(btnYenile);

        var statusBox = CreateRoundedPanel(24, 166, 280, 48, Color.FromArgb(238, 242, 255), 14);
        actionCard.Controls.Add(statusBox);

        _lblDurumBaslik = ModernUi.Label(
            "Bilgi",
            16,
            8,
            220,
            16,
            8f,
            FontStyle.Bold,
            ModernUi.Accent);

        _lblDurumMesaj = ModernUi.Label(
            "Raporlar yüklenmeye hazır.",
            16,
            25,
            240,
            16,
            7.5f,
            FontStyle.Regular,
            ModernUi.Muted);

        statusBox.Controls.Add(_lblDurumBaslik);
        statusBox.Controls.Add(_lblDurumMesaj);
    }

    /// <summary>
    /// Form acilisinda temel rapor bilgileri yuklenir.
    /// </summary>
    private void FrmRaporlama_Load(object? sender, EventArgs e)
    {
        RaporlariYukle();
    }

    /// <summary>
    /// Yenile butonu rapor degerlerini tekrar hesaplar.
    /// </summary>
    private void BtnYenile_Click(object? sender, EventArgs e)
    {
        RaporlariYukle();
    }

    private void RaporlariYukle()
    {
        try
        {
            _lblToplamUrunDeger.Text = _raporManager.ToplamUrunSayisiGetir().ToString();
            _lblKritikStokDeger.Text = _raporManager.KritikStokUrunSayisiGetir().ToString();
            _lblToplamStokDeger.Text = _raporManager.ToplamStokMiktariGetir().ToString();

            ShowStatus(
                "Başarılı",
                "Rapor değerleri güncellendi.",
                false);
        }
        catch (Exception ex)
        {
            ShowStatus(
                "Hata",
                ex.Message,
                true);
        }
    }

    private void ShowStatus(string title, string message, bool error)
    {
        _lblDurumBaslik.Text = title;
        _lblDurumMesaj.Text = message;

        _lblDurumBaslik.ForeColor = error
            ? Color.FromArgb(185, 28, 28)
            : ModernUi.Accent;

        _lblDurumMesaj.ForeColor = error
            ? Color.FromArgb(185, 28, 28)
            : ModernUi.Muted;
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