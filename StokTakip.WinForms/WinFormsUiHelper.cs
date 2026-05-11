using System.Drawing;

namespace StokTakip.WinForms;

/// <summary>
/// WinForms ekranlarinda tekrar eden stil, grid ve mesaj islemlerini merkezi hale getirir.
/// </summary>
internal static class WinFormsUiHelper
{
    public static readonly Color Navy = Color.FromArgb(18, 45, 78);
    public static readonly Color Blue = Color.FromArgb(37, 99, 235);
    public static readonly Color LightBackground = Color.FromArgb(245, 247, 250);
    public static readonly Color BorderGray = Color.FromArgb(220, 226, 235);
    public static readonly Color Success = Color.FromArgb(22, 163, 74);
    public static readonly Color Danger = Color.FromArgb(220, 38, 38);
    public static readonly Color Warning = Color.FromArgb(245, 158, 11);
    public static readonly Font DefaultFont = new("Segoe UI", 9F);
    public static readonly Font HeaderFont = new("Segoe UI", 16F, FontStyle.Bold);
    public static readonly Font SubtitleFont = new("Segoe UI", 9.5F);

    /// <summary>
    /// Formlar icin ortak arka plan, font ve acilis konumunu ayarlar.
    /// </summary>
    public static void ApplyFormStyle(Form form, string title)
    {
        form.Text = title;
        form.Font = DefaultFont;
        form.BackColor = LightBackground;
        form.StartPosition = FormStartPosition.CenterParent;
    }

    /// <summary>
    /// Ekran basligi ve slogan/aciklama iceren standart ust panel olusturur.
    /// </summary>
    public static Panel CreateHeaderPanel(string title, string subtitle)
    {
        var panel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 82,
            BackColor = Navy
        };

        var lblTitle = new Label
        {
            Text = title,
            Location = new Point(22, 14),
            AutoSize = true,
            ForeColor = Color.White,
            Font = HeaderFont
        };

        var lblSubtitle = new Label
        {
            Text = subtitle,
            Location = new Point(24, 48),
            AutoSize = true,
            ForeColor = Color.FromArgb(210, 224, 242),
            Font = SubtitleFont
        };

        panel.Controls.AddRange(new Control[] { lblTitle, lblSubtitle });
        return panel;
    }

    /// <summary>
    /// Mevcut kontrolleri baslik panelinin altina kaydirip forma standart header ekler.
    /// </summary>
    public static void AddHeader(Form form, string title, string subtitle)
    {
        foreach (Control control in form.Controls)
        {
            control.Top += 82;
        }

        form.ClientSize = new Size(form.ClientSize.Width, form.ClientSize.Height + 82);
        var header = CreateHeaderPanel(title, subtitle);
        form.Controls.Add(header);
        header.BringToFront();
    }

    /// <summary>
    /// Formdaki standart giris kontrollerine ortak font ve kenarlik stilini uygular.
    /// </summary>
    public static void StyleInputs(Control parent)
    {
        foreach (Control control in parent.Controls)
        {
            switch (control)
            {
                case TextBox textBox:
                    StyleTextBox(textBox);
                    break;
                case RichTextBox richTextBox:
                    StyleRichTextBox(richTextBox);
                    break;
                case ComboBox comboBox:
                    StyleComboBox(comboBox);
                    break;
                case NumericUpDown numeric:
                    StyleNumeric(numeric);
                    break;
                case DataGridView grid:
                    StyleDataGridView(grid);
                    break;
            }

            if (control.HasChildren)
            {
                StyleInputs(control);
            }
        }
    }

    /// <summary>
    /// Ana islem butonlarini mavi ve belirgin hale getirir.
    /// </summary>
    public static void StylePrimaryButton(Button button)
    {
        StyleButton(button, Blue, Color.White);
    }

    /// <summary>
    /// Ikincil islemler icin sade buton stili uygular.
    /// </summary>
    public static void StyleSecondaryButton(Button button)
    {
        StyleButton(button, Color.White, Navy);
        button.FlatAppearance.BorderColor = BorderGray;
    }

    /// <summary>
    /// Silme gibi dikkat isteyen islemler icin kirmizi buton stili uygular.
    /// </summary>
    public static void StyleDangerButton(Button button)
    {
        StyleButton(button, Danger, Color.White);
    }

    /// <summary>
    /// Basarili veya pozitif islemler icin yesil buton stili uygular.
    /// </summary>
    public static void StyleSuccessButton(Button button)
    {
        StyleButton(button, Success, Color.White);
    }

    /// <summary>
    /// Metin kutulari icin okunabilir ve sade bir gorunum ayarlar.
    /// </summary>
    public static void StyleTextBox(TextBox textBox)
    {
        textBox.Font = DefaultFont;
        textBox.BorderStyle = BorderStyle.FixedSingle;
        textBox.BackColor = Color.White;
    }

    /// <summary>
    /// RichTextBox kontrolunu sonuc/metin okuma alanina uygun hale getirir.
    /// </summary>
    public static void StyleRichTextBox(RichTextBox textBox)
    {
        textBox.Font = new Font("Segoe UI", 10F);
        textBox.BorderStyle = BorderStyle.FixedSingle;
        textBox.BackColor = Color.White;
    }

    /// <summary>
    /// Combobox kontrolleri icin ortak font ve liste stilini ayarlar.
    /// </summary>
    public static void StyleComboBox(ComboBox comboBox)
    {
        comboBox.Font = DefaultFont;
        comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        comboBox.BackColor = Color.White;
    }

    /// <summary>
    /// NumericUpDown kontrolleri icin ortak gorunum ayarlar.
    /// </summary>
    public static void StyleNumeric(NumericUpDown numeric)
    {
        numeric.Font = DefaultFont;
        numeric.BorderStyle = BorderStyle.FixedSingle;
        numeric.BackColor = Color.White;
    }

    /// <summary>
    /// Liste ekranlarinda kullanilan DataGridView icin ortak ayarlari yapar.
    /// </summary>
    public static void ConfigureGrid(DataGridView grid)
    {
        StyleDataGridView(grid);
    }

    /// <summary>
    /// DataGridView kontrollerini okunabilir ve modern hale getirir.
    /// </summary>
    public static void StyleDataGridView(DataGridView grid)
    {
        grid.ReadOnly = true;
        grid.MultiSelect = false;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.RowHeadersVisible = false;
        grid.BackgroundColor = Color.White;
        grid.BorderStyle = BorderStyle.None;
        grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        grid.EnableHeadersVisualStyles = false;
        grid.ColumnHeadersDefaultCellStyle.BackColor = Navy;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        grid.ColumnHeadersHeight = 34;
        grid.RowTemplate.Height = 30;
        grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
        grid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(15, 23, 42);
        grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
    }

    /// <summary>
    /// Paneli beyaz kart gibi gosterir.
    /// </summary>
    public static void StyleCardPanel(Panel panel)
    {
        panel.BackColor = Color.White;
        panel.BorderStyle = BorderStyle.FixedSingle;
    }

    /// <summary>
    /// Basarili islemlerden sonra kullaniciya bilgi verir.
    /// </summary>
    public static void ShowInfo(string message)
    {
        MessageBox.Show(message, "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    /// <summary>
    /// Validasyon veya veritabani hatalarini kullaniciya gosterir.
    /// </summary>
    public static void ShowError(string message)
    {
        MessageBox.Show(message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    /// <summary>
    /// Silme islemlerinde kullanicidan onay alir.
    /// </summary>
    public static bool ConfirmDelete()
    {
        return MessageBox.Show("Seçili kaydı silmek istiyor musunuz?", "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
    }

    private static void StyleButton(Button button, Color backColor, Color foreColor)
    {
        button.FlatStyle = FlatStyle.Flat;
        button.BackColor = backColor;
        button.ForeColor = foreColor;
        button.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        button.FlatAppearance.BorderSize = 1;
        button.FlatAppearance.BorderColor = backColor;
        button.Cursor = Cursors.Hand;
    }
}
