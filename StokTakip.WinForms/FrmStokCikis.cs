using System;
using System.Drawing;
using System.Windows.Forms;
using StokTakip.Business;
using StokTakip.Entities;

namespace StokTakip.WinForms;

/// <summary>
/// Depodan ürün çıkış hareketlerinin Business katmanı üzerinden kaydedildiği formdur.
/// </summary>
public class FrmStokCikis : Form
{
    private readonly StokCikisManager _stokCikisManager = new();
    private readonly UrunManager _urunManager = new();

    private readonly int _aktifKullaniciId;

    private readonly ComboBox _cmbUrun = new();
    private readonly NumericUpDown _numMiktar = new();
    private readonly TextBox _txtAciklama = new();
    private readonly DataGridView _grid = new();

    public FrmStokCikis(int aktifKullaniciId = 1)
    {
        _aktifKullaniciId = aktifKullaniciId <= 0 ? 1 : aktifKullaniciId;
        InitializeComponent();
        Load += FrmStokCikis_Load;
    }

    private void InitializeComponent()
    {
        ModernUi.ConfigureForm(this, "Stok Çıkış", 1120, 720);

        FormBorderStyle = FormBorderStyle.None;
        MaximizeBox = false;
        BackColor = Color.FromArgb(244, 247, 251);

        BuildLayout();
    }

    private void BuildLayout()
    {
        Controls.Clear();

        BuildHeader();
        BuildFormCard();
        BuildInfoCard();
        BuildTableCard();
    }

    private void BuildHeader()
    {
        var headerCard = ModernUi.CardPanel(24, 20, 1072, 82);
        headerCard.BackColor = Color.White;
        Controls.Add(headerCard);

        headerCard.Controls.Add(ModernUi.Label(
            "Stok Çıkış",
            28,
            16,
            400,
            34,
            17f,
            FontStyle.Bold,
            ModernUi.Dark));

        headerCard.Controls.Add(ModernUi.Label(
            "Satılan veya depodan çıkan ürünleri sistemden düşerek stok miktarını güncelleyebilirsiniz.",
            28,
            52,
            800,
            22,
            8.8f,
            FontStyle.Regular,
            ModernUi.Muted));

        var badge = ModernUi.CardPanel(1000, 22, 45, 24);
        badge.BackColor = Color.FromArgb(248, 250, 252);
        headerCard.Controls.Add(badge);

        badge.Controls.Add(ModernUi.Label(
            "SYA",
            11,
            4,
            30,
            16,
            7f,
            FontStyle.Bold,
            ModernUi.Muted));
    }

    private void BuildFormCard()
    {
        var formCard = ModernUi.CardPanel(24, 122, 520, 300);
        formCard.BackColor = Color.White;
        Controls.Add(formCard);

        formCard.Controls.Add(ModernUi.Label(
            "Stok Çıkış Formu",
            24,
            20,
            260,
            26,
            12f,
            FontStyle.Bold,
            ModernUi.Dark));

        formCard.Controls.Add(ModernUi.Label(
            "Ürün, miktar ve açıklama bilgilerini girerek stok çıkış hareketi oluşturun.",
            24,
            48,
            420,
            20,
            8f,
            FontStyle.Regular,
            ModernUi.Muted));

        AddLabel(formCard, "Ürün Seç", 24, 84);
        ConfigureComboBox(_cmbUrun, 24, 108, 455, 32);
        formCard.Controls.Add(_cmbUrun);

        AddLabel(formCard, "Çıkış Miktarı", 24, 152);
        ConfigureNumber(_numMiktar, 24, 176, 455, 32);
        formCard.Controls.Add(_numMiktar);

        AddLabel(formCard, "Açıklama", 24, 220);
        ConfigureTextBox(_txtAciklama, 24, 244, 455, 32, "Stok çıkış açıklaması");
        formCard.Controls.Add(_txtAciklama);
    }

    private void BuildInfoCard()
    {
        var infoCard = ModernUi.CardPanel(570, 122, 526, 300);
        infoCard.BackColor = Color.White;
        Controls.Add(infoCard);

        infoCard.Controls.Add(ModernUi.Label(
            "İşlem Paneli",
            24,
            20,
            250,
            26,
            12f,
            FontStyle.Bold,
            ModernUi.Dark));

        infoCard.Controls.Add(ModernUi.Label(
            "Seçilen ürünün stok miktarı, girilen miktar kadar azaltılır. Stok yetersizse sistem popup uyarısı verir.",
            24,
            50,
            450,
            40,
            8.4f,
            FontStyle.Regular,
            ModernUi.Muted));

        var infoBox = ModernUi.MessageBoxPanel(
            "Bilgi",
            "Stok çıkış işleminden sonra ürün listesi ve çıkış kayıtları otomatik yenilenir.",
            24,
            104,
            470,
            64,
            "success");

        infoCard.Controls.Add(infoBox);

        var warningBox = ModernUi.MessageBoxPanel(
            "Kontrol",
            "Stok yetersizse çıkış işlemi sistem tarafından engellenebilir.",
            24,
            180,
            470,
            58,
            "warning");

        infoCard.Controls.Add(warningBox);

        var btnCikis = ModernUi.Button("Stok Çıkışı Yap", 24, 250, 150, 36, true);
        btnCikis.Font = ModernUi.UiFont(8.8f, FontStyle.Bold);
        btnCikis.Click += BtnCikis_Click;
        infoCard.Controls.Add(btnCikis);

        var btnTemizle = ModernUi.Button("Temizle", 186, 250, 100, 36);
        btnTemizle.Font = ModernUi.UiFont(8.8f, FontStyle.Bold);
        btnTemizle.Click += BtnTemizle_Click;
        infoCard.Controls.Add(btnTemizle);

        var btnListele = ModernUi.Button("Listele / Yenile", 298, 250, 140, 36);
        btnListele.Font = ModernUi.UiFont(8.8f, FontStyle.Bold);
        btnListele.Click += BtnListele_Click;
        infoCard.Controls.Add(btnListele);
    }

    private void BuildTableCard()
    {
        var tableCard = ModernUi.CardPanel(24, 450, 1072, 245);
        tableCard.BackColor = Color.White;
        Controls.Add(tableCard);

        tableCard.Controls.Add(ModernUi.Label(
            "Son Çıkış Kayıtları",
            24,
            16,
            300,
            26,
            12f,
            FontStyle.Bold,
            ModernUi.Dark));

        tableCard.Controls.Add(ModernUi.Label(
            "Depodan düşülen ürünlerin son stok çıkış hareketleri burada listelenir.",
            24,
            43,
            520,
            20,
            8f,
            FontStyle.Regular,
            ModernUi.Muted));

        _grid.Location = new Point(24, 74);
        _grid.Size = new Size(1024, 145);

        ConfigureGridVisual();

        tableCard.Controls.Add(_grid);
    }

    private void FrmStokCikis_Load(object? sender, EventArgs e)
    {
        UrunleriYukle();
        Listele();
    }

    private void BtnCikis_Click(object? sender, EventArgs e)
    {
        try
        {
            if (_cmbUrun.SelectedValue is not int urunId || urunId <= 0)
            {
                WinFormsUiHelper.ShowWarning("Lütfen stok çıkışı yapılacak ürünü seçiniz.");
                return;
            }

            if (_numMiktar.Value <= 0)
            {
                WinFormsUiHelper.ShowWarning("Çıkış miktarı 0'dan büyük olmalıdır.");
                return;
            }

            _stokCikisManager.Add(new StokCikis
            {
                UrunId = urunId,
                KullaniciId = _aktifKullaniciId,
                Miktar = Convert.ToInt32(_numMiktar.Value),
                Aciklama = _txtAciklama.Text.Trim(),
                CikisTarihi = DateTime.Now
            });

            WinFormsUiHelper.ShowSuccess("Stok çıkışı başarıyla yapıldı.");

            Temizle();
            UrunleriYukle();
            Listele();
        }
        catch (Exception ex)
        {
            WinFormsUiHelper.ShowError(ex.Message);
        }
    }

    private void BtnTemizle_Click(object? sender, EventArgs e)
    {
        Temizle();
    }

    private void BtnListele_Click(object? sender, EventArgs e)
    {
        UrunleriYukle();
        Listele();
        WinFormsUiHelper.ShowInfo("Stok çıkış kayıtları yenilendi.");
    }

    private void UrunleriYukle()
    {
        try
        {
            _cmbUrun.DataSource = null;
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
            _grid.DataSource = _stokCikisManager.GetAll();
            GridKolonlariniDuzenle();
        }
        catch (Exception ex)
        {
            WinFormsUiHelper.ShowError(ex.Message);
        }
    }

    private void GridKolonlariniDuzenle()
    {
        if (_grid.Columns.Count == 0)
            return;

        SetHeader("Id", "ID");
        SetHeader("UrunId", "Ürün ID");
        SetHeader("KullaniciId", "Kullanıcı ID");
        SetHeader("Miktar", "Miktar");
        SetHeader("CikisTarihi", "Çıkış Tarihi");
        SetHeader("Aciklama", "Açıklama");

        SetDisplayIndex("Id", 0);
        SetDisplayIndex("UrunId", 1);
        SetDisplayIndex("KullaniciId", 2);
        SetDisplayIndex("Miktar", 3);
        SetDisplayIndex("CikisTarihi", 4);
        SetDisplayIndex("Aciklama", 5);

        if (_grid.Columns.Contains("Id"))
            _grid.Columns["Id"].Width = 70;

        if (_grid.Columns.Contains("CikisTarihi"))
            _grid.Columns["CikisTarihi"].FillWeight = 130;

        if (_grid.Columns.Contains("Aciklama"))
            _grid.Columns["Aciklama"].FillWeight = 170;
    }

    private void SetHeader(string columnName, string headerText)
    {
        if (_grid.Columns.Contains(columnName))
            _grid.Columns[columnName].HeaderText = headerText;
    }

    private void SetDisplayIndex(string columnName, int index)
    {
        if (_grid.Columns.Contains(columnName))
            _grid.Columns[columnName].DisplayIndex = index;
    }

    private void Temizle()
    {
        if (_cmbUrun.Items.Count > 0)
            _cmbUrun.SelectedIndex = 0;

        _numMiktar.Value = 0;
        _txtAciklama.Clear();
        _cmbUrun.Focus();
    }

    private void ConfigureGridVisual()
    {
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
        _grid.RowHeadersVisible = false;
    }

    private void AddLabel(Control parent, string text, int x, int y)
    {
        parent.Controls.Add(ModernUi.Label(
            text,
            x,
            y,
            160,
            18,
            8.2f,
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

    private static void ConfigureNumber(NumericUpDown numeric, int x, int y, int width, int height)
    {
        numeric.Location = new Point(x, y);
        numeric.Size = new Size(width, height);
        numeric.Maximum = 1000000;
        numeric.Minimum = 0;
        numeric.Font = ModernUi.UiFont(8.8f);
        numeric.BorderStyle = BorderStyle.FixedSingle;
        numeric.BackColor = Color.White;
        numeric.ForeColor = ModernUi.Dark;
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
}