using System.Drawing;
using StokTakip.Business;

namespace StokTakip.WinForms;

/// <summary>
/// Kural tabanli saklama kosulu onerilerinin gosterildigi formdur.
/// </summary>
public class FrmSaklamaKosuluAI : Form
{
    private readonly SaklamaKosuluManager _saklamaKosuluManager = new();
    private readonly TextBox _txtUrunAdi = new();
    private readonly TextBox _txtKategoriAdi = new();
    private readonly RichTextBox _txtOneri = new();

    public FrmSaklamaKosuluAI()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Saklama onerisi ekranindaki kontrolleri hazirlar.
    /// </summary>
    private void InitializeComponent()
    {
        Text = "Saklama Kosulu Oneri";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(680, 430);

        var lblUrunAdi = new Label { Text = "Urun Adi", Location = new Point(24, 28), AutoSize = true };
        var lblKategoriAdi = new Label { Text = "Kategori Adi", Location = new Point(24, 68), AutoSize = true };
        var lblOneri = new Label { Text = "Oneri Sonucu", Location = new Point(24, 150), AutoSize = true };
        var btnOneri = new Button { Text = "Oneri Al", Location = new Point(140, 104), Size = new Size(100, 32) };
        var btnTemizle = new Button { Text = "Temizle", Location = new Point(250, 104), Size = new Size(90, 32) };

        _txtUrunAdi.Location = new Point(140, 24);
        _txtUrunAdi.Size = new Size(260, 27);
        _txtKategoriAdi.Location = new Point(140, 64);
        _txtKategoriAdi.Size = new Size(260, 27);
        _txtOneri.Location = new Point(140, 150);
        _txtOneri.Size = new Size(490, 220);
        _txtOneri.ReadOnly = true;

        btnOneri.Click += BtnOneri_Click;
        btnTemizle.Click += BtnTemizle_Click;

        Controls.AddRange(new Control[] { lblUrunAdi, _txtUrunAdi, lblKategoriAdi, _txtKategoriAdi, btnOneri, btnTemizle, lblOneri, _txtOneri });
    }

    /// <summary>
    /// Oneri al butonu kural tabanli Business metodunu cagirir.
    /// </summary>
    private void BtnOneri_Click(object? sender, EventArgs e)
    {
        try
        {
            _txtOneri.Text = _saklamaKosuluManager.SaklamaKosuluOner(_txtUrunAdi.Text, _txtKategoriAdi.Text);
        }
        catch (Exception ex)
        {
            WinFormsUiHelper.ShowError(ex.Message);
        }
    }

    /// <summary>
    /// Temizle butonu form alanlarini sifirlar.
    /// </summary>
    private void BtnTemizle_Click(object? sender, EventArgs e)
    {
        _txtUrunAdi.Clear();
        _txtKategoriAdi.Clear();
        _txtOneri.Clear();
    }
}
