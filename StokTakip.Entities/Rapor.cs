namespace StokTakip.Entities;

/// <summary>
/// Stok sistemi icinde uretilen rapor kayitlarini temsil eder.
/// </summary>
public class Rapor
{
    public int Id { get; set; }
    public int KullaniciId { get; set; }
    public string RaporTuru { get; set; } = string.Empty;
    public string RaporBasligi { get; set; } = string.Empty;
    public string RaporAciklama { get; set; } = string.Empty;
    public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
}
