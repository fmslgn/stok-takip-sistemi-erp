namespace StokTakip.WinForms;

/// <summary>
/// WinForms ekranlarinda tekrar eden grid ve mesaj islemlerini merkezi hale getirir.
/// </summary>
internal static class WinFormsUiHelper
{
    /// <summary>
    /// Liste ekranlarinda kullanilan DataGridView icin ortak ayarlari yapar.
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
        return MessageBox.Show("Secili kaydi silmek istiyor musunuz?", "Silme Onayi", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
    }
}
