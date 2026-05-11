namespace StokTakip.Entities;

/// <summary>
/// Stok sistemi icinde uretilen rapor kayitlarini temsil eder.
/// </summary>
public class Rapor
{
    public int Id { get; set; }
    public string RaporAdi { get; set; } = string.Empty;
    public string RaporTuru { get; set; } = string.Empty;
    public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
    public string Aciklama { get; set; } = string.Empty;
}
