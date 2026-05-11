using System.Drawing;
using StokTakip.Business;
using StokTakip.Entities;

namespace StokTakip.WinForms;

/// <summary>
/// Urun kartlarinin ve stok bilgilerinin Business katmani uzerinden yonetildigi formdur.
/// </summary>
public class FrmUrunYonetimi : Form
{
    private readonly UrunManager _urunManager = new();
    private readonly KategoriManager _kategoriManager = new();
    private readonly SaklamaKosuluManager _saklamaKosuluManager = new();
    private readonly ComboBox _cmbKategori = new();
    private readonly ComboBox _cmbSaklamaKosulu = new();
    private readonly TextBox _txtUrunAdi = new();
    private readonly TextBox _txtBarkod = new();
    private readonly TextBox _txtBirim = new();
    private readonly NumericUpDown _numStok = new();
    private readonly NumericUpDown _numKritik = new();
    private readonly NumericUpDown _numAlis = new();
    private readonly NumericUpDown _numSatis = new();
    private readonly TextBox _txtAciklama = new();
    private readonly CheckBox _chkAktif = new();
    private readonly DataGridView _grid = new();
    private int _seciliId;

    public FrmUrunYonetimi()
    {
        InitializeComponent();
        Load += FrmUrunYonetimi_Load;
    }

    /// <summary>
    /// Urun yonetimi kontrollerini hazirlar.
    /// </summary>
    private void InitializeComponent()
    {
        WinFormsUiHelper.ApplyFormStyle(this, "Ürün Yönetimi");
        ClientSize = new Size(1080, 660);

        AddLabel("Kategori", 24, 24);
        AddLabel("Saklama Koşulu", 24, 64);
        AddLabel("Ürün Adı", 24, 104);
        AddLabel("Barkod", 24, 144);
        AddLabel("Birim", 24, 184);
        AddLabel("Stok", 430, 24);
        AddLabel("Kritik Stok", 430, 64);
        AddLabel("Alış Fiyatı", 430, 104);
        AddLabel("Satış Fiyatı", 430, 144);
        AddLabel("Açıklama", 430, 184);

        _cmbKategori.Location = new Point(150, 20);
        _cmbKategori.Size = new Size(240, 27);
        _cmbKategori.DropDownStyle = ComboBoxStyle.DropDownList;
        _cmbSaklamaKosulu.Location = new Point(150, 60);
        _cmbSaklamaKosulu.Size = new Size(240, 27);
        _cmbSaklamaKosulu.DropDownStyle = ComboBoxStyle.DropDownList;
        _txtUrunAdi.Location = new Point(150, 100);
        _txtUrunAdi.Size = new Size(240, 27);
        _txtBarkod.Location = new Point(150, 140);
        _txtBarkod.Size = new Size(240, 27);
        _txtBirim.Location = new Point(150, 180);
        _txtBirim.Size = new Size(240, 27);

        ConfigureNumber(_numStok, 560, 20, 0);
        ConfigureNumber(_numKritik, 560, 60, 0);
        ConfigureNumber(_numAlis, 560, 100, 2);
        ConfigureNumber(_numSatis, 560, 140, 2);

        _txtAciklama.Location = new Point(560, 180);
        _txtAciklama.Size = new Size(300, 27);
        _chkAktif.Text = "Aktif";
        _chkAktif.Location = new Point(880, 22);
        _chkAktif.Checked = true;

        var btnEkle = new Button { Text = "Ekle", Location = new Point(24, 230), Size = new Size(90, 32) };
        var btnGuncelle = new Button { Text = "Güncelle", Location = new Point(124, 230), Size = new Size(90, 32) };
        var btnSil = new Button { Text = "Sil", Location = new Point(224, 230), Size = new Size(90, 32) };
        var btnTemizle = new Button { Text = "Temizle", Location = new Point(324, 230), Size = new Size(90, 32) };
        var btnListele = new Button { Text = "Listele / Yenile", Location = new Point(424, 230), Size = new Size(130, 32) };

        _grid.Location = new Point(24, 285);
        _grid.Size = new Size(1030, 340);
        WinFormsUiHelper.ConfigureGrid(_grid);

        btnEkle.Click += BtnEkle_Click;
        btnGuncelle.Click += BtnGuncelle_Click;
        btnSil.Click += BtnSil_Click;
        btnTemizle.Click += BtnTemizle_Click;
        btnListele.Click += BtnListele_Click;
        _grid.CellClick += Grid_CellClick;

        WinFormsUiHelper.StyleInputs(this);
        WinFormsUiHelper.StyleSuccessButton(btnEkle);
        WinFormsUiHelper.StylePrimaryButton(btnGuncelle);
        WinFormsUiHelper.StyleDangerButton(btnSil);
        WinFormsUiHelper.StyleSecondaryButton(btnTemizle);
        WinFormsUiHelper.StyleSecondaryButton(btnListele);

        Controls.AddRange(new Control[] { _cmbKategori, _cmbSaklamaKosulu, _txtUrunAdi, _txtBarkod, _txtBirim, _numStok, _numKritik, _numAlis, _numSatis, _txtAciklama, _chkAktif, btnEkle, btnGuncelle, btnSil, btnTemizle, btnListele, _grid });
        WinFormsUiHelper.StyleInputs(this);
        WinFormsUiHelper.AddHeader(this, "Ürün Yönetimi", "Ürün kartlarını, fiyatları ve stok seviyelerini tek ekrandan takip edin.");
    }

    /// <summary>
    /// Form acilisinda comboboxlar ve urun listesi yuklenir.
    /// </summary>
    private void FrmUrunYonetimi_Load(object? sender, EventArgs e)
    {
        KategorileriYukle();
        SaklamaKosullariniYukle();
        Listele();
    }

    /// <summary>
    /// Ekle butonu yeni urun kaydini Business katmanina gonderir.
    /// </summary>
    private void BtnEkle_Click(object? sender, EventArgs e)
    {
        try
        {
            _urunManager.Add(FormdanUrunOlustur());
            WinFormsUiHelper.ShowInfo("Ürün başarıyla eklendi.");
            Temizle();
            Listele();
        }
        catch (Exception ex)
        {
            WinFormsUiHelper.ShowError(ex.Message);
        }
    }

    /// <summary>
    /// Guncelle butonu secili urun kaydini gunceller.
    /// </summary>
    private void BtnGuncelle_Click(object? sender, EventArgs e)
    {
        try
        {
            if (_seciliId <= 0)
            {
                WinFormsUiHelper.ShowError("Güncellemek için bir ürün seçiniz.");
                return;
            }

            var urun = FormdanUrunOlustur();
            urun.Id = _seciliId;
            _urunManager.Update(urun);
            WinFormsUiHelper.ShowInfo("Ürün başarıyla güncellendi.");
            Temizle();
            Listele();
        }
        catch (Exception ex)
        {
            WinFormsUiHelper.ShowError(ex.Message);
        }
    }

    /// <summary>
    /// Sil butonu onay aldiktan sonra secili urunu siler.
    /// </summary>
    private void BtnSil_Click(object? sender, EventArgs e)
    {
        try
        {
            if (_seciliId <= 0)
            {
                WinFormsUiHelper.ShowError("Silmek için bir ürün seçiniz.");
                return;
            }

            if (!WinFormsUiHelper.ConfirmDelete())
            {
                return;
            }

            _urunManager.Delete(_seciliId);
            WinFormsUiHelper.ShowInfo("Ürün başarıyla silindi.");
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
    /// Listele butonu urun listesini yeniler.
    /// </summary>
    private void BtnListele_Click(object? sender, EventArgs e)
    {
        Listele();
    }

    /// <summary>
    /// Grid satirina tiklaninca urun bilgileri forma aktarilir.
    /// </summary>
    private void Grid_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || _grid.CurrentRow?.DataBoundItem is not Urun urun)
        {
            return;
        }

        _seciliId = urun.Id;
        _cmbKategori.SelectedValue = urun.KategoriId;
        _cmbSaklamaKosulu.SelectedValue = urun.SaklamaKosuluId ?? 0;
        _txtUrunAdi.Text = urun.UrunAdi;
        _txtBarkod.Text = urun.Barkod;
        _txtBirim.Text = urun.Birim;
        _numStok.Value = Clamp(urun.StokMiktari, _numStok);
        _numKritik.Value = Clamp(urun.KritikStokSeviyesi, _numKritik);
        _numAlis.Value = Clamp(urun.AlisFiyati, _numAlis);
        _numSatis.Value = Clamp(urun.SatisFiyati, _numSatis);
        _txtAciklama.Text = urun.Aciklama;
        _chkAktif.Checked = urun.AktifMi;
    }

    private void KategorileriYukle()
    {
        try
        {
            _cmbKategori.DataSource = _kategoriManager.GetAll();
            _cmbKategori.DisplayMember = nameof(Kategori.KategoriAdi);
            _cmbKategori.ValueMember = nameof(Kategori.Id);
        }
        catch (Exception ex)
        {
            WinFormsUiHelper.ShowError(ex.Message);
        }
    }

    private void SaklamaKosullariniYukle()
    {
        try
        {
            var kosullar = _saklamaKosuluManager.GetAll();
            kosullar.Insert(0, new SaklamaKosulu { Id = 0, KosulAdi = "Seçilmedi" });
            _cmbSaklamaKosulu.DataSource = kosullar;
            _cmbSaklamaKosulu.DisplayMember = nameof(SaklamaKosulu.KosulAdi);
            _cmbSaklamaKosulu.ValueMember = nameof(SaklamaKosulu.Id);
        }
        catch (Exception ex)
        {
            WinFormsUiHelper.ShowError(ex.Message);
        }
    }

    private void Listele()
    {
        try
        {
            _grid.DataSource = null;
            _grid.DataSource = _urunManager.GetAll();
        }
        catch (Exception ex)
        {
            WinFormsUiHelper.ShowError(ex.Message);
        }
    }

    private Urun FormdanUrunOlustur()
    {
        return new Urun
        {
            KategoriId = _cmbKategori.SelectedValue is int kategoriId ? kategoriId : 0,
            SaklamaKosuluId = _cmbSaklamaKosulu.SelectedValue is int kosulId && kosulId > 0 ? kosulId : null,
            UrunAdi = _txtUrunAdi.Text.Trim(),
            Barkod = _txtBarkod.Text.Trim(),
            Birim = _txtBirim.Text.Trim(),
            StokMiktari = Convert.ToInt32(_numStok.Value),
            KritikStokSeviyesi = Convert.ToInt32(_numKritik.Value),
            AlisFiyati = _numAlis.Value,
            SatisFiyati = _numSatis.Value,
            Aciklama = _txtAciklama.Text.Trim(),
            AktifMi = _chkAktif.Checked
        };
    }

    private void Temizle()
    {
        _seciliId = 0;
        if (_cmbKategori.Items.Count > 0) _cmbKategori.SelectedIndex = 0;
        if (_cmbSaklamaKosulu.Items.Count > 0) _cmbSaklamaKosulu.SelectedIndex = 0;
        _txtUrunAdi.Clear();
        _txtBarkod.Clear();
        _txtBirim.Clear();
        _numStok.Value = 0;
        _numKritik.Value = 0;
        _numAlis.Value = 0;
        _numSatis.Value = 0;
        _txtAciklama.Clear();
        _chkAktif.Checked = true;
        _grid.ClearSelection();
    }

    private void AddLabel(string text, int x, int y)
    {
        Controls.Add(new Label { Text = text, Location = new Point(x, y + 4), AutoSize = true });
    }

    private static void ConfigureNumber(NumericUpDown numeric, int x, int y, int decimalPlaces)
    {
        numeric.Location = new Point(x, y);
        numeric.Size = new Size(120, 27);
        numeric.DecimalPlaces = decimalPlaces;
        numeric.Maximum = 1000000;
    }

    private static decimal Clamp(decimal value, NumericUpDown numeric)
    {
        return Math.Min(Math.Max(value, numeric.Minimum), numeric.Maximum);
    }
}
