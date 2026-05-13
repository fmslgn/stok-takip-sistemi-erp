using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
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
    private readonly TextBox _txtAciklama = new();

    private readonly NumericUpDown _numStok = new();
    private readonly NumericUpDown _numKritik = new();
    private readonly NumericUpDown _numAlis = new();
    private readonly NumericUpDown _numSatis = new();

    private readonly CheckBox _chkAktif = new();
    private readonly DataGridView _grid = new();

    private readonly ComboBox _cmbFiltreKategori = new();
    private readonly ComboBox _cmbFiltreUrunAdi = new();

    private Panel _tableCard = null!;
    private Panel _overlayPanel = null!;

    private Button _btnFiltrele = null!;
    private Button _btnFiltreTemizle = null!;
    private Button _btnListele = null!;
    private Button _btnListeBuyut = null!;
    private Button _btnPopupKucult = null!;
    private Button _btnSaklamaAsistani = null!;

    private Label _lblUrunListesiBaslik = null!;
    private Label _lblKategoriFiltresi = null!;
    private Label _lblUrunAdiFiltresi = null!;

    private readonly Rectangle _tableNormalBounds = new(570, 120, 526, 612);

    private Rectangle _savedTableBounds;
    private Rectangle _savedGridBounds;
    private Rectangle _savedKategoriFilterBounds;
    private Rectangle _savedUrunFilterBounds;
    private Rectangle _savedBtnFiltreleBounds;
    private Rectangle _savedBtnFiltreTemizleBounds;
    private Rectangle _savedBtnListeleBounds;
    private Rectangle _savedBtnListeBuyutBounds;
    private Rectangle _savedLblUrunListesiBaslikBounds;
    private Rectangle _savedLblKategoriFiltresiBounds;
    private Rectangle _savedLblUrunAdiFiltresiBounds;

    private int _seciliId;
    private bool _formDolduruluyor;
    private bool _listeBuyukMu;

    public FrmUrunYonetimi()
    {
        InitializeComponent();
        Load += FrmUrunYonetimi_Load;
        Resize += FrmUrunYonetimi_Resize;
    }

    /// <summary>
    /// Urun yonetimi kontrollerini hazirlar.
    /// </summary>
    private void InitializeComponent()
    {
        ModernUi.ConfigureForm(this, "Ürün Yönetimi", 1120, 760);

        FormBorderStyle = FormBorderStyle.None;
        MaximizeBox = false;
        BackColor = Color.FromArgb(244, 247, 251);

        BuildLayout();
    }

    private void BuildLayout()
    {
        Controls.Clear();

        // Ana icerik iki sutuna ayrilir: solda urun formu ve islemler, sagda urun listesi bulunur.
        BuildHeader();
        BuildFormCard();
        BuildActionCard();
        BuildTableCard();
        CreateOverlayPanel();
    }

    private void BuildHeader()
    {
        var headerCard = ModernUi.CardPanel(24, 20, 1072, 78);
        headerCard.BackColor = Color.White;
        Controls.Add(headerCard);

        headerCard.Controls.Add(ModernUi.Label(
            "Ürün Yönetimi",
            24,
            13,
            400,
            30,
            15.5f,
            FontStyle.Bold,
            ModernUi.Dark));

        headerCard.Controls.Add(ModernUi.Label(
            "Ürün ekleme, güncelleme, silme ve kategori altında düzenli takip işlemleri.",
            24,
            45,
            700,
            22,
            8.2f,
            FontStyle.Regular,
            ModernUi.Muted));

        _btnSaklamaAsistani = ModernUi.Button("💬 Saklama Asistanı", 790, 22, 185, 34, true);
        _btnSaklamaAsistani.Font = ModernUi.UiFont(8f, FontStyle.Bold);
        _btnSaklamaAsistani.Click += BtnSaklamaAsistani_Click;
        headerCard.Controls.Add(_btnSaklamaAsistani);

        var badge = ModernUi.CardPanel(1000, 18, 45, 25);
        badge.BackColor = Color.FromArgb(248, 250, 252);
        headerCard.Controls.Add(badge);

        badge.Controls.Add(ModernUi.Label(
            "SYA",
            11,
            5,
            30,
            16,
            7f,
            FontStyle.Bold,
            ModernUi.Muted));
    }

    private void BuildFormCard()
    {
        var formCard = ModernUi.CardPanel(24, 120, 520, 330);
        formCard.BackColor = Color.White;
        Controls.Add(formCard);

        formCard.Controls.Add(ModernUi.Label(
            "Ürün Bilgi Formu",
            22,
            18,
            250,
            24,
            11f,
            FontStyle.Bold,
            ModernUi.Dark));

        AddLabel(formCard, "Kategori", 22, 58, 120);
        ConfigureComboBox(_cmbKategori, 22, 80, 220, 32);
        formCard.Controls.Add(_cmbKategori);

        AddLabel(formCard, "Saklama Koşulu", 270, 58, 140);
        ConfigureComboBox(_cmbSaklamaKosulu, 270, 80, 220, 32);
        formCard.Controls.Add(_cmbSaklamaKosulu);

        AddLabel(formCard, "Ürün Adı", 22, 122, 120);
        ConfigureTextBox(_txtUrunAdi, 22, 144, 220, 32, "Ürün adı");
        _txtUrunAdi.TextChanged += TxtUrunAdi_TextChanged;
        formCard.Controls.Add(_txtUrunAdi);

        AddLabel(formCard, "Barkod", 270, 122, 120);
        ConfigureTextBox(_txtBarkod, 270, 144, 220, 32, "Barkod");
        formCard.Controls.Add(_txtBarkod);

        AddLabel(formCard, "Birim", 22, 186, 120);
        ConfigureTextBox(_txtBirim, 22, 208, 220, 32, "Adet / Kutu / Kg");
        formCard.Controls.Add(_txtBirim);

        AddLabel(formCard, "Açıklama", 270, 186, 120);
        ConfigureTextBox(_txtAciklama, 270, 208, 220, 32, "Açıklama");
        formCard.Controls.Add(_txtAciklama);

        AddLabel(formCard, "Stok", 22, 250, 80);
        ConfigureNumber(_numStok, 22, 272, 100, 0);
        formCard.Controls.Add(_numStok);

        AddLabel(formCard, "Kritik Stok", 150, 250, 95);
        ConfigureNumber(_numKritik, 150, 272, 100, 0);
        formCard.Controls.Add(_numKritik);

        AddLabel(formCard, "Alış Fiyatı", 278, 250, 95);
        ConfigureNumber(_numAlis, 278, 272, 100, 2);
        formCard.Controls.Add(_numAlis);

        AddLabel(formCard, "Satış Fiyatı", 392, 250, 100);
        ConfigureNumber(_numSatis, 392, 272, 100, 2);
        formCard.Controls.Add(_numSatis);
    }

    /// <summary>
    /// Urun kaydi icin aktiflik ve CRUD butonlarini sade bir islem kartinda toplar.
    /// </summary>
    private void BuildActionCard()
    {
        var actionCard = ModernUi.CardPanel(24, 464, 520, 82);
        actionCard.BackColor = Color.White;
        Controls.Add(actionCard);

        actionCard.Controls.Add(ModernUi.Label(
            "Ürün İşlemleri",
            22,
            18,
            250,
            24,
            11f,
            FontStyle.Bold,
            ModernUi.Dark));

        _chkAktif.Text = "Aktif ürün";
        _chkAktif.Location = new Point(22, 48);
        _chkAktif.Size = new Size(120, 24);
        _chkAktif.Checked = true;
        _chkAktif.Font = ModernUi.UiFont(8.8f);
        _chkAktif.BackColor = Color.Transparent;
        actionCard.Controls.Add(_chkAktif);

        var btnEkle = ModernUi.Button("Ekle", 160, 42, 78, 30, true);
        btnEkle.Font = ModernUi.UiFont(8.2f, FontStyle.Bold);
        btnEkle.Click += BtnEkle_Click;
        actionCard.Controls.Add(btnEkle);

        var btnGuncelle = ModernUi.Button("Güncelle", 248, 42, 90, 30);
        btnGuncelle.Font = ModernUi.UiFont(8.2f, FontStyle.Bold);
        btnGuncelle.Click += BtnGuncelle_Click;
        actionCard.Controls.Add(btnGuncelle);

        var btnSil = ModernUi.Button("Sil", 348, 42, 65, 30);
        btnSil.Font = ModernUi.UiFont(8.2f, FontStyle.Bold);
        btnSil.Click += BtnSil_Click;
        actionCard.Controls.Add(btnSil);

        var btnTemizle = ModernUi.Button("Temizle", 423, 42, 80, 30);
        btnTemizle.Font = ModernUi.UiFont(8.2f, FontStyle.Bold);
        btnTemizle.Click += BtnTemizle_Click;
        actionCard.Controls.Add(btnTemizle);
    }

    /// <summary>
    /// Urun listesini sag sutunda filtreler ve tabloyla birlikte gosterir.
    /// </summary>
    private void BuildTableCard()
    {
        _tableCard = ModernUi.CardPanel(
            _tableNormalBounds.X,
            _tableNormalBounds.Y,
            _tableNormalBounds.Width,
            _tableNormalBounds.Height);

        _tableCard.BackColor = Color.White;
        Controls.Add(_tableCard);

        _lblUrunListesiBaslik = ModernUi.Label(
            "Ürün Listesi",
            22,
            14,
            220,
            26,
            11f,
            FontStyle.Bold,
            ModernUi.Dark);
        _tableCard.Controls.Add(_lblUrunListesiBaslik);

        _lblKategoriFiltresi = ModernUi.Label(
            "Kategori Filtresi",
            22,
            50,
            170,
            18,
            8f,
            FontStyle.Bold,
            ModernUi.Text);
        _tableCard.Controls.Add(_lblKategoriFiltresi);

        ConfigureComboBox(_cmbFiltreKategori, 22, 72, 220, 30);
        _tableCard.Controls.Add(_cmbFiltreKategori);

        _lblUrunAdiFiltresi = ModernUi.Label(
            "Ürün Adı Filtresi",
            270,
            50,
            170,
            18,
            8f,
            FontStyle.Bold,
            ModernUi.Text);
        _tableCard.Controls.Add(_lblUrunAdiFiltresi);

        ConfigureFilterProductCombo(_cmbFiltreUrunAdi, 270, 72, 220, 30);
        _tableCard.Controls.Add(_cmbFiltreUrunAdi);

        _btnFiltrele = ModernUi.Button("Filtrele", 22, 118, 105, 32, true);
        _btnFiltrele.Font = ModernUi.UiFont(8.2f, FontStyle.Bold);
        _btnFiltrele.Click += BtnFiltrele_Click;
        _tableCard.Controls.Add(_btnFiltrele);

        _btnFiltreTemizle = ModernUi.Button("Filtre Temizle", 139, 118, 125, 32);
        _btnFiltreTemizle.Font = ModernUi.UiFont(8.2f, FontStyle.Bold);
        _btnFiltreTemizle.Click += BtnFiltreTemizle_Click;
        _tableCard.Controls.Add(_btnFiltreTemizle);

        _btnListele = ModernUi.Button("Listele / Yenile", 276, 118, 130, 32);
        _btnListele.Font = ModernUi.UiFont(8.2f, FontStyle.Bold);
        _btnListele.Click += BtnListele_Click;
        _tableCard.Controls.Add(_btnListele);

        _btnListeBuyut = ModernUi.Button("⛶", 418, 118, 72, 32);
        _btnListeBuyut.Font = ModernUi.UiFont(9f, FontStyle.Bold);
        _btnListeBuyut.Click += BtnListeBuyut_Click;
        _tableCard.Controls.Add(_btnListeBuyut);

        _btnPopupKucult = ModernUi.Button("↙ Küçült", 930, 18, 112, 34);
        _btnPopupKucult.Font = ModernUi.UiFont(8.2f, FontStyle.Bold);
        _btnPopupKucult.Visible = false;
        _btnPopupKucult.Click += BtnPopupKucult_Click;
        _tableCard.Controls.Add(_btnPopupKucult);

        _grid.Location = new Point(22, 170);
        _grid.Size = new Size(482, 414);

        WinFormsUiHelper.ConfigureGrid(_grid);
        ModernUi.ConfigurePremiumGrid(_grid);

        _grid.BackgroundColor = Color.White;
        _grid.BorderStyle = BorderStyle.None;
        _grid.GridColor = ModernUi.Border;
        _grid.Font = ModernUi.UiFont(8.5f);
        _grid.ColumnHeadersHeight = 34;
        _grid.RowTemplate.Height = 32;
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
        _grid.ScrollBars = ScrollBars.Both;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.MultiSelect = false;
        _grid.ReadOnly = true;
        _grid.AllowUserToAddRows = false;
        _grid.AllowUserToDeleteRows = false;
        _grid.RowHeadersVisible = false;

        _grid.CellClick += Grid_CellClick;

        _tableCard.Controls.Add(_grid);
    }

    private void CreateOverlayPanel()
    {
        _overlayPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(35, 15, 23, 42),
            Visible = false
        };

        Controls.Add(_overlayPanel);
        _overlayPanel.BringToFront();
    }

    private void FrmUrunYonetimi_Resize(object? sender, EventArgs e)
    {
        if (_listeBuyukMu)
        {
            ApplyPopupLayout();
        }
    }

    /// <summary>
    /// Form acilisinda comboboxlar ve urun listesi yuklenir.
    /// </summary>
    private void FrmUrunYonetimi_Load(object? sender, EventArgs e)
    {
        KategorileriYukle();
        SaklamaKosullariniYukle();
        FiltreKategorileriniYukle();
        Listele();
    }

    private void TxtUrunAdi_TextChanged(object? sender, EventArgs e)
    {
        if (_formDolduruluyor)
            return;

        UrunAdiIleOtomatikFiltrele();
    }

    /// <summary>
    /// Ürün adı alanına yazıldıkça tabloyu otomatik filtreler.
    /// </summary>
    private void UrunAdiIleOtomatikFiltrele()
    {
        try
        {
            string arama = _txtUrunAdi.Text.Trim();

            var liste = _urunManager.GetAll();

            if (!string.IsNullOrWhiteSpace(arama))
            {
                liste = liste
                    .Where(x => !string.IsNullOrWhiteSpace(x.UrunAdi)
                             && x.UrunAdi.Contains(arama, StringComparison.CurrentCultureIgnoreCase))
                    .ToList();
            }

            _grid.DataSource = null;
            _grid.DataSource = liste;

            GridKolonlariniDuzenle();
        }
        catch (Exception ex)
        {
            WinFormsUiHelper.ShowError(ex.Message);
        }
    }

    /// <summary>
    /// Ekle butonu yeni urun kaydini Business katmanina gonderir.
    /// </summary>
    private void BtnEkle_Click(object? sender, EventArgs e)
    {
        try
        {
            _urunManager.Add(FormdanUrunOlustur());

            WinFormsUiHelper.ShowSuccess("Ürün başarıyla eklendi.");

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
                WinFormsUiHelper.ShowWarning("Güncellemek için bir ürün seçiniz.");
                return;
            }

            var urun = FormdanUrunOlustur();
            urun.Id = _seciliId;

            _urunManager.Update(urun);

            WinFormsUiHelper.ShowSuccess("Ürün başarıyla güncellendi.");

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
                WinFormsUiHelper.ShowWarning("Silmek için bir ürün seçiniz.");
                return;
            }

            if (!WinFormsUiHelper.ConfirmDelete())
            {
                return;
            }

            _urunManager.Delete(_seciliId);

            WinFormsUiHelper.ShowSuccess("Ürün başarıyla silindi.");

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
    /// Saklama Asistani butonu mevcut urun bilgilerini chat ekranina aktarir.
    /// </summary>
    private void BtnSaklamaAsistani_Click(object? sender, EventArgs e)
    {
        string urunAdi = _txtUrunAdi.Text.Trim();
        string kategoriAdi = SeciliKategoriAdiniAl();
        string barkod = _txtBarkod.Text.Trim();
        string saklamaKosulu = SeciliSaklamaKosuluAdiniAl();

        using var chatForm = new FrmSaklamaAsistaniChat(urunAdi, kategoriAdi, barkod, saklamaKosulu);
        chatForm.ShowDialog(this);
    }

    private void BtnListeBuyut_Click(object? sender, EventArgs e)
    {
        ListeyiPopupAc();
    }

    private void BtnPopupKucult_Click(object? sender, EventArgs e)
    {
        ListeyiPopupKapat();
    }

    private void ListeyiPopupAc()
    {
        if (_listeBuyukMu)
            return;

        SaveNormalListLayout();

        _listeBuyukMu = true;

        _overlayPanel.Visible = true;
        _overlayPanel.BringToFront();

        Controls.Remove(_tableCard);
        _overlayPanel.Controls.Add(_tableCard);

        _tableCard.BringToFront();

        ApplyPopupLayout();

    }

    private void ListeyiPopupKapat()
    {
        if (!_listeBuyukMu)
            return;

        _listeBuyukMu = false;

        _overlayPanel.Controls.Remove(_tableCard);
        Controls.Add(_tableCard);

        ApplyNormalListLayout();

        _overlayPanel.Visible = false;
        _overlayPanel.SendToBack();
        _tableCard.BringToFront();

    }

    private void SaveNormalListLayout()
    {
        _savedTableBounds = _tableCard.Bounds;
        _savedGridBounds = _grid.Bounds;

        _savedLblUrunListesiBaslikBounds = _lblUrunListesiBaslik.Bounds;
        _savedLblKategoriFiltresiBounds = _lblKategoriFiltresi.Bounds;
        _savedLblUrunAdiFiltresiBounds = _lblUrunAdiFiltresi.Bounds;

        _savedKategoriFilterBounds = _cmbFiltreKategori.Bounds;
        _savedUrunFilterBounds = _cmbFiltreUrunAdi.Bounds;

        _savedBtnFiltreleBounds = _btnFiltrele.Bounds;
        _savedBtnFiltreTemizleBounds = _btnFiltreTemizle.Bounds;
        _savedBtnListeleBounds = _btnListele.Bounds;
        _savedBtnListeBuyutBounds = _btnListeBuyut.Bounds;
    }

    private void ApplyNormalListLayout()
    {
        _tableCard.Bounds = _savedTableBounds.Width > 0 ? _savedTableBounds : _tableNormalBounds;

        _lblUrunListesiBaslik.Bounds = _savedLblUrunListesiBaslikBounds;
        _lblKategoriFiltresi.Bounds = _savedLblKategoriFiltresiBounds;
        _lblUrunAdiFiltresi.Bounds = _savedLblUrunAdiFiltresiBounds;

        _cmbFiltreKategori.Bounds = _savedKategoriFilterBounds;
        _cmbFiltreUrunAdi.Bounds = _savedUrunFilterBounds;

        _btnFiltrele.Bounds = _savedBtnFiltreleBounds;
        _btnFiltreTemizle.Bounds = _savedBtnFiltreTemizleBounds;
        _btnListele.Bounds = _savedBtnListeleBounds;
        _btnListeBuyut.Bounds = _savedBtnListeBuyutBounds;

        _grid.Bounds = _savedGridBounds;
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
        _grid.ScrollBars = ScrollBars.Both;

        _btnPopupKucult.Visible = false;
        _btnListeBuyut.Visible = true;
    }

    private void ApplyPopupLayout()
    {
        int popupWidth = Math.Min(1120, _overlayPanel.ClientSize.Width - 140);
        int popupHeight = Math.Min(585, _overlayPanel.ClientSize.Height - 130);

        if (popupWidth < 1040)
            popupWidth = 1040;

        if (popupHeight < 460)
            popupHeight = 460;

        int x = (_overlayPanel.ClientSize.Width - popupWidth) / 2;
        int y = (_overlayPanel.ClientSize.Height - popupHeight) / 2;

        _tableCard.Bounds = new Rectangle(x, y, popupWidth, popupHeight);

        _lblUrunListesiBaslik.SetBounds(24, 18, 260, 28);

        _lblKategoriFiltresi.SetBounds(24, 62, 180, 18);
        _cmbFiltreKategori.SetBounds(24, 84, 255, 30);

        _lblUrunAdiFiltresi.SetBounds(310, 62, 190, 18);
        _cmbFiltreUrunAdi.SetBounds(310, 84, 255, 30);

        _btnFiltrele.SetBounds(590, 82, 110, 34);
        _btnFiltreTemizle.SetBounds(712, 82, 140, 34);
        _btnListele.SetBounds(_tableCard.Width - 175, 82, 145, 34);

        _btnPopupKucult.SetBounds(_tableCard.Width - 142, 18, 112, 34);
        _btnPopupKucult.Visible = true;

        _btnListeBuyut.Visible = false;

        _grid.SetBounds(24, 135, _tableCard.Width - 48, _tableCard.Height - 165);
        _grid.BringToFront();
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
    }

    private void BtnFiltrele_Click(object? sender, EventArgs e)
    {
        Filtrele();
    }

    private void BtnFiltreTemizle_Click(object? sender, EventArgs e)
    {
        if (_cmbFiltreKategori.Items.Count > 0)
            _cmbFiltreKategori.SelectedIndex = 0;

        _cmbFiltreUrunAdi.Text = "";

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

        _formDolduruluyor = true;

        try
        {
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
        finally
        {
            _formDolduruluyor = false;
        }
    }

    private void KategorileriYukle()
    {
        try
        {
            _cmbKategori.DataSource = null;
            _cmbKategori.DataSource = _kategoriManager.GetAll();
            _cmbKategori.DisplayMember = nameof(Kategori.KategoriAdi);
            _cmbKategori.ValueMember = nameof(Kategori.Id);
        }
        catch (Exception ex)
        {
            WinFormsUiHelper.ShowError(ex.Message);
        }
    }

    private void FiltreKategorileriniYukle()
    {
        try
        {
            var kategoriler = _kategoriManager.GetAll();

            kategoriler.Insert(0, new Kategori
            {
                Id = 0,
                KategoriAdi = "Tüm Kategoriler"
            });

            _cmbFiltreKategori.DataSource = null;
            _cmbFiltreKategori.DataSource = kategoriler;
            _cmbFiltreKategori.DisplayMember = nameof(Kategori.KategoriAdi);
            _cmbFiltreKategori.ValueMember = nameof(Kategori.Id);
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

            _cmbSaklamaKosulu.DataSource = null;
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
            var liste = _urunManager.GetAll();

            _grid.DataSource = null;
            _grid.DataSource = liste;

            UrunAdiOnerileriniYukle();
            GridKolonlariniDuzenle();
        }
        catch (Exception ex)
        {
            WinFormsUiHelper.ShowError(ex.Message);
        }
    }

    private void Filtrele()
    {
        try
        {
            var liste = _urunManager.GetAll();

            int kategoriId = _cmbFiltreKategori.SelectedValue is int id ? id : 0;
            string urunAdi = _cmbFiltreUrunAdi.Text.Trim();

            if (kategoriId > 0)
            {
                liste = liste
                    .Where(x => x.KategoriId == kategoriId)
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(urunAdi))
            {
                liste = liste
                    .Where(x => !string.IsNullOrWhiteSpace(x.UrunAdi)
                             && x.UrunAdi.Contains(urunAdi, StringComparison.CurrentCultureIgnoreCase))
                    .ToList();
            }

            _grid.DataSource = null;
            _grid.DataSource = liste;

            GridKolonlariniDuzenle();

            if (liste.Count == 0)
            {
                WinFormsUiHelper.ShowWarning("Seçilen filtrelere uygun ürün bulunamadı.");
            }
        }
        catch (Exception ex)
        {
            WinFormsUiHelper.ShowError(ex.Message);
        }
    }

    private void UrunAdiOnerileriniYukle()
    {
        try
        {
            var urunAdlari = _urunManager.GetAll()
                .Where(x => !string.IsNullOrWhiteSpace(x.UrunAdi))
                .Select(x => x.UrunAdi)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            var autoComplete = new AutoCompleteStringCollection();
            autoComplete.AddRange(urunAdlari.ToArray());

            _txtUrunAdi.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            _txtUrunAdi.AutoCompleteSource = AutoCompleteSource.CustomSource;
            _txtUrunAdi.AutoCompleteCustomSource = autoComplete;

            _cmbFiltreUrunAdi.Items.Clear();
            _cmbFiltreUrunAdi.Items.AddRange(urunAdlari.Cast<object>().ToArray());
            _cmbFiltreUrunAdi.AutoCompleteCustomSource = autoComplete;
        }
        catch
        {
            // Öneri alanı hata verirse ana listeleme süreci bozulmasın.
        }
    }

    private void GridKolonlariniDuzenle()
    {
        if (_grid.Columns.Count == 0)
            return;

        SetHeader("Id", "ID");
        SetHeader("KategoriId", "Kategori ID");
        SetHeader("SaklamaKosuluId", "Saklama Koşulu ID");
        SetHeader("UrunAdi", "Ürün Adı");
        SetHeader("Barkod", "Barkod");
        SetHeader("Birim", "Birim");
        SetHeader("StokMiktari", "Stok");
        SetHeader("KritikStokSeviyesi", "Kritik Stok");
        SetHeader("AlisFiyati", "Alış Fiyatı");
        SetHeader("SatisFiyati", "Satış Fiyatı");
        SetHeader("Aciklama", "Açıklama");
        SetHeader("AktifMi", "Aktif");
        SetHeader("OlusturmaTarihi", "Oluşturma Tarihi");

        SetDisplayIndex("Id", 0);
        SetDisplayIndex("UrunAdi", 1);
        SetDisplayIndex("Barkod", 2);
        SetDisplayIndex("Birim", 3);
        SetDisplayIndex("StokMiktari", 4);
        SetDisplayIndex("KritikStokSeviyesi", 5);
        SetDisplayIndex("AlisFiyati", 6);
        SetDisplayIndex("SatisFiyati", 7);
        SetDisplayIndex("Aciklama", 8);
        SetDisplayIndex("AktifMi", 9);
        SetDisplayIndex("OlusturmaTarihi", 10);

        HideColumn("KategoriId");
        HideColumn("SaklamaKosuluId");

        if (_grid.Columns.Contains("Id"))
            _grid.Columns["Id"].Width = 55;

        if (_grid.Columns.Contains("UrunAdi"))
            _grid.Columns["UrunAdi"].Width = 150;

        if (_grid.Columns.Contains("Barkod"))
            _grid.Columns["Barkod"].Width = 140;

        if (_grid.Columns.Contains("Birim"))
            _grid.Columns["Birim"].Width = 90;

        if (_grid.Columns.Contains("StokMiktari"))
            _grid.Columns["StokMiktari"].Width = 80;

        if (_grid.Columns.Contains("KritikStokSeviyesi"))
            _grid.Columns["KritikStokSeviyesi"].Width = 105;

        if (_grid.Columns.Contains("AlisFiyati"))
            _grid.Columns["AlisFiyati"].Width = 95;

        if (_grid.Columns.Contains("SatisFiyati"))
            _grid.Columns["SatisFiyati"].Width = 95;

        if (_grid.Columns.Contains("Aciklama"))
            _grid.Columns["Aciklama"].Width = 180;

        if (_grid.Columns.Contains("AktifMi"))
            _grid.Columns["AktifMi"].Width = 70;

        if (_grid.Columns.Contains("OlusturmaTarihi"))
            _grid.Columns["OlusturmaTarihi"].Width = 150;
    }

    private void SetHeader(string columnName, string headerText)
    {
        if (_grid.Columns.Contains(columnName))
        {
            _grid.Columns[columnName].HeaderText = headerText;
        }
    }

    private void SetDisplayIndex(string columnName, int index)
    {
        if (_grid.Columns.Contains(columnName))
        {
            _grid.Columns[columnName].DisplayIndex = index;
        }
    }

    private void HideColumn(string columnName)
    {
        if (_grid.Columns.Contains(columnName))
        {
            _grid.Columns[columnName].Visible = false;
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

    private string SeciliKategoriAdiniAl()
    {
        if (_cmbKategori.SelectedItem is Kategori kategori)
        {
            return kategori.KategoriAdi;
        }

        return _cmbKategori.Text.Trim();
    }

    private string SeciliSaklamaKosuluAdiniAl()
    {
        if (_cmbSaklamaKosulu.SelectedItem is SaklamaKosulu saklamaKosulu && saklamaKosulu.Id > 0)
        {
            return saklamaKosulu.KosulAdi;
        }

        return string.Empty;
    }

    private void Temizle()
    {
        _seciliId = 0;

        _formDolduruluyor = true;

        try
        {
            if (_cmbKategori.Items.Count > 0)
                _cmbKategori.SelectedIndex = 0;

            if (_cmbSaklamaKosulu.Items.Count > 0)
                _cmbSaklamaKosulu.SelectedIndex = 0;

            _txtUrunAdi.Clear();
            _txtBarkod.Clear();
            _txtBirim.Clear();
            _txtAciklama.Clear();

            _numStok.Value = 0;
            _numKritik.Value = 0;
            _numAlis.Value = 0;
            _numSatis.Value = 0;

            _chkAktif.Checked = true;
            _grid.ClearSelection();
        }
        finally
        {
            _formDolduruluyor = false;
        }

        Listele();
        _txtUrunAdi.Focus();
    }

    private void AddLabel(Control parent, string text, int x, int y, int width)
    {
        parent.Controls.Add(ModernUi.Label(
            text,
            x,
            y,
            width,
            18,
            8f,
            FontStyle.Bold,
            ModernUi.Text));
    }

    private static void ConfigureComboBox(ComboBox comboBox, int x, int y, int width, int height)
    {
        comboBox.Location = new Point(x, y);
        comboBox.Size = new Size(width, height);
        comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        comboBox.Font = ModernUi.UiFont(8.8f);
        comboBox.BackColor = Color.White;
        comboBox.ForeColor = ModernUi.Dark;
    }

    private static void ConfigureFilterProductCombo(ComboBox comboBox, int x, int y, int width, int height)
    {
        comboBox.Location = new Point(x, y);
        comboBox.Size = new Size(width, height);
        comboBox.DropDownStyle = ComboBoxStyle.DropDown;
        comboBox.Font = ModernUi.UiFont(8.8f);
        comboBox.BackColor = Color.White;
        comboBox.ForeColor = ModernUi.Dark;
        comboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
        comboBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
    }

    private static void ConfigureTextBox(TextBox textBox, int x, int y, int width, int height, string placeholder)
    {
        textBox.Location = new Point(x, y);
        textBox.Size = new Size(width, height);
        textBox.Font = ModernUi.UiFont(8.8f);
        textBox.ForeColor = ModernUi.Dark;
        textBox.BackColor = Color.White;
        textBox.BorderStyle = BorderStyle.FixedSingle;
        textBox.PlaceholderText = placeholder;
    }

    private static void ConfigureNumber(NumericUpDown numeric, int x, int y, int width, int decimalPlaces)
    {
        numeric.Location = new Point(x, y);
        numeric.Size = new Size(width, 32);
        numeric.DecimalPlaces = decimalPlaces;
        numeric.Maximum = 1000000;
        numeric.Minimum = 0;
        numeric.Font = ModernUi.UiFont(8.8f);
        numeric.BorderStyle = BorderStyle.FixedSingle;
        numeric.BackColor = Color.White;
        numeric.ForeColor = ModernUi.Dark;
    }

    private static decimal Clamp(decimal value, NumericUpDown numeric)
    {
        return Math.Min(Math.Max(value, numeric.Minimum), numeric.Maximum);
    }
}
