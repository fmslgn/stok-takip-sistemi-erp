namespace StokTakip.WinForms;

/// <summary>
/// WinForms ekranlarında tekrar eden grid ve mesaj işlemlerini merkezi hale getirir.
/// </summary>
internal static class WinFormsUiHelper
{
    /// <summary>
    /// Liste ekranlarında kullanılan DataGridView için ortak ayarları yapar.
    /// </summary>
    public static void ConfigureGrid(DataGridView grid)
    {
        grid.ReadOnly = true;
        grid.MultiSelect = false;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.RowHeadersVisible = false;
    }

    /// <summary>
    /// Bilgi amaçlı mesaj gösterir.
    /// </summary>
    public static void ShowInfo(string message)
    {
        MessageBox.Show(
            message,
            "Bilgi",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    /// <summary>
    /// Başarılı işlemlerden sonra kullanıcıya bilgi verir.
    /// </summary>
    public static void ShowSuccess(string message)
    {
        MessageBox.Show(
            message,
            "Başarılı",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    /// <summary>
    /// Kullanıcı uyarılarını pop-up olarak gösterir.
    /// </summary>
    public static void ShowWarning(string message)
    {
        MessageBox.Show(
            message,
            "Uyarı",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning);
    }

    /// <summary>
    /// Validasyon veya veritabanı hatalarını kullanıcıya gösterir.
    /// </summary>
    public static void ShowError(string message)
    {
        MessageBox.Show(
            message,
            "Hata",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }

    /// <summary>
    /// Silme işlemlerinde kullanıcıdan onay alır.
    /// </summary>
    public static bool ConfirmDelete()
    {
        return MessageBox.Show(
            "Seçili kaydı silmek istiyor musunuz?",
            "Silme Onayı",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question) == DialogResult.Yes;
    }

    /// <summary>
    /// Genel onay mesajı gösterir.
    /// </summary>
    public static bool Confirm(string message, string title = "Onay")
    {
        return MessageBox.Show(
            message,
            title,
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question) == DialogResult.Yes;
    }
}