using System.Drawing;
using StokTakip.Business;

namespace StokTakip.WinForms;

/// <summary>
/// Kritik stok seviyesine dusen urunlerin listelendigi formdur.
/// </summary>
public class FrmKritikStok : Form
{
    private readonly UrunManager _urunManager = new();
    private readonly DataGridView _grid = new();

    public FrmKritikStok()
    {
        InitializeComponent();
        Load += FrmKritikStok_Load;
    }

    /// <summary>
    /// Kritik stok formundaki kontrolleri hazirlar.
    /// </summary>
    private void InitializeComponent()
    {
        WinFormsUiHelper.ApplyFormStyle(this, "Kritik Stok");
        ClientSize = new Size(900, 500);

        var lblBaslik = new Label { Text = "Kritik Stoktaki Ürünler", Location = new Point(24, 24), AutoSize = true, Font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Bold) };
        var btnListele = new Button { Text = "Listele / Yenile", Location = new Point(24, 62), Size = new Size(130, 32) };
        _grid.Location = new Point(24, 112);
        _grid.Size = new Size(850, 340);
        WinFormsUiHelper.ConfigureGrid(_grid);

        btnListele.Click += BtnListele_Click;
        WinFormsUiHelper.StyleSecondaryButton(btnListele);
        Controls.AddRange(new Control[] { lblBaslik, btnListele, _grid });
        WinFormsUiHelper.StyleInputs(this);
        WinFormsUiHelper.AddHeader(this, "Kritik Stok", "Kritik seviyeye düşen ürünleri hızlıca görün ve önlem alın.");
    }

    /// <summary>
    /// Form acilisinda kritik stok listesi yuklenir.
    /// </summary>
    private void FrmKritikStok_Load(object? sender, EventArgs e)
    {
        Listele();
    }

    /// <summary>
    /// Listele butonu kritik stok listesini yeniler.
    /// </summary>
    private void BtnListele_Click(object? sender, EventArgs e)
    {
        Listele();
    }

    private void Listele()
    {
        try
        {
            var liste = _urunManager.GetKritikStoktakiler();
            _grid.DataSource = null;
            _grid.DataSource = liste;

            // Kritik stok satirlarini uyarı rengiyle daha gorunur hale getirir.
            foreach (DataGridViewRow row in _grid.Rows)
            {
                row.DefaultCellStyle.BackColor = Color.FromArgb(255, 247, 237);
                row.DefaultCellStyle.ForeColor = Color.FromArgb(124, 45, 18);
            }

            if (liste.Count == 0)
            {
                WinFormsUiHelper.ShowInfo("Kritik stok seviyesinde ürün bulunmamaktadır.");
            }
        }
        catch (Exception ex)
        {
            WinFormsUiHelper.ShowError(ex.Message);
        }
    }
}
