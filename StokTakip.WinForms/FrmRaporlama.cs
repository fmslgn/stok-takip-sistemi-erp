using System.Drawing;
using StokTakip.Business;

namespace StokTakip.WinForms;

/// <summary>
/// Temel stok rapor bilgilerinin gosterildigi formdur.
/// </summary>
public class FrmRaporlama : Form
{
    private readonly RaporManager _raporManager = new();
    private readonly TextBox _txtToplamUrun = new();
    private readonly TextBox _txtKritikStok = new();
    private readonly TextBox _txtToplamStok = new();

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
        WinFormsUiHelper.ApplyFormStyle(this, "Raporlama");
        ClientSize = new Size(760, 360);

        var toplamUrunCard = CreateReportCard("Toplam Ürün Sayısı", _txtToplamUrun, 24, 34, WinFormsUiHelper.Blue);
        var kritikCard = CreateReportCard("Kritik Stoktaki Ürün Sayısı", _txtKritikStok, 270, 34, WinFormsUiHelper.Warning);
        var toplamStokCard = CreateReportCard("Toplam Stok Miktarı", _txtToplamStok, 516, 34, WinFormsUiHelper.Success);
        var btnYenile = new Button { Text = "Raporları Getir / Yenile", Location = new Point(24, 190), Size = new Size(190, 36) };

        btnYenile.Click += BtnYenile_Click;
        WinFormsUiHelper.StylePrimaryButton(btnYenile);

        Controls.AddRange(new Control[] { toplamUrunCard, kritikCard, toplamStokCard, btnYenile });
        WinFormsUiHelper.AddHeader(this, "Raporlama", "Stok durumunu özet kartlarla hızlıca inceleyin.");
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
            _txtToplamUrun.Text = _raporManager.ToplamUrunSayisiGetir().ToString();
            _txtKritikStok.Text = _raporManager.KritikStokUrunSayisiGetir().ToString();
            _txtToplamStok.Text = _raporManager.ToplamStokMiktariGetir().ToString();
        }
        catch (Exception ex)
        {
            WinFormsUiHelper.ShowError(ex.Message);
        }
    }

    private static Panel CreateReportCard(string title, TextBox valueBox, int x, int y, Color accentColor)
    {
        var panel = new Panel
        {
            Location = new Point(x, y),
            Size = new Size(220, 120),
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        };

        var accent = new Panel { Dock = DockStyle.Top, Height = 6, BackColor = accentColor };
        var label = new Label
        {
            Text = title,
            Location = new Point(16, 24),
            AutoSize = true,
            ForeColor = Color.FromArgb(71, 85, 105),
            Font = new Font("Segoe UI", 9F, FontStyle.Bold)
        };

        valueBox.Location = new Point(16, 58);
        valueBox.Size = new Size(180, 34);
        valueBox.ReadOnly = true;
        valueBox.BorderStyle = BorderStyle.None;
        valueBox.BackColor = Color.White;
        valueBox.ForeColor = accentColor;
        valueBox.Font = new Font("Segoe UI", 18F, FontStyle.Bold);

        panel.Controls.AddRange(new Control[] { accent, label, valueBox });
        return panel;
    }
}
