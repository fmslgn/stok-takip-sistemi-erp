using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using StokTakip.Entities;

namespace StokTakip.WinForms;

public class FrmAnaMenu : Form
{
    private readonly Kullanici? _aktifKullanici;

    private Panel _contentPanel = null!;
    private Panel _sidebarPanel = null!;
    private MenuItemControl? _activeMenuItem;

    private Form? _currentChildForm;
    private Size _currentChildDesignSize;
    private readonly Dictionary<Control, (Rectangle Bounds, float FontSize)> _originalLayout = new();

    private bool _cikisOnaylandi = false;

    private const int SidebarWidth = 220;
    private const int Gap = 16;
    private const int PageMargin = 20;

    public FrmAnaMenu(Kullanici? aktifKullanici = null)
    {
        _aktifKullanici = aktifKullanici;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        ModernUi.ConfigureForm(this, "SYA Stok Takip Sistemi", 1240, 760);
        BackColor = Color.FromArgb(244, 247, 251);

        FormBorderStyle = FormBorderStyle.Sizable;
        MaximizeBox = true;
        WindowState = FormWindowState.Maximized;
        MinimumSize = new Size(1240, 760);

        BuildSidebar();
        BuildContent();
        BuildDashboard();

        Resize += (_, _) => ResizeMainLayout();
        Shown += (_, _) => ResizeMainLayout();
        FormClosing += FrmAnaMenu_FormClosing;
    }

    private void ResizeMainLayout()
    {
        if (_sidebarPanel == null || _contentPanel == null)
            return;

        _sidebarPanel.Location = new Point(PageMargin, PageMargin);
        _sidebarPanel.Size = new Size(SidebarWidth, ClientSize.Height - (PageMargin * 2));
        ApplyRoundedRegion(_sidebarPanel, 24);

        _contentPanel.Location = new Point(PageMargin + SidebarWidth + Gap, PageMargin);
        _contentPanel.Size = new Size(
            ClientSize.Width - SidebarWidth - Gap - (PageMargin * 2),
            ClientSize.Height - (PageMargin * 2));

        if (_currentChildForm == null)
        {
            BuildDashboard();
        }
        else
        {
            FitCurrentChildForm();
        }
    }

    private void BuildSidebar()
    {
        _sidebarPanel = CreateRoundedPanel(
            PageMargin,
            PageMargin,
            SidebarWidth,
            ClientSize.Height - (PageMargin * 2),
            Color.White,
            24);

        Controls.Add(_sidebarPanel);

        var sidebar = _sidebarPanel;

        var logoBox = CreateRoundedPanel(18, 20, 42, 42, ModernUi.Dark, 21);
        sidebar.Controls.Add(logoBox);

        logoBox.Controls.Add(CreatePlainLabel(
            "◈",
            11,
            10,
            20,
            20,
            10f,
            FontStyle.Bold,
            Color.White,
            ContentAlignment.MiddleCenter));

        sidebar.Controls.Add(CreatePlainLabel(
            "SYA",
            76,
            20,
            90,
            20,
            10.8f,
            FontStyle.Bold,
            ModernUi.Dark,
            ContentAlignment.MiddleLeft));

        sidebar.Controls.Add(CreatePlainLabel(
            "Stoklarını düzenle, işini",
            76,
            42,
            120,
            15,
            7.5f,
            FontStyle.Regular,
            ModernUi.Muted,
            ContentAlignment.MiddleLeft));

        sidebar.Controls.Add(CreatePlainLabel(
            "kolaylaştır.",
            76,
            58,
            90,
            15,
            7.5f,
            FontStyle.Regular,
            ModernUi.Muted,
            ContentAlignment.MiddleLeft));

        string kullaniciAdi = _aktifKullanici?.KullaniciAdi ?? "admin";

        var userCard = CreateRoundedPanel(18, 94, 184, 68, Color.FromArgb(248, 250, 252), 18);
        sidebar.Controls.Add(userCard);

        userCard.Controls.Add(CreatePlainLabel(
            "◎  Giriş Yapan Kullanıcı",
            16,
            12,
            150,
            18,
            7.7f,
            FontStyle.Bold,
            ModernUi.Dark,
            ContentAlignment.MiddleLeft));

        userCard.Controls.Add(CreatePlainLabel(
            $"{kullaniciAdi} · yönetici",
            16,
            36,
            145,
            18,
            7.5f,
            FontStyle.Regular,
            ModernUi.Muted,
            ContentAlignment.MiddleLeft));

        AddMenuItem(sidebar, "Kullanıcı Yönetimi", 205, false, () => OpenForm(new FrmKullaniciYonetimi()));
        AddMenuItem(sidebar, "Ürün Yönetimi", 248, true, () => OpenForm(new FrmUrunYonetimi()));
        AddMenuItem(sidebar, "Stok Giriş", 291, false, () => OpenForm(new FrmStokGiris(_aktifKullanici?.Id ?? 1)));
        AddMenuItem(sidebar, "Stok Çıkış", 334, false, () => OpenForm(new FrmStokCikis(_aktifKullanici?.Id ?? 1)));
        AddMenuItem(sidebar, "Kritik Stok", 377, false, () => OpenForm(new FrmKritikStok()));
        AddMenuItem(sidebar, "Raporlama", 420, false, () => OpenForm(new FrmRaporlama()));

        var divider = new Panel
        {
            Location = new Point(24, 612),
            Size = new Size(170, 1),
            BackColor = Color.FromArgb(235, 239, 244)
        };
        sidebar.Controls.Add(divider);

        sidebar.Controls.Add(CreatePlainLabel(
            "OTURUM",
            24,
            628,
            80,
            18,
            8f,
            FontStyle.Bold,
            ModernUi.Muted,
            ContentAlignment.MiddleLeft));

        AddMenuItem(sidebar, "Çıkış", 656, false, CikisYap, true);
    }

    private void AddMenuItem(Control parent, string text, int y, bool active, Action action, bool danger = false)
    {
        var item = new MenuItemControl
        {
            Location = new Point(18, y),
            Size = new Size(184, danger ? 38 : 40),
            MenuText = text,
            IconText = GetMenuIcon(text),
            IsActive = active,
            IsDanger = danger,
            Cursor = Cursors.Hand,
            BackColor = Color.Transparent
        };

        if (active)
            _activeMenuItem = item;

        item.Click += (_, _) =>
        {
            if (!danger)
                SetActiveMenuItem(item);

            item.Refresh();
            Application.DoEvents();

            Cursor.Current = Cursors.WaitCursor;

            try
            {
                action();
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        };

        parent.Controls.Add(item);
    }

    private void SetActiveMenuItem(MenuItemControl item)
    {
        if (_activeMenuItem != null && _activeMenuItem != item)
        {
            _activeMenuItem.IsActive = false;
            _activeMenuItem.Invalidate();
        }

        _activeMenuItem = item;
        _activeMenuItem.IsActive = true;
        _activeMenuItem.Invalidate();
    }

    private void CikisYap()
    {
        bool onay = WinFormsUiHelper.Confirm(
            "Uygulamadan çıkmak istiyor musunuz?",
            "Çıkış Onayı");

        if (onay)
        {
            _cikisOnaylandi = true;
            Close();
        }
    }

    private void FrmAnaMenu_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (_cikisOnaylandi)
            return;

        if (e.CloseReason == CloseReason.UserClosing)
        {
            bool onay = WinFormsUiHelper.Confirm(
                "Uygulamadan çıkmak istiyor musunuz?",
                "Çıkış Onayı");

            if (!onay)
            {
                e.Cancel = true;
                return;
            }

            _cikisOnaylandi = true;
        }
    }

    private string GetMenuIcon(string text)
    {
        return text switch
        {
            "Kullanıcı Yönetimi" => "◎",
            "Ürün Yönetimi" => "▣",
            "Stok Giriş" => "⇩",
            "Stok Çıkış" => "⇧",
            "Kritik Stok" => "⚠",
            "Raporlama" => "☰",
            "Çıkış" => "↩",
            _ => "•"
        };
    }

    private void BuildContent()
    {
        _contentPanel = new Panel
        {
            Location = new Point(PageMargin + SidebarWidth + Gap, PageMargin),
            Size = new Size(
                ClientSize.Width - SidebarWidth - Gap - (PageMargin * 2),
                ClientSize.Height - (PageMargin * 2)),
            BackColor = Color.FromArgb(244, 247, 251),
            AutoScroll = false
        };

        _contentPanel.Resize += (_, _) =>
        {
            if (_currentChildForm != null)
                FitCurrentChildForm();
        };

        Controls.Add(_contentPanel);
    }

    private void BuildDashboard()
    {
        _currentChildForm = null;
        _originalLayout.Clear();

        _contentPanel.Controls.Clear();

        int w = Math.Max(960, _contentPanel.ClientSize.Width);

        int heroTop = 25;
        int heroHeight = 255;

        int moduleTop = heroTop + heroHeight + 60;
        int moduleHeight = Math.Max(420, _contentPanel.ClientSize.Height - moduleTop);

        var hero = CreateRoundedPanel(0, heroTop, w, heroHeight, Color.FromArgb(248, 250, 255), 22);
        _contentPanel.Controls.Add(hero);

        hero.Controls.Add(ModernUi.Label(
            "Stok Yönetim Paneli",
            32,
            28,
            300,
            24,
            10.5f,
            FontStyle.Bold,
            ModernUi.Accent));

        hero.Controls.Add(ModernUi.Label(
            "SYA ile stok süreçlerini tek ekrandan yönet",
            32,
            68,
            w - 80,
            44,
            20f,
            FontStyle.Bold,
            ModernUi.Dark));

        hero.Controls.Add(ModernUi.Label(
            "Ürün yönetimi, stok giriş-çıkış, kritik stok kontrolü ve raporlama işlemlerini modern, anlaşılır ve hızlı bir panelden yönetin.",
            55,
            118,
            w - 120,
            42,
            9.4f,
            FontStyle.Regular,
            ModernUi.Muted));

        int infoGap = 36;
        int infoW = (w - 64 - (infoGap * 2)) / 3;

        hero.Controls.Add(CreateInfoBox(
            "Kolay Kullanım",
            "Sade ve anlaşılır ekran yapısı",
            32,
            176,
            infoW,
            ModernUi.AccentSoft,
            ModernUi.Accent));

        hero.Controls.Add(CreateInfoBox(
            "Anlık Stok Takibi",
            "Giriş ve çıkış hareketleri",
            32 + infoW + infoGap,
            176,
            infoW,
            ModernUi.CyanSoft,
            ModernUi.Cyan));

        hero.Controls.Add(CreateInfoBox(
            "Düzenli Yönetim",
            "Raporlama ve kritik stok",
            32 + ((infoW + infoGap) * 2),
            176,
            infoW,
            ModernUi.GreenSoft,
            ModernUi.Green));

        var modules = CreateRoundedPanel(0, moduleTop, w, moduleHeight, Color.White, 22);
        _contentPanel.Controls.Add(modules);

        modules.Controls.Add(ModernUi.Label(
            "Modül Paneli",
            36,
            26,
            320,
            38,
            18f,
            FontStyle.Bold,
            ModernUi.Dark));

        modules.Controls.Add(ModernUi.Label(
            "Uygulamadaki temel işlemlere aşağıdaki kartlardan hızlıca ulaşabilirsiniz.",
            36,
            68,
            760,
            22,
            8.8f,
            FontStyle.Regular,
            ModernUi.Muted));

        int cardGap = 52;
        int cardW = (w - 72 - (cardGap * 2)) / 3;

        int firstRowY = 125;
        int secondRowY = 300;

        AddModuleCard(
            modules,
            "Kullanıcı Yönetimi",
            "Personel ve yetki işlemlerini düzenle.",
            36,
            firstRowY,
            cardW,
            ModernUi.BlueSoft,
            ModernUi.Blue,
            () => OpenForm(new FrmKullaniciYonetimi()));

        AddModuleCard(
            modules,
            "Ürün Yönetimi",
            "Ürün ekleme, güncelleme ve takip işlemleri.",
            36 + cardW + cardGap,
            firstRowY,
            cardW,
            ModernUi.CyanSoft,
            ModernUi.Cyan,
            () => OpenForm(new FrmUrunYonetimi()));

        AddModuleCard(
            modules,
            "Stok Giriş",
            "Depoya gelen ürünleri sisteme işle.",
            36 + ((cardW + cardGap) * 2),
            firstRowY,
            cardW,
            ModernUi.GreenSoft,
            ModernUi.Green,
            () => OpenForm(new FrmStokGiris(_aktifKullanici?.Id ?? 1)));

        AddModuleCard(
            modules,
            "Stok Çıkış",
            "Depodan çıkan ürünleri kaydet.",
            36,
            secondRowY,
            cardW,
            ModernUi.AmberSoft,
            ModernUi.Amber,
            () => OpenForm(new FrmStokCikis(_aktifKullanici?.Id ?? 1)));

        AddModuleCard(
            modules,
            "Kritik Stok",
            "Azalan ürünleri kontrol et.",
            36 + cardW + cardGap,
            secondRowY,
            cardW,
            ModernUi.RedSoft,
            ModernUi.Red,
            () => OpenForm(new FrmKritikStok()));

        AddModuleCard(
            modules,
            "Raporlama",
            "Stok özetlerini görüntüle.",
            36 + ((cardW + cardGap) * 2),
            secondRowY,
            cardW,
            ModernUi.PurpleSoft,
            ModernUi.Purple,
            () => OpenForm(new FrmRaporlama()));
    }

    private Panel CreateInfoBox(string title, string desc, int x, int y, int width, Color bg, Color accent)
    {
        var box = CreateRoundedPanel(x, y, width, 58, bg, 16);

        var line = new Panel
        {
            Location = new Point(16, 14),
            Size = new Size(4, 30),
            BackColor = accent
        };

        box.Controls.Add(line);

        box.Controls.Add(ModernUi.Label(
            title,
            32,
            10,
            width - 50,
            20,
            9f,
            FontStyle.Bold,
            ModernUi.Dark));

        box.Controls.Add(ModernUi.Label(
            desc,
            32,
            31,
            width - 50,
            18,
            8.2f,
            FontStyle.Regular,
            ModernUi.Muted));

        return box;
    }

    private void AddModuleCard(
        Control parent,
        string title,
        string desc,
        int x,
        int y,
        int width,
        Color bg,
        Color accent,
        Action action)
    {
        var card = CreateRoundedPanel(x, y, width, 132, bg, 18);

        var iconBox = CreateRoundedPanel(18, 18, 46, 46, Color.White, 14);

        iconBox.Controls.Add(ModernUi.Label(
            "●",
            15,
            11,
            20,
            20,
            11f,
            FontStyle.Bold,
            accent));

        card.Controls.Add(iconBox);

        card.Controls.Add(ModernUi.Label(
            title,
            78,
            20,
            width - 105,
            24,
            11.4f,
            FontStyle.Bold,
            ModernUi.Dark));

        card.Controls.Add(ModernUi.Label(
            desc,
            78,
            48,
            width - 105,
            38,
            8.6f,
            FontStyle.Regular,
            ModernUi.Muted));

        var btn = ModernUi.Button("Modülü Aç", 78, 92, 118, 32, true);
        btn.Click += (_, _) => action();

        card.Controls.Add(btn);
        parent.Controls.Add(card);
    }

    private void OpenForm(Form form)
    {
        _contentPanel.Controls.Clear();
        _originalLayout.Clear();

        _currentChildForm = form;
        _currentChildDesignSize = form.ClientSize.Width > 0 && form.ClientSize.Height > 0
            ? form.ClientSize
            : new Size(960, 705);

        form.TopLevel = false;
        form.FormBorderStyle = FormBorderStyle.None;
        form.StartPosition = FormStartPosition.Manual;
        form.Dock = DockStyle.Fill;
        form.AutoScroll = false;

        _contentPanel.Controls.Add(form);

        form.Show();

        CaptureOriginalLayout(form);
        FitCurrentChildForm();

        form.BringToFront();
    }

    private void CaptureOriginalLayout(Control parent)
    {
        foreach (Control control in parent.Controls)
        {
            if (!_originalLayout.ContainsKey(control))
            {
                _originalLayout.Add(control, (control.Bounds, control.Font.Size));
            }

            if (control.HasChildren)
            {
                CaptureOriginalLayout(control);
            }
        }
    }

    private void FitCurrentChildForm()
    {
        if (_currentChildForm == null ||
            _currentChildDesignSize.Width <= 0 ||
            _currentChildDesignSize.Height <= 0)
        {
            return;
        }

        float scaleX = _contentPanel.ClientSize.Width / (float)_currentChildDesignSize.Width;
        float scaleY = _contentPanel.ClientSize.Height / (float)_currentChildDesignSize.Height;

        scaleX = Math.Max(1f, scaleX);
        scaleY = Math.Max(1f, scaleY);

        _currentChildForm.SuspendLayout();

        foreach (Control control in _currentChildForm.Controls)
        {
            ApplyScale(control, scaleX, scaleY);
        }

        _currentChildForm.ResumeLayout();
    }

    private void ApplyScale(Control control, float scaleX, float scaleY)
    {
        if (_originalLayout.TryGetValue(control, out var original))
        {
            control.Bounds = new Rectangle(
                (int)(original.Bounds.X * scaleX),
                (int)(original.Bounds.Y * scaleY),
                Math.Max(1, (int)(original.Bounds.Width * scaleX)),
                Math.Max(1, (int)(original.Bounds.Height * scaleY)));

            float fontScale = Math.Min(scaleX, scaleY);

            if (fontScale > 1.05f)
            {
                control.Font = new Font(
                    control.Font.FontFamily,
                    original.FontSize * Math.Min(fontScale, 1.22f),
                    control.Font.Style);
            }
        }

        foreach (Control child in control.Controls)
        {
            ApplyScale(child, scaleX, scaleY);
        }
    }

    private Label CreatePlainLabel(
        string text,
        int x,
        int y,
        int width,
        int height,
        float fontSize,
        FontStyle style,
        Color color,
        ContentAlignment align)
    {
        return new Label
        {
            Text = text,
            Location = new Point(x, y),
            Size = new Size(width, height),
            Font = ModernUi.UiFont(fontSize, style),
            ForeColor = color,
            BackColor = Color.Transparent,
            TextAlign = align,
            AutoSize = false
        };
    }

    private Panel CreateRoundedPanel(int x, int y, int width, int height, Color backColor, int radius)
    {
        var panel = new Panel
        {
            Location = new Point(x, y),
            Size = new Size(width, height),
            BackColor = backColor
        };

        panel.Resize += (_, _) => ApplyRoundedRegion(panel, radius);
        ApplyRoundedRegion(panel, radius);

        return panel;
    }

    private void ApplyRoundedRegion(Control control, int radius)
    {
        if (control.Width <= 0 || control.Height <= 0)
            return;

        using var path = GetRoundedPath(new Rectangle(0, 0, control.Width, control.Height), radius);
        control.Region = new Region(path);
    }

    private GraphicsPath GetRoundedPath(Rectangle rect, int radius)
    {
        int d = radius * 2;
        var path = new GraphicsPath();

        path.AddArc(rect.X, rect.Y, d, d, 180, 90);
        path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
        path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
        path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
        path.CloseFigure();

        return path;
    }

    private class MenuItemControl : Control
    {
        private bool _hover;

        public string MenuText { get; set; } = "";
        public string IconText { get; set; } = "";
        public bool IsActive { get; set; }
        public bool IsDanger { get; set; }

        public MenuItemControl()
        {
            DoubleBuffered = true;

            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor,
                true);

            Font = ModernUi.UiFont(8.8f, FontStyle.Regular);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.Clear(Parent?.BackColor ?? Color.White);

            Color bgColor;
            Color textColor;

            if (IsActive)
            {
                bgColor = ModernUi.Dark;
                textColor = Color.White;
            }
            else if (IsDanger)
            {
                bgColor = Color.FromArgb(255, 245, 245);
                textColor = Color.FromArgb(185, 28, 28);
            }
            else if (_hover)
            {
                bgColor = Color.FromArgb(248, 250, 252);
                textColor = ModernUi.Dark;
            }
            else
            {
                bgColor = Color.White;
                textColor = ModernUi.Muted;
            }

            if (IsActive || IsDanger || _hover)
            {
                using var bgBrush = new SolidBrush(bgColor);
                using var path = RoundedRect(new Rectangle(0, 0, Width - 1, Height - 1), 18);
                e.Graphics.FillPath(bgBrush, path);
            }

            using var iconFont = new Font("Segoe UI Symbol", 10.5f, FontStyle.Regular);
            using var textFont = ModernUi.UiFont(9f, IsActive ? FontStyle.Bold : FontStyle.Regular);
            using var textBrush = new SolidBrush(textColor);

            using var sfIcon = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            using var sfText = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter
            };

            e.Graphics.DrawString(
                IconText,
                iconFont,
                textBrush,
                new RectangleF(14, 0, 26, Height),
                sfIcon);

            e.Graphics.DrawString(
                MenuText,
                textFont,
                textBrush,
                new RectangleF(48, 0, Width - 58, Height),
                sfText);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _hover = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _hover = false;
            Invalidate();
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            Invalidate();
        }

        private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            var path = new GraphicsPath();

            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }
    }
}