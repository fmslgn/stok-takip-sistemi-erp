namespace StokTakip.Entities;

/// <summary>
/// Urunlerin depodan cikis hareketlerini temsil eder.
/// </summary>
public class StokCikis
{
    public int Id { get; set; }
    public int UrunId { get; set; }
    public int KullaniciId { get; set; }
    public int Miktar { get; set; }
    public DateTime CikisTarihi { get; set; } = DateTime.Now;
    public string CikisNedeni { get; set; } = string.Empty;
}
