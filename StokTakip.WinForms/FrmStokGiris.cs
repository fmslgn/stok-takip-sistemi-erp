using System.Drawing;
using StokTakip.Business;
using StokTakip.Entities;

namespace StokTakip.WinForms;

/// <summary>
/// Depoya urun giris hareketlerinin Business katmani uzerinden kaydedildigi formdur.
/// </summary>
public class FrmStokGiris : Form
{
    private readonly StokGirisManager _stokGirisManager = new();
    private readonly UrunManager _urunManager = new();
    private readonly int _aktifKullaniciId;
    private readonly ComboBox _cmbUrun = new();
    private readonly NumericUpDown _numMiktar = new();
    private readonly TextBox _txtAciklama = new();
    private readonly DataGridView _grid = new();

    public FrmStokGiris(int aktifKullaniciId = 1)
    {
        _aktifKullaniciId = aktifKullaniciId <= 0 ? 1 : aktifKullaniciId;
        InitializeComponent();
        Load += FrmStokGiris_Load;
    }

    /// <summary>
    /// Stok giris formundaki kontrolleri hazirlar.
    /// </summary>
    private void InitializeComponent()
    {
        WinFormsUiHelper.ApplyFormStyle(this, "Stok Giriş");
        ClientSize = new Size(850, 520);

        var lblUrun = new Label { Text = "Ürün", Location = new Point(24, 28), AutoSize = true };
        var lblMiktar = new Label { Text = "Miktar", Location = new Point(24, 68), AutoSize = true };
        var lblAciklama = new Label { Text = "Açıklama", Location = new Point(24, 108), AutoSize = true };
        var btnGiris = new Button { Text = "Stok Girişi Yap", Location = new Point(150, 145), Size = new Size(130, 32) };
        var btnTemizle = new Button { Text = "Temizle", Location = new Point(290, 145), Size = new Size(90, 32) };
        var btnListele = new Button { Text = "Listele / Yenile", Location = new Point(390, 145), Size = new Size(130, 32) };

        _cmbUrun.Location = new Point(150, 24);
        _cmbUrun.Size = new Size(280, 27);
        _cmbUrun.DropDownStyle = ComboBoxStyle.DropDownList;
        _numMiktar.Location = new Point(150, 64);
        _numMiktar.Size = new Size(120, 27);
        _numMiktar.Maximum = 1000000;
        _txtAciklama.Location = new Point(150, 104);
        _txtAciklama.Size = new Size(370, 27);

        _grid.Location = new Point(24, 200);
        _grid.Size = new Size(800, 280);
        WinFormsUiHelper.ConfigureGrid(_grid);

        btnGiris.Click += BtnGiris_Click;
        btnTemizle.Click += BtnTemizle_Click;
        btnListele.Click += BtnListele_Click;

        WinFormsUiHelper.StyleInputs(this);
        WinFormsUiHelper.StyleSuccessButton(btnGiris);
        WinFormsUiHelper.StyleSecondaryButton(btnTemizle);
        WinFormsUiHelper.StyleSecondaryButton(btnListele);

        Controls.AddRange(new Control[] { lblUrun, _cmbUrun, lblMiktar, _numMiktar, lblAciklama, _txtAciklama, btnGiris, btnTemizle, btnListele, _grid });
        WinFormsUiHelper.StyleInputs(this);
        WinFormsUiHelper.AddHeader(this, "Stok Giriş", "Depoya giren ürün hareketlerini kaydedin ve stok miktarını artırın.");
    }

    /// <summary>
    /// Form acilisinda urunler ve stok giris kayitlari yuklenir.
    /// </summary>
    private void FrmStokGiris_Load(object? sender, EventArgs e)
    {
        UrunleriYukle();
        Listele();
    }

    /// <summary>
    /// Stok girisi butonu hareket kaydi olusturur ve Business katmaninda stogu artirir.
    /// </summary>
    private void BtnGiris_Click(object? sender, EventArgs e)
    {
        try
        {
            _stokGirisManager.Add(new StokGiris
            {
                UrunId = _cmbUrun.SelectedValue is int urunId ? urunId : 0,
                KullaniciId = _aktifKullaniciId,
                Miktar = Convert.ToInt32(_numMiktar.Value),
                Aciklama = _txtAciklama.Text.Trim(),
                GirisTarihi = DateTime.Now
            });

            WinFormsUiHelper.ShowInfo("Stok girişi başarıyla yapıldı.");
            Temizle();
            UrunleriYukle();
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
    /// Listele butonu stok giris kayitlarini yeniler.
    /// </summary>
    private void BtnListele_Click(object? sender, EventArgs e)
    {
        UrunleriYukle();
        Listele();
    }

    private void UrunleriYukle()
    {
        try
        {
            _cmbUrun.DataSource = _urunManager.GetAll();
            _cmbUrun.DisplayMember = nameof(Urun.UrunAdi);
            _cmbUrun.ValueMember = nameof(Urun.Id);
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
            _grid.DataSource = _stokGirisManager.GetAll();
        }
        catch (Exception ex)
        {
            WinFormsUiHelper.ShowError(ex.Message);
        }
    }

    private void Temizle()
    {
        if (_cmbUrun.Items.Count > 0) _cmbUrun.SelectedIndex = 0;
        _numMiktar.Value = 0;
        _txtAciklama.Clear();
    }
}
