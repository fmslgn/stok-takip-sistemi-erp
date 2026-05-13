using System.Drawing;
using StokTakip.Business;
using StokTakip.Entities;

namespace StokTakip.WinForms;

/// <summary>
/// Urun kategorilerinin Business katmani uzerinden yonetildigi formdur.
/// </summary>
public class FrmKategori : Form
{
    private readonly KategoriManager _kategoriManager = new();
    private readonly TextBox _txtKategoriAdi = new();
    private readonly TextBox _txtAciklama = new();
    private readonly CheckBox _chkAktif = new();
    private readonly DataGridView _grid = new();
    private int _seciliId;

    public FrmKategori()
    {
        InitializeComponent();
        Load += FrmKategori_Load;
    }

    /// <summary>
    /// Kategori formundaki kontrolleri hazirlar.
    /// </summary>
    private void InitializeComponent()
    {
        Text = "Kategori Yönetimi";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(860, 520);

        var lblKategoriAdi = new Label { Text = "Kategori Adı", Location = new Point(24, 26), AutoSize = true };
        var lblAciklama = new Label { Text = "Açıklama", Location = new Point(24, 66), AutoSize = true };
        var btnEkle = new Button { Text = "Ekle", Location = new Point(24, 140), Size = new Size(90, 32) };
        var btnGuncelle = new Button { Text = "Güncelle", Location = new Point(124, 140), Size = new Size(90, 32) };
        var btnSil = new Button { Text = "Sil", Location = new Point(224, 140), Size = new Size(90, 32) };
        var btnTemizle = new Button { Text = "Temizle", Location = new Point(324, 140), Size = new Size(90, 32) };
        var btnListele = new Button { Text = "Listele / Yenile", Location = new Point(424, 140), Size = new Size(130, 32) };

        _txtKategoriAdi.Location = new Point(130, 22);
        _txtKategoriAdi.Size = new Size(250, 27);
        _txtAciklama.Location = new Point(130, 62);
        _txtAciklama.Size = new Size(420, 27);
        _chkAktif.Text = "Aktif";
        _chkAktif.Location = new Point(130, 100);
        _chkAktif.Checked = true;

        _grid.Location = new Point(24, 190);
        _grid.Size = new Size(812, 300);
        WinFormsUiHelper.ConfigureGrid(_grid);

        btnEkle.Click += BtnEkle_Click;
        btnGuncelle.Click += BtnGuncelle_Click;
        btnSil.Click += BtnSil_Click;
        btnTemizle.Click += BtnTemizle_Click;
        btnListele.Click += BtnListele_Click;
        _grid.CellClick += Grid_CellClick;

        Controls.AddRange(new Control[] { lblKategoriAdi, _txtKategoriAdi, lblAciklama, _txtAciklama, _chkAktif, btnEkle, btnGuncelle, btnSil, btnTemizle, btnListele, _grid });
    }

    /// <summary>
    /// Form acilisinda kategori listesi yuklenir.
    /// </summary>
    private void FrmKategori_Load(object? sender, EventArgs e)
    {
        Listele();
    }

    /// <summary>
    /// Ekle butonu Business katmanindaki validasyonlardan gecerek kategori ekler.
    /// </summary>
    private void BtnEkle_Click(object? sender, EventArgs e)
    {
        try
        {
            _kategoriManager.Add(FormdanKategoriOlustur());
            WinFormsUiHelper.ShowInfo("Kategori başarıyla eklendi.");
            Temizle();
            Listele();
        }
        catch (Exception ex)
        {
            WinFormsUiHelper.ShowError(ex.Message);
        }
    }

    /// <summary>
    /// Guncelle butonu secili kategori kaydini gunceller.
    /// </summary>
    private void BtnGuncelle_Click(object? sender, EventArgs e)
    {
        try
        {
            if (_seciliId <= 0)
            {
                WinFormsUiHelper.ShowError("Güncellemek için bir kategori seçiniz.");
                return;
            }

            var kategori = FormdanKategoriOlustur();
            kategori.Id = _seciliId;
            _kategoriManager.Update(kategori);
            WinFormsUiHelper.ShowInfo("Kategori başarıyla güncellendi.");
            Temizle();
            Listele();
        }
        catch (Exception ex)
        {
            WinFormsUiHelper.ShowError(ex.Message);
        }
    }

    /// <summary>
    /// Sil butonu onay aldiktan sonra secili kategoriyi siler.
    /// </summary>
    private void BtnSil_Click(object? sender, EventArgs e)
    {
        try
        {
            if (_seciliId <= 0)
            {
                WinFormsUiHelper.ShowError("Silmek için bir kategori seçiniz.");
                return;
            }

            if (!WinFormsUiHelper.ConfirmDelete())
            {
                return;
            }

            _kategoriManager.Delete(_seciliId);
            WinFormsUiHelper.ShowInfo("Kategori başarıyla silindi.");
            Temizle();
            Listele();
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
        Temizle();
    }

    /// <summary>
    /// Listele butonu kategori listesini yeniler.
    /// </summary>
    private void BtnListele_Click(object? sender, EventArgs e)
    {
        Listele();
    }

    /// <summary>
    /// Grid satirina tiklaninca kategori bilgileri forma aktarilir.
    /// </summary>
    private void Grid_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || _grid.CurrentRow?.DataBoundItem is not Kategori kategori)
        {
            return;
        }

        _seciliId = kategori.Id;
        _txtKategoriAdi.Text = kategori.KategoriAdi;
        _txtAciklama.Text = kategori.Aciklama;
        _chkAktif.Checked = kategori.AktifMi;
    }

    private void Listele()
    {
        try
        {
            _grid.DataSource = null;
            _grid.DataSource = _kategoriManager.GetAll();
        }
        catch (Exception ex)
        {
            WinFormsUiHelper.ShowError(ex.Message);
        }
    }

    private Kategori FormdanKategoriOlustur()
    {
        return new Kategori
        {
            KategoriAdi = _txtKategoriAdi.Text.Trim(),
            Aciklama = _txtAciklama.Text.Trim(),
            AktifMi = _chkAktif.Checked
        };
    }

    private void Temizle()
    {
        _seciliId = 0;
        _txtKategoriAdi.Clear();
        _txtAciklama.Clear();
        _chkAktif.Checked = true;
        _grid.ClearSelection();
    }
}
