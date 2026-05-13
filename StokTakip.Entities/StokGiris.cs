namespace StokTakip.Entities;

/// <summary>
/// Urunlerin depoya giris hareketlerini temsil eder.
/// </summary>
public class StokGiris
{
    public int Id { get; set; }
    public int UrunId { get; set; }
    public int KullaniciId { get; set; }
    public int Miktar { get; set; }
    public DateTime GirisTarihi { get; set; } = DateTime.Now;
    public string Aciklama { get; set; } = string.Empty;
}
