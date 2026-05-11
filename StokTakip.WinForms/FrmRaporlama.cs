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
        Text = "Raporlama";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(520, 300);

        var lblBaslik = new Label { Text = "Temel Stok Raporları", Location = new Point(24, 24), AutoSize = true, Font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Bold) };
        var lblToplamUrun = new Label { Text = "Toplam Ürün Sayısı", Location = new Point(24, 82), AutoSize = true };
        var lblKritikStok = new Label { Text = "Kritik Stoktaki Ürün Sayısı", Location = new Point(24, 122), AutoSize = true };
        var lblToplamStok = new Label { Text = "Toplam Stok Miktarı", Location = new Point(24, 162), AutoSize = true };
        var btnYenile = new Button { Text = "Raporları Getir / Yenile", Location = new Point(210, 210), Size = new Size(170, 32) };

        ConfigureReadOnlyTextBox(_txtToplamUrun, 240, 78);
        ConfigureReadOnlyTextBox(_txtKritikStok, 240, 118);
        ConfigureReadOnlyTextBox(_txtToplamStok, 240, 158);
        btnYenile.Click += BtnYenile_Click;

        Controls.AddRange(new Control[] { lblBaslik, lblToplamUrun, _txtToplamUrun, lblKritikStok, _txtKritikStok, lblToplamStok, _txtToplamStok, btnYenile });
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

    private static void ConfigureReadOnlyTextBox(TextBox textBox, int x, int y)
    {
        textBox.Location = new Point(x, y);
        textBox.Size = new Size(140, 27);
        textBox.ReadOnly = true;
    }
}
