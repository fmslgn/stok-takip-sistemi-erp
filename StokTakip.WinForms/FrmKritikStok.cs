using System;
using System.Drawing;
using System.Windows.Forms;
using StokTakip.Business;

namespace StokTakip.WinForms;

/// <summary>
/// Kritik stok seviyesine düşen ürünlerin listelendiği formdur.
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
    /// Kritik stok formundaki kontrolleri hazırlar.
    /// </summary>
    private void InitializeComponent()
    {
        ModernUi.ConfigureForm(this, "Kritik Stok", 1120, 720);

        FormBorderStyle = FormBorderStyle.None;
        MaximizeBox = false;
        BackColor = Color.FromArgb(244, 247, 251);

        var headerCard = ModernUi.CardPanel(24, 20, 1072, 78);
        headerCard.BackColor = Color.White;
        Controls.Add(headerCard);

        headerCard.Controls.Add(ModernUi.Label(
            "Kritik Stok",
            24,
            16,
            400,
            33,
            16f,
            FontStyle.Bold,
            ModernUi.Dark));

        headerCard.Controls.Add(ModernUi.Label(
            "Kritik stok seviyesine düşen ürünlerin listelenmesi ve takip edilmesi.",
            24,
            55,
            760,
            22,
            8.8f,
            FontStyle.Regular,
            ModernUi.Muted));

        var badge = ModernUi.CardPanel(1000, 18, 45, 25);
        badge.BackColor = Color.FromArgb(248, 250, 252);
        badge.Controls.Add(ModernUi.Label(
            "SYA",
            11,
            5,
            30,
            16,
            7f,
            FontStyle.Bold,
            ModernUi.Muted));

        headerCard.Controls.Add(badge);

        var actionCard = ModernUi.CardPanel(24, 120, 1072, 130);
        actionCard.BackColor = Color.White;
        Controls.Add(actionCard);

        actionCard.Controls.Add(ModernUi.Label(
            "İşlem Paneli",
            22,
            18,
            220,
            24,
            12f,
            FontStyle.Bold,
            ModernUi.Dark));

        actionCard.Controls.Add(ModernUi.Label(
            "Listeyi yenileyerek güncel kritik stok durumunu kontrol edebilirsiniz.",
            22,
            50,
            720,
            36,
            8.5f,
            FontStyle.Regular,
            ModernUi.Muted));

        var btnListele = ModernUi.Button("Listele / Yenile", 880, 48, 150, 36, true);
        btnListele.Click += BtnListele_Click;
        actionCard.Controls.Add(btnListele);

        var tableCard = ModernUi.CardPanel(24, 270, 1072, 425);
        tableCard.BackColor = Color.White;
        Controls.Add(tableCard);

        tableCard.Controls.Add(ModernUi.Label(
            "Kritik Stok Listesi",
            22,
            18,
            300,
            24,
            12f,
            FontStyle.Bold,
            ModernUi.Dark));

        tableCard.Controls.Add(ModernUi.Label(
            "Azalan ürünleri buradan takip ederek stok giriş işlemi yapabilirsiniz.",
            22,
            45,
            500,
            20,
            8.5f,
            FontStyle.Regular,
            ModernUi.Muted));

        _grid.Location = new Point(22, 82);
        _grid.Size = new Size(1028, 315);

        WinFormsUiHelper.ConfigureGrid(_grid);
        ModernUi.ConfigurePremiumGrid(_grid);

        _grid.BackgroundColor = Color.White;
        _grid.BorderStyle = BorderStyle.None;
        _grid.GridColor = ModernUi.Border;
        _grid.Font = ModernUi.UiFont(8.5f);
        _grid.ColumnHeadersHeight = 34;
        _grid.RowTemplate.Height = 32;
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.ReadOnly = true;
        _grid.AllowUserToAddRows = false;
        _grid.AllowUserToDeleteRows = false;

        tableCard.Controls.Add(_grid);
    }

    /// <summary>
    /// Form açılışında kritik stok listesi yüklenir fakat popup gösterilmez.
    /// </summary>
    private void FrmKritikStok_Load(object? sender, EventArgs e)
    {
        Listele(false);
    }

    /// <summary>
    /// Listele butonu kritik stok listesini yeniler ve gerekiyorsa popup gösterir.
    /// </summary>
    private void BtnListele_Click(object? sender, EventArgs e)
    {
        Listele(true);
    }

    private void Listele(bool popupGoster)
    {
        try
        {
            var liste = _urunManager.GetKritikStoktakiler();

            _grid.DataSource = null;
            _grid.DataSource = liste;

            if (liste.Count == 0 && popupGoster)
            {
                BeginInvoke(new Action(() =>
                {
                    WinFormsUiHelper.ShowWarning("Kritik stok seviyesinde ürün bulunmamaktadır.");
                }));

                return;
            }

            if (liste.Count > 0 && popupGoster)
            {
                WinFormsUiHelper.ShowSuccess("Kritik stok listesi başarıyla güncellendi.");
            }
        }
        catch (Exception ex)
        {
            WinFormsUiHelper.ShowError(ex.Message);
        }
    }
}
