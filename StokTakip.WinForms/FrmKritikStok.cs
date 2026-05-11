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
        Text = "Kritik Stok";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(900, 500);

        var lblBaslik = new Label { Text = "Kritik Stoktaki Urunler", Location = new Point(24, 24), AutoSize = true, Font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Bold) };
        var btnListele = new Button { Text = "Listele / Yenile", Location = new Point(24, 62), Size = new Size(130, 32) };
        _grid.Location = new Point(24, 112);
        _grid.Size = new Size(850, 340);
        WinFormsUiHelper.ConfigureGrid(_grid);

        btnListele.Click += BtnListele_Click;
        Controls.AddRange(new Control[] { lblBaslik, btnListele, _grid });
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

            if (liste.Count == 0)
            {
                WinFormsUiHelper.ShowInfo("Kritik stok seviyesinde urun bulunmamaktadir.");
            }
        }
        catch (Exception ex)
        {
            WinFormsUiHelper.ShowError(ex.Message);
        }
    }
}
